namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a string based message
    /// </summary>
    public sealed class StringContentType : ContentType
    {
        /// <summary>
        /// Gets the optional maximum length for this string
        /// </summary>
        public int? MaximumLength { get; set; }
    }
}
