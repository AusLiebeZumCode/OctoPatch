using System;
using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class CommonNodeModel : NodeModel
    {
        public Guid Id { get; }

        public string Key { get; }

        public CommonNodeModel(Guid id, CommonNodeDescription description) : base(id, description)
        {
            Id = id;
            Key = description.Key;
        }
    }
}
