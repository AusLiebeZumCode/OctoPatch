using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using OctoPatch.Descriptions;
using OctoPatch.DesktopClient.Models;
using OctoPatch.Server;
using OctoPatch.Setup;

namespace OctoPatch.DesktopClient.ViewModels
{
    public sealed class RuntimeViewModel : IRuntimeViewModel
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

                _addSelectedNodeDescription.Enabled = value != null;
            }
        }

        private readonly ActionCommand _addSelectedNodeDescription;

        public ICommand AddSelectedNodeDescription => _addSelectedNodeDescription;
        
        public ObservableCollection<NodeSetup> Nodes { get; }
        
        private readonly ActionCommand _removeSelectedNode;

        public ICommand RemoveSelectedNode => _removeSelectedNode;
        
        private readonly ActionCommand _startSelectedNode;

        public ICommand StartSelectedNode => _startSelectedNode;
        
        private readonly ActionCommand _stopSelectedNode;

        public ICommand StopSelectedNode => _stopSelectedNode;

        private NodeSetup _selectedNode;

        public NodeSetup SelectedNode
        {
            get => _selectedNode;
            set
            {
                _selectedNode = value;
                OnPropertyChanged();

                _removeSelectedNode.Enabled = value != null;
                _startSelectedNode.Enabled = value != null;
                _stopSelectedNode.Enabled = value != null;
            }
        }

        public NodeDescriptionModel NodeDescription { get; }
        public ICommand SaveNodeDescription { get; }

        public ObservableCollection<WireSetup> Wires { get; }

        public RuntimeViewModel()
        {
            NodeDescriptions = new ObservableCollection<NodeDescription>();
            Nodes = new ObservableCollection<NodeSetup>();
            Wires = new ObservableCollection<WireSetup>();

            _addSelectedNodeDescription = new ActionCommand(AddNodeDescriptionCallback, false);
            _removeSelectedNode = new ActionCommand(RemoveSelectedNodeCallback, false);
            _startSelectedNode = new ActionCommand(StartSelectedNodeCallback, false);
            _stopSelectedNode = new ActionCommand(StopSelectedNodeCallback, false);

            var repository = new Repository();
            _runtime = new Runtime(repository);

            _runtime.OnNodeAdded += RuntimeOnOnNodeAdded;
            _runtime.OnNodeRemoved += RuntimeOnOnNodeRemoved;
            _runtime.OnWireAdded += RuntimeOnOnWireAdded;
            _runtime.OnWireRemoved += RuntimeOnOnWireRemoved;

            Task.Run(() => Setup(CancellationToken.None));
        }

        private void StopSelectedNodeCallback(object obj)
        {
            
        }

        private void StartSelectedNodeCallback(object obj)
        {
            
        }

        private async void RemoveSelectedNodeCallback(object obj)
        {
            var node = SelectedNode;
            if (node == null)
            {
                return;
            }

            await _runtime.RemoveNode(node.NodeId, CancellationToken.None);
        }

        private async void AddNodeDescriptionCallback(object obj)
        {
            var description = SelectedNodeDescription;
            if (description != null)
            {
                await _runtime.AddNode(description.Key, CancellationToken.None);
            }
        }

        private void RuntimeOnOnWireRemoved(Guid obj)
        {
            throw new NotImplementedException();
        }

        private void RuntimeOnOnWireAdded(WireSetup obj)
        {
            throw new NotImplementedException();
        }

        private void RuntimeOnOnNodeRemoved(Guid obj)
        {
            var node = Nodes.ToArray().FirstOrDefault(n => n.NodeId == obj);
            if (node == null)
            {
                return;
            }

            Nodes.Remove(node);
        }

        private void RuntimeOnOnNodeAdded(NodeSetup obj)
        {
            Nodes.Add(obj);
        }

        public async Task Setup(CancellationToken cancellationToken)
        {
            var descriptions = await _runtime.GetNodeDescriptions(cancellationToken);
            foreach (var description in descriptions.Where(d => d.NodeType == nameof(NodeDescription)))
            {
                NodeDescriptions.Add(description);    
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
