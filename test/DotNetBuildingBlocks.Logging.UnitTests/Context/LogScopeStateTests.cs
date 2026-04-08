using DotNetBuildingBlocks.Logging.Context;
using FluentAssertions;
using Xunit;

namespace DotNetBuildingBlocks.Logging.UnitTests.Context
{
    /// <summary>
    /// LogScopeStateTests
    /// </summary>
    public sealed class LogScopeStateTests
    {
        /// <summary>
        /// Constructor_Should_Filter_Blank_Property_Names
        /// </summary>
        [Fact]
        public void Constructor_Should_Filter_Blank_Property_Names()
        {
            KeyValuePair<string, object?>[] properties =
            [
                new KeyValuePair<string, object?>("CorrelationId", "corr-1"),
                new KeyValuePair<string, object?>("", "ignored"),
                new KeyValuePair<string, object?>("OperationName", "ImportCustomers")
            ];

            LogScopeState state = new(properties);

            _ = state.Count.Should().Be(2);
            _ = state.Select(x => x.Key).Should().ContainInOrder("CorrelationId", "OperationName");
        }
    }
}
