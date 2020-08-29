namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Describes the type of content within a connector or complex type
    /// </summary>
    public abstract class ContentType
    {
        public string Type => GetType().Name;
    }
}
