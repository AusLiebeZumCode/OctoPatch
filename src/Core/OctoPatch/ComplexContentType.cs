namespace OctoPatch
{
    /// <summary>
    /// Represents a message description based on a complex type
    /// </summary>
    public sealed class ComplexContentType : ContentType
    {
        /// <summary>
        /// Gets or sets the name of the complex type
        /// </summary>
        public string Type { get; set; }

        public static ComplexContentType Create<T>()
        {
            return new ComplexContentType()
            {
                Type = typeof(T).FullName
            };
        }
    }
}
