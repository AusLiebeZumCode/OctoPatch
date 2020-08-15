using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OctoPatch.Communication;

namespace OctoPatch
{
    /// <summary>
    /// Base class for all kind of node implementations
    /// </summary>
    /// <typeparam name="T">configuration type</typeparam>
    public abstract class Node<T> : INode where T : INodeConfiguration
    {
        /// <summary>
        /// Local lock to synchronize the node lifecycle
        /// </summary>
        private readonly SemaphoreSlim _localLock;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public NodeState State { get; private set; }

        protected readonly List<IInputConnector> _inputs;

        protected readonly List<IOutputConnector> _outputs;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public NodeInstance Instance { get; private set; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<IInputConnector> Inputs => _inputs;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<IOutputConnector> Outputs => _outputs;

        protected Node()
        {
            _inputs = new List<IInputConnector>();
            _outputs = new List<IOutputConnector>();
            State = NodeState.NotReady;
            _localLock = new SemaphoreSlim(1);
        }

        #region Lifecycle methods

        public async Task Setup(NodeInstance instance, CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                if (State != NodeState.NotReady)
                {
                    throw new NotSupportedException("node is in the wrong state");
                }

                State = NodeState.Initializing;
                var configuration = JsonConvert.DeserializeObject<T>(instance.Configuration);
                await OnSetup(configuration, cancellationToken);
                Instance = instance;
                State = NodeState.Running;
            }
            catch (Exception)
            {
                State = NodeState.Failed;
                throw;
            }
            finally
            {
                _localLock.Release();
            }
        }

        public abstract Task OnSetup(T configuration, CancellationToken cancellationToken);

        public async Task Start(CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                if (State != NodeState.Stopped)
                {
                    throw new NotSupportedException("node is in the wrong state");
                }

                State = NodeState.Starting;
                await OnStart(cancellationToken);
                State = NodeState.Running;
            }
            catch (Exception)
            {
                State = NodeState.Failed;
                throw;
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// 
        /// Gets a call when node is starting
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        protected abstract Task OnStart(CancellationToken cancellationToken);

        public async Task Stop(CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                if (State != NodeState.Running)
                {
                    throw new NotSupportedException("node is in the wrong state");
                }

                State = NodeState.Stopping;
                await OnStart(cancellationToken);
                State = NodeState.Stopped;
            }
            catch (Exception)
            {
                State = NodeState.Failed;
                throw;
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Gets a call when node is stopping
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task OnStop(CancellationToken cancellationToken);

        public async Task Dispose(CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                if (State != NodeState.Stopped)
                {
                    throw new NotSupportedException("node is in the wrong state");
                }

                State = NodeState.Disposing;
                await OnStart(cancellationToken);
                State = NodeState.NotReady;
            }
            catch (Exception)
            {
                State = NodeState.Failed;
                throw;
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Gets a call when node gets disposed
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task OnDispose(CancellationToken cancellationToken);

        public async Task Reset(CancellationToken cancellationToken)
        {
            await _localLock.WaitAsync(cancellationToken);
            try
            {
                if (State != NodeState.Failed)
                {
                    throw new NotSupportedException("node is in the wrong state");
                }

                await OnStart(cancellationToken);
                State = NodeState.NotReady;
            }
            catch (Exception)
            {
                State = NodeState.Failed;
                throw;
            }
            finally
            {
                _localLock.Release();
            }
        }

        /// <summary>
        /// Gets a call when node gets a reset
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        protected abstract Task OnReset(CancellationToken cancellationToken);

        #endregion
    }
}
