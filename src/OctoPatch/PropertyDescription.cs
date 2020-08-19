namespace OctoPatch
{
    /// <summary>
    /// Description for a single property of a custom message
    /// </summary>
    public sealed class PropertyDescription
    {
        /// <summary>
        /// Property Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description for this property
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Name of the type
        /// </summary>
        public string Type { get; set; }
    }
}
