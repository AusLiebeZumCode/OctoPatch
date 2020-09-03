using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class CommonNodeModel : NodeModel
    {
        public string Key { get; }

        public CommonNodeModel(CommonNodeDescription description) : base(description)
        {
            Key = description.Key;
        }
    }
}
