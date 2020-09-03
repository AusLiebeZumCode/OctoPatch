using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class OutputNodeModel : NodeModel
    {
        public string TypeKey { get; }

        public OutputNodeModel(ConnectorDescription description) : base(description)
        {
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
