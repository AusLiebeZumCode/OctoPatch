using System;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class InputNodeModel : NodeModel
    {
        public Guid ParentId { get; }

        public string TypeKey { get; }

        public InputNodeModel(Guid parentId, ConnectorDescription description) : base(description)
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
