using Microsoft.Extensions.Options;
using Renci.SshNet;
using RoconMqtt.Can.Models;
using RoconMqtt.Can.Options;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace RoconMqtt.Can;

/// <summary>
/// SSH-based CAN bus implementation using candump and cansend on a remote Linux system.
/// Useful for testing and remote CAN bus access without direct hardware connection.
/// </summary>
public partial class SshCanBus : ICanBus, IDisposable
{
    private const string ConnectionType = "write";
    private readonly ILogger<SshCanBus> _logger;
    private readonly SshOptions _options;
    private readonly string _canInterface;
    private SshClient? _readSshClient;
    private SshClient? _writeSshClient;
    private readonly SemaphoreSlim _readConnectionLock = new(1, 1);
    private readonly SemaphoreSlim _writeConnectionLock = new(1, 1);
    private bool _disposed;

    // Regex to parse candump output: "can0  180   [7]  D2 1D FA 0A 06 01 E0"
    private static readonly Regex CandumpRegex = CanDumpRegex();

    public SshCanBus(ILogger<SshCanBus> logger, IOptions<CanOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(options);
        
        _options = options.Value.Ssh ?? throw new ArgumentException("SSH options are not configured", nameof(options));
        _canInterface = options.Value.CanInterfaceName;
    }

    private async Task EnsureReadConnectionAsync(CancellationToken token)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        if (_readSshClient?.IsConnected == true)
            return;

        await _readConnectionLock.WaitAsync(token);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            if (_readSshClient?.IsConnected == true)
                return;

            LogConnectingToSshHost(_logger, _options.Host, _options.Port, _options.Username, "read");

            _readSshClient?.Dispose();
            _readSshClient = new SshClient(
                _options.Host,
                _options.Port,
                _options.Username,
                _options.Password)
            {
                ConnectionInfo =
                {
                    Timeout = TimeSpan.FromMilliseconds(_options.ConnectionTimeoutMs)
                }
            };

            await Task.Run(() => _readSshClient.Connect(), token);
            LogSshConnected(_logger, _options.Host, "read");

            // Test if candump exists
            var testCommand = _readSshClient.CreateCommand("which candump");
            var result = await Task.Run(() => testCommand.Execute(), token);
            if (string.IsNullOrWhiteSpace(result))
            {
                LogCandumpNotFound(_logger);
                throw new InvalidOperationException("candump command not found on remote system. Please install can-utils package.");
            }

            LogSshCanBusInitialized(_logger, _options.Host, _canInterface, "read");
        }
        catch (Exception ex)
        {
            LogSshConnectionFailed(_logger, ex, _options.Host, "read");
            throw;
        }
        finally
        {
            _readConnectionLock.Release();
        }
    }

    private async Task EnsureWriteConnectionAsync(CancellationToken token)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        if (_writeSshClient?.IsConnected == true)
            return;

        await _writeConnectionLock.WaitAsync(token);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            if (_writeSshClient?.IsConnected == true)
                return;

            LogConnectingToSshHost(_logger, _options.Host, _options.Port, _options.Username, ConnectionType);

            _writeSshClient?.Dispose();
            _writeSshClient = new SshClient(
                _options.Host,
                _options.Port,
                _options.Username,
                _options.Password)
            {
                ConnectionInfo =
                {
                    Timeout = TimeSpan.FromMilliseconds(_options.ConnectionTimeoutMs)
                }
            };

            await Task.Run(() => _writeSshClient.Connect(), token);
            LogSshConnected(_logger, _options.Host, ConnectionType);

            // Test if cansend exists
            var testCommand = _writeSshClient.CreateCommand("which cansend");
            var result = await Task.Run(() => testCommand.Execute(), token);
            if (string.IsNullOrWhiteSpace(result))
            {
                LogCansendNotFound(_logger);
                throw new InvalidOperationException("cansend command not found on remote system. Please install can-utils package.");
            }

            LogSshCanBusInitialized(_logger, _options.Host, _canInterface, ConnectionType);
        }
        catch (Exception ex)
        {
            LogSshConnectionFailed(_logger, ex, _options.Host, ConnectionType);
            throw;
        }
        finally
        {
            _writeConnectionLock.Release();
        }
    }

    public async IAsyncEnumerable<CanFrame> ReadFramesAsync([EnumeratorCancellation] CancellationToken token, uint? canId = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        
        await EnsureReadConnectionAsync(token);

        if (_readSshClient == null)
            throw new InvalidOperationException("Read SSH client is not connected");

        LogStartingCandump(_logger, _canInterface);

        // Use local variable instead of shared field to avoid concurrent access issues
        ShellStream? candumpStream = null;

        try
        {
            // Start candump in a shell stream for continuous output
            candumpStream = _readSshClient.CreateShellStream("candump", 1024, 800, 1024, 600, 4096);
        
            // Execute candump command with optional CAN ID filter
            // Format: candump can0 or candump can0,180:7FF (filters by CAN ID 180 with mask 7FF)
            var candumpCommand = canId.HasValue 
                ? $"candump {_canInterface},{canId.Value:X}:7FF\n"
                : $"candump {_canInterface}\n";
            
            LogCandumpCommandExecuting(_logger, candumpCommand.TrimEnd());
            await candumpStream.WriteAsync(Encoding.UTF8.GetBytes(candumpCommand), token);
            await candumpStream.FlushAsync(token);

            LogCandumpStarted(_logger);

            var buffer = new StringBuilder();
            var readBuffer = new byte[4096];
            var candumpStarted = false;

            // Skip initial shell output (login messages, command echo, etc.)
            var startupTimeout = DateTime.UtcNow.AddSeconds(5);
            while (!candumpStarted && DateTime.UtcNow < startupTimeout && !token.IsCancellationRequested)
            {
                if (candumpStream.DataAvailable)
                {
                    var bytesRead = await candumpStream.ReadAsync(readBuffer.AsMemory(0, readBuffer.Length), token);
                    if (bytesRead > 0)
                    {
                        var output = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);
                        LogRawShellOutput(_logger, output);
                        buffer.Append(output);
                        
                        // Wait until we see the command echo or a small delay passes
                        // This ensures we skip the initial shell prompt and command echo
                        if (buffer.ToString().Contains($"candump {_canInterface}"))
                        {
                            candumpStarted = true;
                            buffer.Clear();
                            LogCandumpOutputStarted(_logger);
                        }
                    }
                }
                else
                {
                    await Task.Delay(10, token);
                }
            }

            if (!candumpStarted)
            {
                LogCandumpStartupTimeout(_logger);
            }

            buffer.Clear();

            while (!token.IsCancellationRequested)
            {
                if (candumpStream.DataAvailable)
                {
                    var bytesRead = await candumpStream.ReadAsync(readBuffer.AsMemory(0, readBuffer.Length), token);
                    if (bytesRead > 0)
                    {
                        var rawData = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);
                        LogRawCandumpData(_logger, rawData);
                        buffer.Append(rawData);
                        
                        // Process complete lines
                        var lines = buffer.ToString().Replace("\r", "").Split('\n');
                        for (int i = 0; i < lines.Length - 1; i++)
                        {
                            var line = lines[i];
                            
                            if (string.IsNullOrWhiteSpace(line))
                                continue;
                                
                            if (line.Contains($"candump {_canInterface}"))
                            {
                                LogSkippingCommandEcho(_logger, line);
                                continue; // Skip command echo line
                            }

                            LogProcessingCandumpLine(_logger, line);
                            var frame = ParseCandumpLine(line);
                            if (frame != null)
                            {
                                LogReceivedCanFrame(_logger, frame.Id, frame.Data.Length);
                                yield return frame;
                            }
                        }
                        
                        // Keep the last incomplete line in buffer
                        buffer.Clear();
                        buffer.Append(lines[^1]);
                    }
                }
                else
                {
                    await Task.Delay(10, token);
                }
            }
        }
        finally
        {
            // Properly terminate candump process with Ctrl+C before disposing
            if (candumpStream != null)
            {
                try
                {
                    LogStoppingCandump(_logger);
                    // Send Ctrl+C (ASCII 0x03) to terminate candump gracefully
                    candumpStream.Write("\x03");
                    await candumpStream.FlushAsync(CancellationToken.None);
                    
                    // Wait a bit for the process to terminate
                    await Task.Delay(100, CancellationToken.None);
                    
                    await candumpStream.DisposeAsync();
                    LogCandumpStopped(_logger);
                }
                catch (Exception ex)
                {
                    LogCandumpCleanupFailed(_logger, ex);
                }
            }
        }
    }

    public async Task SendFrameAsync(uint canId, byte[] data, CancellationToken token
)
    {
        ArgumentNullException.ThrowIfNull(data);
        ObjectDisposedException.ThrowIf(_disposed, this);

        await EnsureWriteConnectionAsync(token);

        if (_writeSshClient == null || !_writeSshClient.IsConnected)
            throw new InvalidOperationException("Write SSH client is not connected");

        try
        {
            // Format: cansend can0 180#D21DFA0A0601E0
            var dataHex = Convert.ToHexString(data);
            var cansendCommand = $"cansend {_canInterface} {canId:X}#{dataHex}";

            LogSendingCanFrame(_logger, canId, data.Length, cansendCommand);

            var command = _writeSshClient.CreateCommand(cansendCommand);
            _ = await Task.Run(() => command.Execute(), token);

            var exitStatus = command.ExitStatus ?? -1;
            if (exitStatus != 0)
            {
                var error = command.Error;
                LogCansendFailed(_logger, exitStatus, error);
                throw new InvalidOperationException($"cansend failed with exit code {exitStatus}: {error}");
            }

            LogCanFrameSentSuccessfully(_logger);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            LogFailedToSendCanFrame(_logger, ex, canId);
            throw;
        }
    }

    private CanFrame? ParseCandumpLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;

        var match = CandumpRegex.Match(line);
        if (!match.Success)
        {
            LogFailedToParseCandumpLine(_logger, line);
            return null;
        }

        try
        {
            var canIdHex = match.Groups[1].Value;
            var dataHex = match.Groups[3].Value.Replace(" ", "");

            var canId = Convert.ToUInt32(canIdHex, 16);
            var data = Convert.FromHexString(dataHex);

            return new CanFrame(canId, data);
        }
        catch (Exception ex)
        {
            LogErrorParsingCandumpLine(_logger, ex, line);
            return null;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        LogDisposingSshCanBus(_logger);

        // Dispose both SSH clients - this will close all associated streams
        _readSshClient?.Dispose();
        _writeSshClient?.Dispose();
        _readConnectionLock.Dispose();
        _writeConnectionLock.Dispose();

        GC.SuppressFinalize(this);
    }

    [LoggerMessage(EventId = 2101, Level = LogLevel.Information, Message = "Connecting to SSH host {Host}:{Port} as user {Username} ({ConnectionType} connection)")]
    private static partial void LogConnectingToSshHost(ILogger logger, string host, int port, string username, string connectionType);

    [LoggerMessage(EventId = 2102, Level = LogLevel.Information, Message = "SSH connected to {Host} ({ConnectionType} connection)")]
    private static partial void LogSshConnected(ILogger logger, string host, string connectionType);

    [LoggerMessage(EventId = 2103, Level = LogLevel.Error, Message = "SSH connection failed to {Host} ({ConnectionType} connection)")]
    private static partial void LogSshConnectionFailed(ILogger logger, Exception exception, string host, string connectionType);

    [LoggerMessage(EventId = 2104, Level = LogLevel.Information, Message = "SSH CAN bus initialized on {Host} using interface {Interface} ({ConnectionType} connection)")]
    private static partial void LogSshCanBusInitialized(ILogger logger, string host, string @interface, string connectionType);

    [LoggerMessage(EventId = 2105, Level = LogLevel.Error, Message = "candump command not found on remote system")]
    private static partial void LogCandumpNotFound(ILogger logger);

    [LoggerMessage(EventId = 2128, Level = LogLevel.Error, Message = "cansend command not found on remote system")]
    private static partial void LogCansendNotFound(ILogger logger);

    [LoggerMessage(EventId = 2106, Level = LogLevel.Information, Message = "Starting candump on interface {Interface}")]
    private static partial void LogStartingCandump(ILogger logger, string @interface);

    [LoggerMessage(EventId = 2107, Level = LogLevel.Debug, Message = "candump started successfully")]
    private static partial void LogCandumpStarted(ILogger logger);

    [LoggerMessage(EventId = 2119, Level = LogLevel.Debug, Message = "Executing candump command: {Command}")]
    private static partial void LogCandumpCommandExecuting(ILogger logger, string command);

    [LoggerMessage(EventId = 2120, Level = LogLevel.Debug, Message = "Raw shell output: {Output}")]
    private static partial void LogRawShellOutput(ILogger logger, string output);

    [LoggerMessage(EventId = 2121, Level = LogLevel.Debug, Message = "candump output stream ready, parsing CAN frames")]
    private static partial void LogCandumpOutputStarted(ILogger logger);

    [LoggerMessage(EventId = 2122, Level = LogLevel.Debug, Message = "Raw candump data: {Data}")]
    private static partial void LogRawCandumpData(ILogger logger, string data);

    [LoggerMessage(EventId = 2123, Level = LogLevel.Debug, Message = "Processing candump line: {Line}")]
    private static partial void LogProcessingCandumpLine(ILogger logger, string line);

    [LoggerMessage(EventId = 2124, Level = LogLevel.Debug, Message = "Skipping command echo: {Line}")]
    private static partial void LogSkippingCommandEcho(ILogger logger, string line);

    [LoggerMessage(EventId = 2118, Level = LogLevel.Warning, Message = "Timeout waiting for candump to start - proceeding anyway")]
    private static partial void LogCandumpStartupTimeout(ILogger logger);

    [LoggerMessage(EventId = 2108, Level = LogLevel.Debug, Message = "candump stopped")]
    private static partial void LogCandumpStopped(ILogger logger);

    [LoggerMessage(EventId = 2109, Level = LogLevel.Debug, Message = "Received CAN frame: ID=0x{CanId:X}, DataLength={DataLength}")]
    private static partial void LogReceivedCanFrame(ILogger logger, uint canId, int dataLength);

    [LoggerMessage(EventId = 2110, Level = LogLevel.Debug, Message = "Sending CAN frame: ID=0x{CanId:X}, DataLength={DataLength}, Command={Command}")]
    private static partial void LogSendingCanFrame(ILogger logger, uint canId, int dataLength, string command);

    [LoggerMessage(EventId = 2111, Level = LogLevel.Debug, Message = "CAN frame sent successfully via cansend")]
    private static partial void LogCanFrameSentSuccessfully(ILogger logger);

    [LoggerMessage(EventId = 2112, Level = LogLevel.Error, Message = "Failed to send CAN frame with ID 0x{CanId:X}")]
    private static partial void LogFailedToSendCanFrame(ILogger logger, Exception exception, uint canId);

    [LoggerMessage(EventId = 2113, Level = LogLevel.Error, Message = "Error reading CAN frame from candump")]
    private static partial void LogErrorReadingCanFrame(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 2114, Level = LogLevel.Warning, Message = "Failed to parse candump line: {Line}")]
    private static partial void LogFailedToParseCandumpLine(ILogger logger, string line);

    [LoggerMessage(EventId = 2115, Level = LogLevel.Error, Message = "Error parsing candump line: {Line}")]
    private static partial void LogErrorParsingCandumpLine(ILogger logger, Exception exception, string line);

    [LoggerMessage(EventId = 2116, Level = LogLevel.Error, Message = "cansend command failed with exit code {ExitCode}: {Error}")]
    private static partial void LogCansendFailed(ILogger logger, int exitCode, string error);

    [LoggerMessage(EventId = 2125, Level = LogLevel.Debug, Message = "Disposing SSH CAN bus connection")]
    private static partial void LogDisposingSshCanBus(ILogger logger);

    [LoggerMessage(EventId = 2126, Level = LogLevel.Debug, Message = "Stopping candump process")]
    private static partial void LogStoppingCandump(ILogger logger);

    [LoggerMessage(EventId = 2127, Level = LogLevel.Warning, Message = "Failed to cleanly stop candump process")]
    private static partial void LogCandumpCleanupFailed(ILogger logger, Exception exception);

    [GeneratedRegex(@"^\s*\w+\s+([0-9A-Fa-f]+)\s+\[(\d+)\]\s+([0-9A-Fa-f\s]+)", RegexOptions.Compiled)]
    private static partial Regex CanDumpRegex();
}
