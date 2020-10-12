using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using OctoPatch.Descriptions;
using OctoPatch.DesktopClient.Models;

namespace OctoPatch.DesktopClient.ViewModels
{
    public interface IRuntimeViewModel : INotifyPropertyChanged
    {
        #region Application

        /// <summary>
        /// Command to clean grid
        /// </summary>
        ICommand NewCommand { get; }

        /// <summary>
        /// Loads a file to the grid
        /// </summary>
        ICommand LoadCommand { get; }

        /// <summary>
        /// Saves the current grid to a file
        /// </summary>
        ICommand SaveCommand { get; }

        #endregion

        #region Toolbox

        /// <summary>
        /// List of available nodes to drag into tree
        /// </summary>
        ObservableCollection<NodeDescription> NodeDescriptions { get; }

        /// <summary>
        /// Gets or sets the current selected node description in toolbox
        /// </summary>
        NodeDescription SelectedNodeDescription { get; set; }

        /// <summary>
        /// Command to add the selected node description to the tree
        /// </summary>
        ICommand AddSelectedNodeDescription { get; }

        #endregion

        #region Patch

        /// <summary>
        /// Hierarchical tree structure
        /// </summary>
        ObservableCollection<NodeModel> NodeTree { get; }

        /// <summary>
        /// Gets or sets the current selected node within the tree
        /// </summary>
        NodeModel SelectedNode { get; set; }

        /// <summary>
        /// Command to delete the current selected tree node
        /// </summary>
        ICommand RemoveSelectedNode { get; }

        /// <summary>
        /// Command to delete the current selected wire
        /// </summary>
        ICommand RemoveSelectedWire { get; }

        #endregion

        #region Context Toolbox

        /// <summary>
        /// List of available nodes for the current selected tree node
        /// </summary>
        ObservableCollection<NodeDescription> ContextNodeDescriptions { get; }

        /// <summary>
        /// Gets or sets the current selected node description of context toolbox
        /// </summary>
        NodeDescription SelectedContextNodeDescription { get; set; }

        /// <summary>
        /// Command to add the current selected context node description
        /// to the current selected tree node
        /// </summary>
        ICommand AddSelectedContextNodeDescription { get; }

        #endregion

        #region Property bar

        #region Node lifecycle management

        /// <summary>
        /// Command to start the current selected node
        /// </summary>
        ICommand StartSelectedNode { get; }

        /// <summary>
        /// Command to stop the current selected node
        /// </summary>
        ICommand StopSelectedNode { get; }

        #endregion

        #region Common node configuration

        /// <summary>
        /// Gets the common configuration (name and description) of the current selected tree node
        /// </summary>
        NodeDescriptionModel NodeDescription { get; }

        /// <summary>
        /// Command to store all the changed made in the NodeDescription property
        /// </summary>
        ICommand SaveNodeDescription { get; }

        #endregion

        #region Node specific configuration

        /// <summary>
        /// Holds the specific configuration model for the current selected tree node
        /// </summary>
        ConfigurationModel NodeConfiguration { get; }

        /// <summary>
        /// Command to store all the changes made in the node configuration
        /// </summary>
        ICommand SaveNodeConfiguration { get; }

        #endregion

        #region Wire wizard

        /// <summary>
        /// gets the current selected ouptut connector (if available)
        /// </summary>
        OutputNodeModel SelectedWireConnector { get; }

        /// <summary>
        /// Command to take the selected output / input connector to wire them up
        /// </summary>
        ICommand TakeConnector { get; }

        #endregion

        #region Wire configuration

        /// <summary>
        /// List of all available adapters for the selected wire context
        /// </summary>
        ObservableCollection<AdapterDescription> AdapterDescriptions { get; }

        /// <summary>
        /// Gets or sets the current selected adapter
        /// </summary>
        AdapterDescription SelectedAdapterDescription { get; set; }

        /// <summary>
        /// Command to apply current adapter selection to the selected wire
        /// </summary>
        ICommand SaveAdapter { get; }

        #endregion

        #region Adapter configuration

        /// <summary>
        /// Gets the configuration for the adapter of the current selected wire
        /// </summary>
        ConfigurationModel AdapterConfiguration { get; }

        /// <summary>
        /// Command to store all the changes in the adapter configuration
        /// </summary>
        ICommand SaveAdapterConfiguration { get; }

        #endregion

        #endregion
    }
}
