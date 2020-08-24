namespace OctoPatch
{
    /// <summary>
    /// Represents a integer based message
    /// </summary>
    public sealed class IntegerMessageDescription : MessageDescription
    {
        /// <summary>
        /// Optional lowest value for this message
        /// </summary>
        public int? MinimumValue { get; set; }

        /// <summary>
        /// Optional highest value for this message
        /// </summary>
        public int? MaximumValue { get; set; }
    }
}
