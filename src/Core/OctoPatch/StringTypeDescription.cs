namespace OctoPatch
{
    /// <summary>
    /// Represents a string based message
    /// </summary>
    public sealed class StringTypeDescription : ContentType
    {
        /// <summary>
        /// Gets the optional maximum length for this string
        /// </summary>
        public int? MaximumLength { get; set; }
    }
}
