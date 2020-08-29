namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a integer based message
    /// </summary>
    public sealed class FloatContentType : ContentType
    {
        /// <summary>
        /// Optional lowest value for this message
        /// </summary>
        public float? MinimumValue { get; set; }

        /// <summary>
        /// Optional highest value for this message
        /// </summary>
        public float? MaximumValue { get; set; }
    }
}
