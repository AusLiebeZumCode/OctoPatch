using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Server;

namespace OctoPatch.DesktopClient.ViewModels
{
    public sealed class RuntimeViewModel
    {
        private readonly IRuntime _runtime;

        public ObservableCollection<NodeDescription> NodeDescriptions { get; }

        public ObservableCollection<NodeInstance> Nodes { get; }

        public ObservableCollection<WireInstance> Wires { get; }

        public RuntimeViewModel()
        {
            NodeDescriptions = new ObservableCollection<NodeDescription>();
            Nodes = new ObservableCollection<NodeInstance>();
            Wires = new ObservableCollection<WireInstance>();

            var repository = new Repository();
            _runtime = new Runtime(repository);

            Task.Run(() => Setup(CancellationToken.None));
        }

        public async Task Setup(CancellationToken cancellationToken)
        {
            var descriptions = await _runtime.GetNodeDescriptions(cancellationToken);
            foreach (var description in descriptions)
            {
                NodeDescriptions.Add(description);    
            }
        }
    }
}
