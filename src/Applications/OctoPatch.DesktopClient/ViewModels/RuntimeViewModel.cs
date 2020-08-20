using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Server;

namespace OctoPatch.DesktopClient.ViewModels
{
    public sealed class RuntimeViewModel : INotifyPropertyChanged
    {
        private readonly IRuntime _runtime;

        public ObservableCollection<NodeDescription> NodeDescriptions { get; }

        private NodeDescription _selectedNodeDescription;

        public NodeDescription SelectedNodeDescription
        {
            get => _selectedNodeDescription;
            set
            {
                _selectedNodeDescription = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<NodeInstance> Nodes { get; }

        private NodeInstance _selectedNode;

        public NodeInstance SelectedNode
        {
            get => _selectedNode;
            set
            {
                _selectedNode = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<WireInstance> Wires { get; }

        public RuntimeViewModel()
        {
            NodeDescriptions = new ObservableCollection<NodeDescription>();
            Nodes = new ObservableCollection<NodeInstance>();
            Wires = new ObservableCollection<WireInstance>();

            var repository = new Repository();
            _runtime = new Runtime(repository);

            _runtime.OnNodeAdded += RuntimeOnOnNodeAdded;
            _runtime.OnNodeRemoved += RuntimeOnOnNodeRemoved;
            _runtime.OnWireAdded += RuntimeOnOnWireAdded;
            _runtime.OnWireRemoved += RuntimeOnOnWireRemoved;

            Task.Run(() => Setup(CancellationToken.None));
        }

        private void RuntimeOnOnWireRemoved(Guid obj)
        {
            throw new NotImplementedException();
        }

        private void RuntimeOnOnWireAdded(WireInstance obj)
        {
            throw new NotImplementedException();
        }

        private void RuntimeOnOnNodeRemoved(Guid obj)
        {
            throw new NotImplementedException();
        }

        private void RuntimeOnOnNodeAdded(NodeInstance obj)
        {
            Nodes.Add(obj);
        }

        public async Task Setup(CancellationToken cancellationToken)
        {
            var descriptions = await _runtime.GetNodeDescriptions(cancellationToken);
            foreach (var description in descriptions)
            {
                NodeDescriptions.Add(description);    
            }
        }

        public async void AddNode(NodeDescription description)
        {
            await _runtime.AddNode(description.Guid, CancellationToken.None);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
