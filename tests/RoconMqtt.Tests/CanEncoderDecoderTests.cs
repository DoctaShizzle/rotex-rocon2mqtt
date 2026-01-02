using Microsoft.Extensions.Logging;
using Moq;
using RoconMqtt.Can;
using RoconMqtt.Can.Models;
using Xunit;

namespace RoconMqtt.Tests;

public class CanEncoderDecoderTests
{
    private readonly CanEncoder _encoder;
    private readonly CanDecoder _decoder;
    private readonly Mock<ICanParameterRegistry> _registryMock;
    private readonly CommunicationCommand _testCommand;

    public CanEncoderDecoderTests()
    {
        var encoderLoggerMock = new Mock<ILogger<CanEncoder>>();
        var decoderLoggerMock = new Mock<ILogger<CanDecoder>>();
        _registryMock = new Mock<ICanParameterRegistry>();
        
        // Setup test parameters in mock registry
        var testParameters = new Dictionary<InfoNumber, ParameterDefinition>
        {
            { new InfoNumber(0x12, 0x34), new ParameterDefinition("IntParam", new InfoNumber(0x12, 0x34), ParameterType.Int, BigEndian: false) },
            { new InfoNumber(0x56, 0x78), new ParameterDefinition("IntParamBE", new InfoNumber(0x56, 0x78), ParameterType.Int, BigEndian: true) },
            { new InfoNumber(0xAA, 0xBB), new ParameterDefinition("FloatParam", new InfoNumber(0xAA, 0xBB), ParameterType.Float, Factor: 1.0) },
            { new InfoNumber(0x11, 0x22), new ParameterDefinition("BoolParam", new InfoNumber(0x11, 0x22), ParameterType.Bool) },
            { new InfoNumber(0x33, 0x44), new ParameterDefinition("BoolParamFalse", new InfoNumber(0x33, 0x44), ParameterType.Bool) },
            { new InfoNumber(0x55, 0x66), new ParameterDefinition("TimeRangeParam", new InfoNumber(0x55, 0x66), ParameterType.TimeRange) },
            { new InfoNumber(0x77, 0x88), new ParameterDefinition("TimeRangeFullDay", new InfoNumber(0x77, 0x88), ParameterType.TimeRange) },
            { new InfoNumber(0x99, 0xAA), new ParameterDefinition("TimeRangeQuarters", new InfoNumber(0x99, 0xAA), ParameterType.TimeRange) },
        };

        _registryMock.Setup(r => r.Parameters).Returns(testParameters);
        
        _encoder = new CanEncoder(encoderLoggerMock.Object, _registryMock.Object);
        _decoder = new CanDecoder(decoderLoggerMock.Object, _registryMock.Object);
        
        // Use a test command with standard header
        _testCommand = new CommunicationCommand
        {
            CanId = 0x180,
            Bytes = [0xD2, 0x1D, 0xFA]
        };
    }

    [Fact]
    public void RoundTrip_Int_ShouldEncodeAndDecodeCorrectly()
    {
        // Arrange
        var info = new InfoNumber(0x12, 0x34);
        int originalValue = 1234;

        // Act
        var encoded = _encoder.Encode(_testCommand, info, originalValue);
        var decoded = _decoder.Decode(encoded, _testCommand);

        // Assert
        Assert.NotNull(decoded);
        Assert.Equal("IntParam", decoded.Name);
        Assert.Equal(originalValue, (int)decoded.Value);
    }

    [Fact]
    public void RoundTrip_Int_BigEndian_ShouldEncodeAndDecodeCorrectly()
    {
        // Arrange
        var info = new InfoNumber(0x56, 0x78);
        int originalValue = 5678;

        // Act
        var encoded = _encoder.Encode(_testCommand, info, originalValue);
        var decoded = _decoder.Decode(encoded, _testCommand);

        // Assert
        Assert.NotNull(decoded);
        Assert.Equal("IntParamBE", decoded.Name);
        Assert.Equal(originalValue, (int)decoded.Value);
    }

    [Fact]
    public void RoundTrip_Double_ShouldEncodeAndDecodeCorrectly()
    {
        // Arrange
        var info = new InfoNumber(0xAA, 0xBB);
        double originalValue = 42.5;

        // Act
        var encoded = _encoder.Encode(_testCommand, info, originalValue);
        var decoded = _decoder.Decode(encoded, _testCommand);

        // Assert
        Assert.NotNull(decoded);
        Assert.Equal("FloatParam", decoded.Name);
        Assert.Equal(42.0, (double)decoded.Value);
    }

    [Fact]
    public void RoundTrip_Bool_True_ShouldEncodeAndDecodeCorrectly()
    {
        // Arrange
        var info = new InfoNumber(0x11, 0x22);
        bool originalValue = true;

        // Act
        var encoded = _encoder.Encode(_testCommand, info, originalValue);
        var decoded = _decoder.Decode(encoded, _testCommand);

        // Assert
        Assert.NotNull(decoded);
        Assert.Equal("BoolParam", decoded.Name);
        Assert.True((bool)decoded.Value);
    }

    [Fact]
    public void RoundTrip_Bool_False_ShouldEncodeAndDecodeCorrectly()
    {
        // Arrange
        var info = new InfoNumber(0x33, 0x44);
        bool originalValue = false;

        // Act
        var encoded = _encoder.Encode(_testCommand, info, originalValue);
        var decoded = _decoder.Decode(encoded, _testCommand);

        // Assert
        Assert.NotNull(decoded);
        Assert.Equal("BoolParamFalse", decoded.Name);
        Assert.False((bool)decoded.Value);
    }

    [Fact]
    public void RoundTrip_TimeRange_ShouldEncodeAndDecodeCorrectly()
    {
        // Arrange
        var info = new InfoNumber(0x55, 0x66);
        string originalValue = "08:30-16:45";

        // Act
        var encoded = _encoder.Encode(_testCommand, info, originalValue);
        var decoded = _decoder.Decode(encoded, _testCommand);

        // Assert
        Assert.NotNull(decoded);
        Assert.Equal("TimeRangeParam", decoded.Name);
        Assert.Equal(originalValue, (string)decoded.Value);
    }

    [Fact]
    public void RoundTrip_TimeRange_Midnight_ShouldEncodeAndDecodeCorrectly()
    {
        // Arrange
        var info = new InfoNumber(0x77, 0x88);
        string originalValue = "00:00-23:45";

        // Act
        var encoded = _encoder.Encode(_testCommand, info, originalValue);
        var decoded = _decoder.Decode(encoded, _testCommand);

        // Assert
        Assert.NotNull(decoded);
        Assert.Equal("TimeRangeFullDay", decoded.Name);
        Assert.Equal(originalValue, (string)decoded.Value);
    }

    [Fact]
    public void RoundTrip_TimeRange_QuarterHours_ShouldEncodeAndDecodeCorrectly()
    {
        // Arrange - TimeRange should handle quarter-hour boundaries
        var info = new InfoNumber(0x99, 0xAA);
        string originalValue = "12:00-15:45"; // Exactly 3 and 63 quarter-hours

        // Act
        var encoded = _encoder.Encode(_testCommand, info, originalValue);
        var decoded = _decoder.Decode(encoded, _testCommand);

        // Assert
        Assert.NotNull(decoded);
        Assert.Equal("TimeRangeQuarters", decoded.Name);
        Assert.Equal(originalValue, (string)decoded.Value);
    }

    [Fact]
    public void Encoder_TimeRange_FrameSize_ShouldBe7Bytes()
    {
        // Arrange
        var info = new InfoNumber(0x11, 0x22);
        string timeRange = "09:00-17:00";

        // Act
        var encoded = _encoder.Encode(_testCommand, info, timeRange);

        // Assert
        Assert.Equal(7, encoded.Length);
    }

    [Fact]
    public void Encoder_Int_FrameSize_ShouldBe7Bytes()
    {
        // Arrange
        var info = new InfoNumber(0x11, 0x22);
        int value = 100;

        // Act
        var encoded = _encoder.Encode(_testCommand, info, value);

        // Assert
        Assert.Equal(7, encoded.Length);
    }

    [Fact]
    public void Encoder_Header_ShouldAlwaysBeCorrect()
    {
        // Arrange
        var info = new InfoNumber(0x12, 0x34);
        int value = 42;

        // Act
        var encoded = _encoder.Encode(_testCommand, info, value);

        // Assert
        Assert.Equal(0xD2, encoded[0]);
        Assert.Equal(0x1D, encoded[1]);
        Assert.Equal(0xFA, encoded[2]);
    }

    [Fact]
    public void Encoder_InfoNumber_ShouldBePreserved()
    {
        // Arrange
        var info = new InfoNumber(0xAB, 0xCD);
        int value = 999;

        // Act
        var encoded = _encoder.Encode(_testCommand, info, value);

        // Assert
        Assert.Equal(0xAB, encoded[3]);
        Assert.Equal(0xCD, encoded[4]);
    }
}

