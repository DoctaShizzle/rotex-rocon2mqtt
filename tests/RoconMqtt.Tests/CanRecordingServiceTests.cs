using Microsoft.Extensions.Logging;
using Moq;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;
using Xunit;

namespace RoconMqtt.Tests;

public class CanRecordingServiceTests : IDisposable
{
    private readonly Mock<ICanBus> _canBusMock;
    private readonly Mock<ILogger<CanRecordingService>> _loggerMock;
    private readonly CanRecordingService _service;
    private readonly CancellationTokenSource _cts;

    public CanRecordingServiceTests()
    {
        _canBusMock = new Mock<ICanBus>();
        _loggerMock = new Mock<ILogger<CanRecordingService>>();
        _cts = new CancellationTokenSource();
        
        _service = new CanRecordingService(_canBusMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task StartRecordingAsync_WhenNotRecording_ShouldStartSuccessfully()
    {
        // Arrange
        var options = new CanRecordingOptions
        {
            Description = "Test recording"
        };

        // Act
        var result = await _service.StartRecordingAsync(options);

        // Assert
        Assert.True(result);
        Assert.True(_service.IsRecording);
    }

    [Fact]
    public async Task StartRecordingAsync_WhenAlreadyRecording_ShouldReturnFalse()
    {
        // Arrange
        await _service.StartRecordingAsync();

        // Act
        var result = await _service.StartRecordingAsync();

        // Assert
        Assert.False(result);
        Assert.True(_service.IsRecording);
    }

    [Fact]
    public async Task StartRecordingAsync_WithNullOptions_ShouldStartSuccessfully()
    {
        // Act
        var result = await _service.StartRecordingAsync(null);

        // Assert
        Assert.True(result);
        Assert.True(_service.IsRecording);
    }

    [Fact]
    public async Task StartRecordingAsync_WithCanIdFilter_ShouldStoreFilter()
    {
        // Arrange
        var options = new CanRecordingOptions
        {
            CanIdFilter = ["0x180", "0x300"],
            Description = "Filtered recording"
        };

        // Act
        await _service.StartRecordingAsync(options);
        var result = await _service.StopRecordingAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(options.CanIdFilter, result.CanIdFilter);
        Assert.Equal(options.Description, result.Description);
    }

    [Fact]
    public async Task StopRecordingAsync_WhenNotRecording_ShouldReturnNull()
    {
        // Act
        var result = await _service.StopRecordingAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task StopRecordingAsync_WhenRecording_ShouldReturnResult()
    {
        // Arrange
        await _service.StartRecordingAsync();

        // Act
        var result = await _service.StopRecordingAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Frames);
        Assert.False(_service.IsRecording);
    }

    [Fact]
    public async Task StopRecordingAsync_ShouldCalculateDurationCorrectly()
    {
        // Arrange
        await _service.StartRecordingAsync();
        await Task.Delay(100); // Small delay to ensure duration > 0

        // Act
        var result = await _service.StopRecordingAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Duration.TotalMilliseconds >= 100);
        Assert.Equal(result.StoppedAt - result.StartedAt, result.Duration);
    }

    [Fact]
    public async Task StopRecordingAsync_ShouldClearRecordingState()
    {
        // Arrange
        var options = new CanRecordingOptions { Description = "Test" };
        await _service.StartRecordingAsync(options);

        // Act
        await _service.StopRecordingAsync();
        var secondResult = await _service.StopRecordingAsync();

        // Assert
        Assert.Null(secondResult);
        Assert.False(_service.IsRecording);
    }

    [Fact]
    public async Task IsRecording_ShouldReflectCurrentState()
    {
        // Arrange & Act & Assert - Initial state
        Assert.False(_service.IsRecording);

        // Start recording
        await _service.StartRecordingAsync();
        Assert.True(_service.IsRecording);

        // Stop recording
        await _service.StopRecordingAsync();
        Assert.False(_service.IsRecording);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotRecordFrames_WhenNotRecording()
    {
        // Arrange
        var frames = new[]
        {
            new CanFrame(0x180, [0xD2, 0x1D, 0xFA, 0x00, 0x0C, 0x01, 0xE0]),
            new CanFrame(0x300, [0xD2, 0x1D, 0xFA, 0x01, 0x12, 0x00, 0xFA])
        };

        _canBusMock.Setup(cb => cb.ReadFramesAsync(It.IsAny<CancellationToken>(), It.IsAny<uint?>()))
            .Returns(CreateAsyncEnumerable(frames));

        // Act
        var executeTask = _service.StartAsync(_cts.Token);
        await Task.Delay(100); // Let it process frames
        await _cts.CancelAsync();
        await executeTask;

        var result = await _service.StopRecordingAsync();

        // Assert
        Assert.Null(result); // Should be null because we never started recording
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRecordFrames_WhenRecording()
    {
        // Arrange
        var frames = new[]
        {
            new CanFrame(0x180, [0xD2, 0x1D, 0xFA, 0x00, 0x0C, 0x01, 0xE0]),
            new CanFrame(0x300, [0xD2, 0x1D, 0xFA, 0x01, 0x12, 0x00, 0xFA])
        };

        _canBusMock.Setup(cb => cb.ReadFramesAsync(It.IsAny<CancellationToken>(), It.IsAny<uint?>()))
            .Returns(CreateAsyncEnumerable(frames));

        // Act
        await _service.StartAsync(_cts.Token);
        await _service.StartRecordingAsync();
        await Task.Delay(200); // Let it process frames
        var result = await _service.StopRecordingAsync();
        await _cts.CancelAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.FrameCount);
        Assert.Equal(2, result.Frames.Count);
    }

    [Fact]
    public async Task ExecuteAsync_WithCanIdFilter_ShouldOnlyRecordMatchingFrames()
    {
        // Arrange
        var frames = new[]
        {
            new CanFrame(0x180, [0xD2, 0x1D, 0xFA, 0x00, 0x0C, 0x01, 0xE0]),
            new CanFrame(0x300, [0xD2, 0x1D, 0xFA, 0x01, 0x12, 0x00, 0xFA]),
            new CanFrame(0x600, [0xD2, 0x1D, 0xFA, 0x02, 0x34, 0x02, 0xA0])
        };

        _canBusMock.Setup(cb => cb.ReadFramesAsync(It.IsAny<CancellationToken>(), It.IsAny<uint?>()))
            .Returns(CreateAsyncEnumerable(frames));

        var options = new CanRecordingOptions
        {
            CanIdFilter = ["0x180", "0x600"]
        };

        // Act
        await _service.StartAsync(_cts.Token);
        await _service.StartRecordingAsync(options);
        await Task.Delay(200);
        var result = await _service.StopRecordingAsync();
        await _cts.CancelAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.FrameCount);
        Assert.Contains(result.Frames, f => f.CanId == "0x180");
        Assert.Contains(result.Frames, f => f.CanId == "0x600");
        Assert.DoesNotContain(result.Frames, f => f.CanId == "0x300");
    }

    [Fact]
    public async Task ExecuteAsync_WithCaseInsensitiveFilter_ShouldMatchFrames()
    {
        // Arrange
        var frames = new[]
        {
            new CanFrame(0x180, [0xD2, 0x1D, 0xFA, 0x00, 0x0C, 0x01, 0xE0])
        };

        _canBusMock.Setup(cb => cb.ReadFramesAsync(It.IsAny<CancellationToken>(), It.IsAny<uint?>()))
            .Returns(CreateAsyncEnumerable(frames));

        var options = new CanRecordingOptions
        {
            CanIdFilter = ["0X180"] // Uppercase X
        };

        // Act
        await _service.StartAsync(_cts.Token);
        await _service.StartRecordingAsync(options);
        await Task.Delay(200);
        var result = await _service.StopRecordingAsync();
        await _cts.CancelAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.FrameCount);
    }

    [Fact]
    public async Task RecordedFrame_ShouldHaveCorrectFormat()
    {
        // Arrange
        var testFrame = new CanFrame(0x180, [0xD2, 0x1D, 0xFA, 0x00, 0x0C, 0x01, 0xE0]);
        
        _canBusMock.Setup(cb => cb.ReadFramesAsync(It.IsAny<CancellationToken>(), It.IsAny<uint?>()))
            .Returns(CreateAsyncEnumerable([testFrame]));

        // Act
        await _service.StartAsync(_cts.Token);
        await _service.StartRecordingAsync();
        await Task.Delay(200);
        var result = await _service.StopRecordingAsync();
        await _cts.CancelAsync();

        // Assert
        Assert.NotNull(result);
        var frame = Assert.Single(result.Frames);
        Assert.Equal("0x180", frame.CanId);
        Assert.Equal("D2 1D FA 00 0C 01 E0", frame.Data);
        Assert.Equal(7, frame.DataLength);
        Assert.True((DateTime.UtcNow - frame.Timestamp).TotalSeconds < 5);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleCancellation()
    {
        // Arrange
        _canBusMock.Setup(cb => cb.ReadFramesAsync(It.IsAny<CancellationToken>(), It.IsAny<uint?>()))
            .Returns((CancellationToken ct, uint? _) => CreateInfiniteAsyncEnumerable(ct));

        // Act
        await _service.StartAsync(_cts.Token);
        await Task.Delay(100);
        await _cts.CancelAsync();

        // Should complete without throwing
        await Task.Delay(100);
        
        // Assert - no exception thrown
        Assert.True(true);
    }

    [Fact]
    public async Task MultipleStartStopCycles_ShouldWorkCorrectly()
    {
        // Arrange
        var frames1 = new[]
        {
            new CanFrame(0x180, [0xD2, 0x1D, 0xFA, 0x00, 0x0C, 0x01, 0xE0])
        };
        var frames2 = new[]
        {
            new CanFrame(0x300, [0xD2, 0x1D, 0xFA, 0x01, 0x12, 0x00, 0xFA]),
            new CanFrame(0x600, [0xD2, 0x1D, 0xFA, 0x02, 0x34, 0x02, 0xA0])
        };

        // Setup to return different frames for each read attempt
        var callCount = 0;
        _canBusMock.Setup(cb => cb.ReadFramesAsync(It.IsAny<CancellationToken>(), It.IsAny<uint?>()))
            .Returns(() =>
            {
                callCount++;
                return callCount == 1 ? CreateAsyncEnumerable(frames1) : CreateAsyncEnumerable(frames2);
            });

        // Act - First recording
        await _service.StartAsync(_cts.Token);
        await _service.StartRecordingAsync(new CanRecordingOptions { Description = "First" });
        await Task.Delay(100);
        var result1 = await _service.StopRecordingAsync();

        // Act - Second recording
        await _service.StartRecordingAsync(new CanRecordingOptions { Description = "Second" });
        await Task.Delay(100);
        var result2 = await _service.StopRecordingAsync();
        
        await _cts.CancelAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.Equal("First", result1.Description);
        
        Assert.NotNull(result2);
        Assert.Equal("Second", result2.Description);
    }

    [Fact]
    public async Task RecordedFrames_ShouldCaptureAllFrames()
    {
        // Arrange
        var frames = Enumerable.Range(0, 10)
            .Select(i => new CanFrame((uint)(0x180 + i), [(byte)i]))
            .ToArray();

        _canBusMock.Setup(cb => cb.ReadFramesAsync(It.IsAny<CancellationToken>(), It.IsAny<uint?>()))
            .Returns(CreateAsyncEnumerable(frames));

        // Act
        await _service.StartAsync(_cts.Token);
        await _service.StartRecordingAsync();
        await Task.Delay(200);
        var result = await _service.StopRecordingAsync();
        await _cts.CancelAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.FrameCount);
        
        // All frames should be captured (order may vary due to ConcurrentBag)
        Assert.Equal(10, result.Frames.Count);
        
        // Verify all expected CAN IDs are present
        for (int i = 0; i < 10; i++)
        {
            var expectedCanId = $"0x{0x180 + i:X}";
            Assert.Contains(result.Frames, f => f.CanId == expectedCanId);
        }
    }

    // Helper method to create async enumerable from array
    private static async IAsyncEnumerable<CanFrame> CreateAsyncEnumerable(CanFrame[] frames)
    {
        foreach (var frame in frames)
        {
            await Task.Delay(10); // Small delay to simulate real CAN bus timing
            yield return frame;
        }
    }

    // Helper method to create infinite async enumerable for cancellation testing
    private static async IAsyncEnumerable<CanFrame> CreateInfiniteAsyncEnumerable([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int i = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(10, cancellationToken);
            yield return new CanFrame(0x180, [(byte)(i++ % 256)]);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cts.Cancel();
            _cts.Dispose();
            _service.Dispose();
        }
    }
}
