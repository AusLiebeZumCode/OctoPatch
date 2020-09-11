using System;
using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class CollectorNodeModel : NodeModel
    {
        public Guid Id { get; }

        public CollectorNodeModel(Guid id, CollectorNodeDescription description) : base(id, description)
        {
            Id = id;
        }
    }
}
