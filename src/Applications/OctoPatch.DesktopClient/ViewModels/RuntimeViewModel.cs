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

        public ObservableCollection<NodeModel> Nodes { get; }

        private readonly ActionCommand _removeSelectedNode;

        public ICommand RemoveSelectedNode => _removeSelectedNode;

        private readonly ActionCommand _startSelectedNode;

        public ICommand StartSelectedNode => _startSelectedNode;

        private readonly ActionCommand _stopSelectedNode;

        public ICommand StopSelectedNode => _stopSelectedNode;

        private NodeModel _selectedNode;

        public NodeModel SelectedNode
        {
            get => _selectedNode;
            set
            {
                _selectedNode = value;
                OnPropertyChanged();

                if (value == null)
                {
                    NodeDescription = null;
                }
                else
                {
                    NodeDescription = new NodeDescriptionModel
                    {
                        Name = value.Setup.Name,
                        Description = value.Setup.Description
                    };
                }

                _removeSelectedNode.Enabled = value != null;
                _startSelectedNode.Enabled = value != null;
                _stopSelectedNode.Enabled = value != null;
                _saveNodeDescription.Enabled = value != null;

                // TODO: Lookup model by Attribute
                if (value != null && value.Setup.Key == "12ea0035-45af-4da8-8b5d-e1b9d9484ba4:MidiDevice")
                {
                    var model = new MidiDeviceModel();
                    model.Setup(value.Environment);
                    model.SetConfiguration(value.Setup.Configuration);
                    NodeConfiguration = model;
                    _saveNodeConfiguration.Enabled = true;
                }
                else
                {
                    NodeConfiguration = null;
                    _saveNodeConfiguration.Enabled = false;
                }
            }
        }

        private NodeDescriptionModel _nodeDescription;

        public NodeDescriptionModel NodeDescription
        {
            get => _nodeDescription;
            private set
            {
                _nodeDescription = value;
                OnPropertyChanged();
            }
        }

        private readonly ActionCommand _saveNodeDescription;

        public ICommand SaveNodeDescription => _saveNodeDescription;

        private NodeConfigurationModel _nodeConfiguration;

        public NodeConfigurationModel NodeConfiguration
        {
            get => _nodeConfiguration;
            private set
            {
                _nodeConfiguration = value;
                OnPropertyChanged();
            }
        }

        private readonly ActionCommand _saveNodeConfiguration;

        public ICommand SaveNodeConfiguration => _saveNodeConfiguration;

        public ObservableCollection<WireSetup> Wires { get; }

        public RuntimeViewModel()
        {
            NodeDescriptions = new ObservableCollection<NodeDescription>();
            Nodes = new ObservableCollection<NodeModel>();
            Wires = new ObservableCollection<WireSetup>();

            _addSelectedNodeDescription = new ActionCommand(AddNodeDescriptionCallback, false);
            _removeSelectedNode = new ActionCommand(RemoveSelectedNodeCallback, false);
            _startSelectedNode = new ActionCommand(StartSelectedNodeCallback, false);
            _stopSelectedNode = new ActionCommand(StopSelectedNodeCallback, false);
            _saveNodeDescription = new ActionCommand(SaveNodeDescriptionCallback, false);
            _saveNodeConfiguration = new ActionCommand(SaveNodeConfigurationCallback, false);

            var repository = new Repository();
            _runtime = new Runtime(repository);

            _runtime.OnNodeAdded += RuntimeOnOnNodeAdded;
            _runtime.OnNodeRemoved += RuntimeOnOnNodeRemoved;
            _runtime.OnNodeUpdated += RuntimeOnOnNodeUpdated;
            _runtime.OnNodeStateChanged += RuntimeOnOnNodeStateChanged;
            _runtime.OnNodeEnvironmentChanged += RuntimeOnOnNodeEnvironmentChanged;
            _runtime.OnWireAdded += RuntimeOnOnWireAdded;
            _runtime.OnWireRemoved += RuntimeOnOnWireRemoved;

            Task.Run(() => Setup(CancellationToken.None));
        }

        private async void SaveNodeConfigurationCallback(object obj)
        {
            var node = SelectedNode;
            if (node == null)
            {
                return;
            }

            try
            {
                await _runtime.SetNodeConfiguration(node.Setup.NodeId, NodeConfiguration.GetConfiguration(),
                    CancellationToken.None);
            }
            catch (Exception)
            {
                // This is just to see what happens
            }
        }

        private async void SaveNodeDescriptionCallback(object parameter)
        {
            var node = SelectedNode;
            if (node == null)
            {
                return;
            }

            await _runtime.SetNodeDescription(node.Setup.NodeId, NodeDescription.Name, NodeDescription.Description,
                CancellationToken.None);
        }

        private void RuntimeOnOnNodeUpdated(NodeSetup setup)
        {
            var node = Nodes.ToArray().FirstOrDefault(n => n.Setup.NodeId == setup.NodeId);
            if (node == null)
            {
                return;
            }

            node.Setup = setup;
        }

        private void RuntimeOnOnNodeStateChanged(Guid nodeId, NodeState state)
        {
            var node = Nodes.ToArray().FirstOrDefault(n => n.Setup.NodeId == nodeId);
            if (node == null)
            {
                return;
            }

            node.State = state;
        }

        private void RuntimeOnOnNodeEnvironmentChanged(Guid nodeId, string environment)
        {
            var node = Nodes.ToArray().FirstOrDefault(n => n.Setup.NodeId == nodeId);
            if (node == null)
            {
                return;
            }

            node.Environment = environment;
        }

        private async void StopSelectedNodeCallback(object obj)
        {
            var node = SelectedNode;
            if (node == null)
            {
                return;
            }

            await _runtime.StopNode(node.Setup.NodeId, CancellationToken.None);
        }

        private async void StartSelectedNodeCallback(object obj)
        {
            var node = SelectedNode;
            if (node == null)
            {
                return;
            }

            await _runtime.StartNode(node.Setup.NodeId, CancellationToken.None);
        }

        private async void RemoveSelectedNodeCallback(object obj)
        {
            var node = SelectedNode;
            if (node == null)
            {
                return;
            }

            await _runtime.RemoveNode(node.Setup.NodeId, CancellationToken.None);
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
            var node = Nodes.ToArray().FirstOrDefault(n => n.Setup.NodeId == obj);
            if (node == null)
            {
                return;
            }

            Nodes.Remove(node);
        }

        private void RuntimeOnOnNodeAdded(NodeSetup setup, NodeState state, string environment)
        {
            Nodes.Add(new NodeModel { Setup = setup, State = state, Environment = environment });
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
