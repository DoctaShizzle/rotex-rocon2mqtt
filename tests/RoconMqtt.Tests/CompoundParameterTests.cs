using RoconMqtt.Mqtt.Compound;
using Xunit;

namespace RoconMqtt.Tests;

public class CompoundParameterTests
{
    [Fact]
    public void TimestampCompoundParameter_ShouldCombineHourAndMinute()
    {
        // Arrange
        var timestamp = new TimestampCompoundParameter();
        var now = DateTime.Now;
        
        // Act
        var hourSet = timestamp.TrySetComponent("Hour", 14);
        var minuteSet = timestamp.TrySetComponent("Minute", 30);
        var value = timestamp.GetValue();
        
        // Assert
        Assert.False(hourSet); // Should return false because not all components are set yet
        Assert.True(minuteSet); // Should return true because all components are now set
        Assert.NotNull(value);
        
        var iso8601String = Assert.IsType<string>(value);
        var dateTime = DateTime.Parse(iso8601String);
        Assert.Equal(14, dateTime.Hour);
        Assert.Equal(30, dateTime.Minute);
        Assert.Equal(now.Year, dateTime.Year);
        Assert.Equal(now.Month, dateTime.Month);
        Assert.Equal(now.Day, dateTime.Day);
    }
    
    [Fact]
    public void TimestampCompoundParameter_ShouldHandleDoubleValues()
    {
        // Arrange
        var timestamp = new TimestampCompoundParameter();
        
        // Act
        timestamp.TrySetComponent("Hour", 9.0);
        timestamp.TrySetComponent("Minute", 45.0);
        var value = timestamp.GetValue();
        
        // Assert
        Assert.NotNull(value);
        var iso8601String = Assert.IsType<string>(value);
        var dateTime = DateTime.Parse(iso8601String);
        Assert.Equal(9, dateTime.Hour);
        Assert.Equal(45, dateTime.Minute);
    }
    
    [Fact]
    public void TimestampCompoundParameter_ShouldFormatWithoutTimezoneOffset()
    {
        // Arrange
        var timestamp = new TimestampCompoundParameter();
        var now = DateTime.Now;
        
        // Act
        timestamp.TrySetComponent("Hour", 14);
        timestamp.TrySetComponent("Minute", 30);
        var value = timestamp.GetValue();
        
        // Assert
        Assert.NotNull(value);
        var iso8601String = Assert.IsType<string>(value);
        
        // Verify format is yyyy-MM-ddTHH:mm:ss without timezone offset
        Assert.Matches(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$", iso8601String);
        Assert.DoesNotContain("+", iso8601String);
        Assert.DoesNotContain("Z", iso8601String);
        
        // Verify the expected format
        var expectedFormat = $"{now.Year:D4}-{now.Month:D2}-{now.Day:D2}T14:30:00";
        Assert.Equal(expectedFormat, iso8601String);
    }
    
    [Fact]
    public void TimestampCompoundParameter_ShouldResetCorrectly()
    {
        // Arrange
        var timestamp = new TimestampCompoundParameter();
        timestamp.TrySetComponent("Hour", 10);
        timestamp.TrySetComponent("Minute", 20);
        
        // Act
        timestamp.Reset();
        var value = timestamp.GetValue();
        
        // Assert
        Assert.Null(value);
    }
    
    [Fact]
    public void TimestampCompoundParameter_ShouldReturnNullWhenIncomplete()
    {
        // Arrange
        var timestamp = new TimestampCompoundParameter();
        timestamp.TrySetComponent("Hour", 10);
        
        // Act
        var value = timestamp.GetValue();
        
        // Assert
        Assert.Null(value);
    }
    
    [Fact]
    public void CompoundParameterFactory_ShouldCreateTimestampParameter()
    {
        // Act
        var compound = CompoundParameterFactory.Create("Timestamp");
        
        // Assert
        Assert.NotNull(compound);
        Assert.IsType<TimestampCompoundParameter>(compound);
        Assert.Equal("Timestamp", compound.Name);
        Assert.Contains("Hour", compound.ComponentParameters);
        Assert.Contains("Minute", compound.ComponentParameters);
    }
    
    [Fact]
    public void CompoundParameterFactory_ShouldThrowForUnknownParameter()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            CompoundParameterFactory.Create("InvalidParameter"));
        Assert.Contains("Unknown compound parameter", exception.Message);
    }
}
