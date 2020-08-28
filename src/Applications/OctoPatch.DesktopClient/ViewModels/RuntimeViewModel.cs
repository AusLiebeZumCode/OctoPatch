using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Descriptions;
using OctoPatch.Server;
using OctoPatch.Setup;

namespace OctoPatch.DesktopClient.ViewModels
{
    public sealed class RuntimeViewModel
    {
        private readonly IRuntime _runtime;

        public ObservableCollection<NodeDescription> NodeDescriptions { get; }

        public ObservableCollection<NodeSetup> Nodes { get; }

        public ObservableCollection<WireSetup> Wires { get; }

        public RuntimeViewModel()
        {
            NodeDescriptions = new ObservableCollection<NodeDescription>();
            Nodes = new ObservableCollection<NodeSetup>();
            Wires = new ObservableCollection<WireSetup>();

            var repository = new Repository();
            _runtime = new Runtime(repository);

            Task.Run(() => Setup(CancellationToken.None));
        }

        public async Task Setup(CancellationToken cancellationToken)
        {
            var descriptions = (await _runtime.GetNodeDescriptions(cancellationToken)).ToArray();
            foreach (var description in descriptions)
            {
                NodeDescriptions.Add(description);    
            }
        }
    }
}
