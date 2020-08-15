namespace OctoPatch
{
    /// <summary>
    /// List of all possible states of a single node
    /// </summary>
    public enum NodeState
    {
        /// <summary>
        /// Block is not initialized yet and not ready to run.
        /// </summary>
        NotReady,

        /// <summary>
        /// Block is initializing at the moment.
        /// Transition between NotReady and Stopped
        /// </summary>
        Initializing,

        /// <summary>
        /// Block is ready to run but stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// Block is starting up
        /// Transition between Stopped and Running
        /// </summary>
        Starting,

        /// <summary>
        /// Block is running
        /// </summary>
        Running,

        /// <summary>
        /// Block is shutting down
        /// Transition between Running and Stopped
        /// </summary>
        Stopping,

        /// <summary>
        /// Block is disposing
        /// Transition between Stopped and NotReady
        /// </summary>
        Disposing,

        /// <summary>
        /// Fail state. Something went wrong somewhere and node needs to be initialized again
        /// </summary>
        Failed
    }
}
