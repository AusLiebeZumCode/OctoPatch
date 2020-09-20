using System;
using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class SplitterNodeModel : NodeModel
    {
        public Guid Id { get; }

        public SplitterNodeModel(Guid id, SplitterNodeDescription description) : base(id, description)
        {
            Id = id;
        }
    }
}
