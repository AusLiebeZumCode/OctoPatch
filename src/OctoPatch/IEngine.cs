using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Communication;

namespace OctoPatch
{
    /// <summary>
    /// Interface for the central grid runtime
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// List of all node instances
        /// </summary>
        ReadOnlyObservableCollection<INode> Nodes { get; }

        /// <summary>
        /// List of all wires
        /// </summary>
        ReadOnlyObservableCollection<IWire> Wires { get; }

        /// <summary>
        /// Loads the given grid into the engine
        /// </summary>
        /// <param name="grid">grid setup</param>
        void Load(Grid grid);

        /// <summary>
        /// Returns the current grid setup
        /// </summary>
        /// <returns>current grid setup</returns>
        Grid Store();

        #region engine lifecycle

        /// <summary>
        /// Gets the current engine state
        /// </summary>
        EngineState State { get; }

        /// <summary>
        /// Starts the engine
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// Stops the engine
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        Task Stop(CancellationToken cancellationToken);

        #endregion
    }
}
