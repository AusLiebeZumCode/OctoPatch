using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class CollectorNodeModel : NodeModel
    {
        public string Key { get; }

        public CollectorNodeModel(CollectorNodeDescription description) : base(description)
        {
            Key = description.Key;
        }
    }
}
