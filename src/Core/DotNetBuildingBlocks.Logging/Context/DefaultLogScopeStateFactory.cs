namespace DotNetBuildingBlocks.Logging.Context
{
    /// <summary>
    /// Default implementation of <see cref="ILogScopeStateFactory"/>.
    /// </summary>
    public sealed class DefaultLogScopeStateFactory : ILogScopeStateFactory
    {
        /// <inheritdoc />
        public LogScopeState Create(IEnumerable<KeyValuePair<string, object?>> properties)
        {
            ArgumentNullException.ThrowIfNull(properties);
            return new LogScopeState(properties);
        }
    }
}
