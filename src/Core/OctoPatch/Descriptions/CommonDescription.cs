namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Basic description class for all kind of descriptions
    /// </summary>
    public abstract class CommonDescription
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// optional Description
        /// </summary>
        public string Description { get; set; }

        protected CommonDescription(string name, string description = null)
        {
            Name = name;
            Description = description;
        }
    }
}
