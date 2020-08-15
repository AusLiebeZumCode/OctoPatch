namespace OctoPatch
{
    /// <summary>
    /// List of possible engine states
    /// </summary>
    public enum EngineState
    {
        /// <summary>
        /// Engine is stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// Engine is starting
        /// Transition from <see cref="Stopped"/> to <see cref="Running"/>
        /// </summary>
        Starting,

        /// <summary>
        /// Engine is running
        /// </summary>
        Running,

        /// <summary>
        /// Engine is stopping
        /// Transition from <see cref="Running"/> to <see cref="Stopped"/>
        /// </summary>
        Stopping,
    }
}
