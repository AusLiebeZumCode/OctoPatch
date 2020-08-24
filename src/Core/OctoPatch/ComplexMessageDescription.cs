namespace OctoPatch
{
    /// <summary>
    /// Represents a message description based on a complex type
    /// </summary>
    public sealed class ComplexMessageDescription : MessageDescription
    {
        /// <summary>
        /// Gets or sets the name of the complex type
        /// </summary>
        public string Type { get; set; }
    }
}
