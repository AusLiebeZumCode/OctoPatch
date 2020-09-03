using System;
using System.Collections.Generic;
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

        private List<NodeItem> _nodes;

        private List<NodeDescription> _descriptions;

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

        public ObservableCollection<NodeDescription> ContextNodeDescriptions { get; }

        private NodeDescription _selectedContextNodeDescription;

        public NodeDescription SelectedContextNodeDescription
        {
            get => _selectedContextNodeDescription;
            set
            {
                _selectedContextNodeDescription = value;
                OnPropertyChanged();
            }
        }

        private readonly ActionCommand _addSelectedContextNodeDescription;

        public ICommand AddSelectedContextNodeDescription => _addSelectedNodeDescription;


        public ObservableCollection<NodeModel> NodeTree { get; }

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

                ContextNodeDescriptions.Clear();
                switch (value)
                {
                    case InputNodeModel input:

                        foreach (var description in _descriptions.OfType<SplitterNodeDescription>().Where(d => d.TypeKey == input.TypeKey))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;
                    case OutputNodeModel output:

                        foreach (var description in _descriptions.OfType<CollectorNodeDescription>().Where(d => d.TypeKey == output.TypeKey))
                        {
                            ContextNodeDescriptions.Add(description);
                        }
                        
                        break;

                    case CommonNodeModel common:

                        foreach (var description in _descriptions.OfType<AttachedNodeDescription>().Where(d => d.ParentKey == common.Key))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;
                    case AttachedNodeModel attached:

                        foreach (var description in _descriptions.OfType<AttachedNodeDescription>().Where(d => d.ParentKey == attached.Key))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;

                    case SplitterNodeModel splitter:

                        foreach (var description in _descriptions.OfType<AttachedNodeDescription>().Where(d => d.ParentKey == splitter.Key))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;

                    case CollectorNodeModel collector:

                        foreach (var description in _descriptions.OfType<AttachedNodeDescription>().Where(d => d.ParentKey == collector.Key))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;

                }

                //if (value == null)
                //{
                //    NodeDescription = null;
                //}
                //else
                //{
                //    NodeDescription = new NodeDescriptionModel
                //    {
                //        Name = value.Setup.Name,
                //        Description = value.Setup.Description
                //    };
                //}

                //_removeSelectedNode.Enabled = value != null;
                //_startSelectedNode.Enabled = value != null;
                //_stopSelectedNode.Enabled = value != null;
                //_saveNodeDescription.Enabled = value != null;

                //// TODO: Lookup model by Attribute
                //if (value != null && value.Setup.Key == "12ea0035-45af-4da8-8b5d-e1b9d9484ba4:MidiDevice")
                //{
                //    var model = new MidiDeviceModel();
                //    model.Setup(value.Environment);
                //    model.SetConfiguration(value.Setup.Configuration);
                //    NodeConfiguration = model;
                //    _saveNodeConfiguration.Enabled = true;
                //}
                //else
                //{
                //    NodeConfiguration = null;
                //    _saveNodeConfiguration.Enabled = false;
                //}
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
            _nodes = new List<NodeItem>();
            _descriptions = new List<NodeDescription>();

            NodeDescriptions = new ObservableCollection<NodeDescription>();
            ContextNodeDescriptions = new ObservableCollection<NodeDescription>();
            NodeTree = new ObservableCollection<NodeModel>();
            Wires = new ObservableCollection<WireSetup>();

            _addSelectedNodeDescription = new ActionCommand(AddNodeDescriptionCallback, false);
            _addSelectedContextNodeDescription = new ActionCommand(AddContextNodeDescriptionCallback, false);
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

        private void AddContextNodeDescriptionCallback(object obj)
        {
            throw new NotImplementedException();
        }

        private async void SaveNodeConfigurationCallback(object obj)
        {
            //var node = SelectedNode;
            //if (node == null)
            //{
            //    return;
            //}

            //try
            //{
            //    await _runtime.SetNodeConfiguration(node.Setup.NodeId, NodeConfiguration.GetConfiguration(),
            //        CancellationToken.None);
            //}
            //catch (Exception)
            //{
            //    // This is just to see what happens
            //}
        }

        private async void SaveNodeDescriptionCallback(object parameter)
        {
            //var node = SelectedNode;
            //if (node == null)
            //{
            //    return;
            //}

            //await _runtime.SetNodeDescription(node.Setup.NodeId, NodeDescription.Name, NodeDescription.Description,
            //    CancellationToken.None);
        }

        private void RuntimeOnOnNodeUpdated(NodeSetup setup)
        {
            var node = _nodes.FirstOrDefault(n => n.Setup.NodeId == setup.NodeId);
            if (node == null)
            {
                return;
            }

            node.Setup = setup;
        }

        private void RuntimeOnOnNodeStateChanged(Guid nodeId, NodeState state)
        {
            var node = _nodes.FirstOrDefault(n => n.Setup.NodeId == nodeId);
            if (node == null)
            {
                return;
            }

            node.State = state;
        }

        private void RuntimeOnOnNodeEnvironmentChanged(Guid nodeId, string environment)
        {
            var node = _nodes.FirstOrDefault(n => n.Setup.NodeId == nodeId);
            if (node == null)
            {
                return;
            }

            node.Environment = environment;
        }

        private async void StopSelectedNodeCallback(object obj)
        {
            //var node = SelectedNode;
            //if (node == null)
            //{
            //    return;
            //}

            //await _runtime.StopNode(node.Setup.NodeId, CancellationToken.None);
        }

        private async void StartSelectedNodeCallback(object obj)
        {
            //var node = SelectedNode;
            //if (node == null)
            //{
            //    return;
            //}

            //await _runtime.StartNode(node.Setup.NodeId, CancellationToken.None);
        }

        private async void RemoveSelectedNodeCallback(object obj)
        {
            //var node = SelectedNode;
            //if (node == null)
            //{
            //    return;
            //}

            //await _runtime.RemoveNode(node.Setup.NodeId, CancellationToken.None);
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
            var node = _nodes.FirstOrDefault(n => n.Setup.NodeId == obj);
            if (node == null)
            {
                return;
            }

            // TODO: Remove node in tree

            _nodes.Remove(node);
        }

        private void RuntimeOnOnNodeAdded(NodeSetup setup, NodeState state, string environment)
        {
            var node = new NodeItem
            {
                Setup = setup,
                State = state,
                Environment = environment
            };

            // identify description
            var description = _descriptions.FirstOrDefault(d => d.Key == setup.Key);

            NodeModel nodeModel = null;
            switch (description)
            {
                case AttachedNodeDescription attached:
                    nodeModel = new AttachedNodeModel(attached);

                    // TODO: Find parent

                    break;
                case CollectorNodeDescription collector:
                    nodeModel = new CollectorNodeModel(collector);

                    // TODO: Find parent

                    break;
                case SplitterNodeDescription splitter:
                    nodeModel = new SplitterNodeModel(splitter);

                    // TODO: Find parent

                    break;
                case CommonNodeDescription common:
                    nodeModel = new CommonNodeModel(common);
                    NodeTree.Add(nodeModel);
                    break;
            }

            _nodes.Add(node);
        }

        public async Task Setup(CancellationToken cancellationToken)
        {
            var descriptions = await _runtime.GetNodeDescriptions(cancellationToken);

            _descriptions.Clear();
            _descriptions.AddRange(descriptions);

            foreach (var description in _descriptions.OfType<CommonNodeDescription>())
            {
                NodeDescriptions.Add(description);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private sealed class NodeItem
        {
            public NodeSetup Setup { get; set; }

            public NodeState State { get; set; }

            public string Environment { get; set; }
        }
    }
}
