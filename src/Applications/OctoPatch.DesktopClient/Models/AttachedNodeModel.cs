using System;
using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class AttachedNodeModel : NodeModel
    {
        public Guid Id { get; }

        public AttachedNodeModel(Guid id, AttachedNodeDescription description) : base(id, description)
        {
            Id = id;
        }
    }
}
