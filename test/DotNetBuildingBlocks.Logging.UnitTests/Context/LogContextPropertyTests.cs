using DotNetBuildingBlocks.Logging.Context;
using FluentAssertions;
using Xunit;

namespace DotNetBuildingBlocks.Logging.UnitTests.Context
{
    /// <summary>
    /// LogContextPropertyTests
    /// </summary>
    public sealed class LogContextPropertyTests
    {
        /// <summary>
        /// Constructor_Should_Set_Properties
        /// </summary>
        [Fact]
        public void Constructor_Should_Set_Properties()
        {
            LogContextProperty property = new("CorrelationId", "corr-1");

            _ = property.Name.Should().Be("CorrelationId");
            _ = property.Value.Should().Be("corr-1");
        }

        /// <summary>
        /// Constructor_Should_Throw_When_Name_Is_Invalid
        /// </summary>
        [Fact]
        public void Constructor_Should_Throw_When_Name_Is_Invalid()
        {
            Func<LogContextProperty> action = () => new LogContextProperty(" ", "value");

            _ = action.Should().Throw<ArgumentException>();
        }
    }
}
