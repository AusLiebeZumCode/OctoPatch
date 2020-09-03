using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class AttachedNodeModel : NodeModel
    {
        public string Key { get; }

        public AttachedNodeModel(AttachedNodeDescription description) : base(description)
        {
            Key = description.Key;
        }
    }
}
