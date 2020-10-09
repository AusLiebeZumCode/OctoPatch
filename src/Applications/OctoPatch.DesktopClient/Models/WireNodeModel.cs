using System;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class WireNodeModel : NodeModel
    {
        public WireNodeModel(Guid wireId, string name) : base(wireId)
        {
            Name = name;
        }
    }
}
