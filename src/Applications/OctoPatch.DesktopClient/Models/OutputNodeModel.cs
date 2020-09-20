using System;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class OutputNodeModel : NodeModel
    {
        public Guid ParentId { get; }

        public string TypeKey { get; }

        public OutputNodeModel(Guid parentId, ConnectorDescription description) : base(description)
        {
            ParentId = parentId;
            if (description.ContentType is ComplexContentType complex)
            {
                TypeKey = complex.Key;
            }
            else
            {
                TypeKey = description.ContentType.Type;
            }
        }
    }
}
