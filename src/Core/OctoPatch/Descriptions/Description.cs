namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Basic description class for all kind of descriptions
    /// </summary>
    public abstract class Description
    {
        /// <summary>
        /// Gets or sets the (localized) display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the (localized) description
        /// </summary>
        public string DisplayDescription { get; set; }

        protected Description(string displayName, string displayDescription)
        {
            DisplayName = displayName;
            DisplayDescription = displayDescription;
        }
    }
}
