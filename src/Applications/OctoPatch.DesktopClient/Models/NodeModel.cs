using System;
using System.Collections.Generic;
using System.Text;
using OctoPatch.Core;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class NodeModel
    {
        public NodeInstance Instance { get; set; }

        public NodeState State { get; set; }
    }
}
