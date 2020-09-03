using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class SplitterNodeModel : NodeModel
    {
        public string Key { get; }

        public SplitterNodeModel(SplitterNodeDescription description) : base(description)
        {
            Key = description.Key;
        }
    }
}
