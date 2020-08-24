namespace OctoPatch.Core
{
    /// <summary>
    /// Represents a single message
    /// </summary>
    public struct Message
    {
        /// <summary>
        /// Gets or sets the type of the serialized message
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the serialized content
        /// </summary>
        public string Content { get; set; }
    }
}
