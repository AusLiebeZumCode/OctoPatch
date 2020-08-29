using OctoPatch.Setup;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class NodeModel
    {
        public NodeSetup Setup { get; set; }

        public NodeState State { get; set; }
    }
}
