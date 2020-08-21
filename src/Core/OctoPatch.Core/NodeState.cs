namespace OctoPatch.Core
{
    /// <summary>
    /// List of all possible states of a single node
    /// </summary>
    public enum NodeState
    {
        /// <summary>
        /// Block is not initialized yet and not ready to run.
        /// </summary>
        Uninitialized,

        /// <summary>
        /// Block is initializing at the moment.
        /// Transition between <see cref="Uninitialized"/> and <see cref="Stopped"/>
        /// In error case it turns to <see cref="InitializationFailed"/>
        /// </summary>
        Initializing,

        /// <summary>
        /// Transition state for resetting the block.
        /// In case of missing configuration this is the transition between <see cref="InitializationFailed"/> and <see cref="Uninitialized"/> and can't fail.
        /// In case of an existing configuration this is a transition between <see cref="Failed"/> and <see cref="Stopped"/> and can't fail.
        /// </summary>
        Resetting,

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
        /// Transition between <see cref="Running"/> and <see cref="Stopped"/> and can result in <see cref="Failed"/> in case of errors.
        /// </summary>
        Stopping,

        /// <summary>
        /// Block is deinitializing
        /// Transition between <see cref="Stopped"/> and <see cref="Uninitialized"/> and can't fail.
        /// </summary>
        Deinitializing,

        /// <summary>
        /// Fail state during <see cref="Initializing"/>
        /// </summary>
        InitializationFailed,

        /// <summary>
        /// Fail state. Something went wrong somewhere and node needs to be initialized again
        /// </summary>
        Failed
    }
}
