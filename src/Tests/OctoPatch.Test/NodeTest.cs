using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OctoPatch.Test
{
    /// <summary>
    /// Collection of tests around the node base class
    /// </summary>
    public sealed class NodeTest
    {
        public const string ResetName = "Reset";

        public const string InitializeResetName = "InitializeReset";

        #region Transitions

        /// <summary>
        /// Tests the transition behavior of the initialize method
        /// </summary>
        /// <param name="currentState">current state</param>
        /// <param name="exception">does the call expect an exception</param>
        /// <param name="targetState">expected target state</param>
        /// <param name="expectedCalls">list of expected method calls</param>
        [Theory]
        [InlineData(NodeState.Uninitialized, false, NodeState.Stopped, nameof(INode.Initialize))]
        [InlineData(NodeState.Stopped, false, NodeState.Stopped, nameof(INode.Deinitialize), nameof(INode.Initialize))]
        [InlineData(NodeState.Running, false, NodeState.Running, nameof(INode.Stop), nameof(INode.Deinitialize), nameof(INode.Initialize), nameof(INode.Start))]
        [InlineData(NodeState.InitializationFailed, false, NodeState.Stopped, InitializeResetName, nameof(INode.Initialize))]
        [InlineData(NodeState.Failed, false, NodeState.Stopped, ResetName, nameof(INode.Deinitialize), nameof(INode.Initialize))]
        public Task InitializeTest(NodeState currentState, bool exception, NodeState targetState, params string[] expectedCalls)
        {
            return ExecuteTest((n, t) => n.Initialize(Guid.Empty, "{}", t), currentState, exception, targetState, expectedCalls);
        }

        /// <summary>
        /// Tests the transition behavior of the start method
        /// </summary>
        /// <param name="currentState">current state</param>
        /// <param name="exception">does the call expect an exception</param>
        /// <param name="targetState">expected target state</param>
        /// <param name="expectedCalls">list of expected method calls</param>
        [Theory]
        [InlineData(NodeState.Uninitialized, true, NodeState.Uninitialized)]
        [InlineData(NodeState.Stopped, false, NodeState.Running, nameof(INode.Start))]
        [InlineData(NodeState.Running, false, NodeState.Running)]
        [InlineData(NodeState.InitializationFailed, true, NodeState.InitializationFailed)]
        [InlineData(NodeState.Failed, false, NodeState.Running, ResetName, nameof(INode.Start))]
        public Task StartTest(NodeState currentState, bool exception, NodeState targetState, params string[] expectedCalls)
        {
            return ExecuteTest((n, t) => n.Start(t), currentState, exception, targetState, expectedCalls);
        }

        /// <summary>
        /// Tests the transition behavior of the stop method
        /// </summary>
        /// <param name="currentState">current state</param>
        /// <param name="exception">does the call expect an exception</param>
        /// <param name="targetState">expected target state</param>
        /// <param name="expectedCalls">list of expected method calls</param>
        [Theory]
        [InlineData(NodeState.Uninitialized, true, NodeState.Uninitialized)]
        [InlineData(NodeState.Stopped, false, NodeState.Stopped)]
        [InlineData(NodeState.Running, false, NodeState.Stopped, nameof(INode.Stop))]
        [InlineData(NodeState.InitializationFailed, true, NodeState.InitializationFailed)]
        [InlineData(NodeState.Failed, false, NodeState.Stopped, ResetName)]
        public Task StopTest(NodeState currentState, bool exception, NodeState targetState, params string[] expectedCalls)
        {
            return ExecuteTest((n, t) => n.Stop(t), currentState, exception, targetState, expectedCalls);
        }

        /// <summary>
        /// Tests the transition behavior of the Deinitialze method
        /// </summary>
        /// <param name="currentState">current state</param>
        /// <param name="exception">does the call expect an exception</param>
        /// <param name="targetState">expected target state</param>
        /// <param name="expectedCalls">list of expected method calls</param>
        [Theory]
        [InlineData(NodeState.Uninitialized, false, NodeState.Uninitialized)]
        [InlineData(NodeState.Stopped, false, NodeState.Uninitialized, nameof(INode.Deinitialize))]
        [InlineData(NodeState.Running, false, NodeState.Uninitialized, nameof(INode.Stop), nameof(INode.Deinitialize))]
        [InlineData(NodeState.InitializationFailed, false, NodeState.Uninitialized, InitializeResetName)]
        [InlineData(NodeState.Failed, false, NodeState.Uninitialized, ResetName, nameof(INode.Deinitialize))]
        public Task DeinitializeTest(NodeState currentState, bool exception, NodeState targetState, params string[] expectedCalls)
        {
            return ExecuteTest((n, t) => n.Deinitialize(t), currentState, exception, targetState, expectedCalls);
        }

        /// <summary>
        /// Wraps the whole test structure of executing specific methods
        /// </summary>
        /// <param name="action">execution handler</param>
        /// <param name="currentState">current state</param>
        /// <param name="exception">does the call expect an exception</param>
        /// <param name="targetState">expected target state</param>
        /// <param name="expectedCalls">list of expected method calls</param>
        private async Task ExecuteTest(Func<INode, CancellationToken, Task> action, NodeState currentState,
            bool exception, NodeState targetState, params string[] expectedCalls)
        {
            // Setup
            var node = await CreateNode(currentState);

            // Act
            if (exception)
            {
                await Assert.ThrowsAsync<NotSupportedException>(() => action(node, CancellationToken.None));
            }
            else
            {
                await action(node, CancellationToken.None);
            }

            // Assert
            Assert.Equal(targetState, node.State);
            Assert.Equal(expectedCalls.Length, node.CallList.Count);
            for (var i = 0; i < expectedCalls.Length; i++)
            {
                Assert.Equal(expectedCalls[i], node.CallList[i]);
            }
        }

        /// <summary>
        /// Creates a node in the specific state
        /// </summary>
        /// <param name="state">expected state of the returning node</param>
        /// <returns>new node instance</returns>
        private async Task<NodeMock> CreateNode(NodeState state)
        {
            var node = new NodeMock();

            switch (state)
            {
                case NodeState.Uninitialized: break;
                case NodeState.Stopped:
                    await node.Initialize(Guid.Empty, "{}", CancellationToken.None);
                    break;
                case NodeState.Running:
                    await node.Initialize(Guid.Empty, "{}", CancellationToken.None);
                    await node.Start(CancellationToken.None);
                    break;
                case NodeState.InitializationFailed:
                    node.InitializeBehavior = MockBehavior.ThrowException;
                    try
                    {
                        await node.Initialize(Guid.Empty, "{}", CancellationToken.None);
                    }
                    catch (Exception)
                    {
                        // This is expected to go into target state
                    }

                    break;
                case NodeState.Failed:
                    await node.Initialize(Guid.Empty, "{}", CancellationToken.None);
                    await node.LetFail(CancellationToken.None);
                    break;
                case NodeState.Resetting:
                case NodeState.Starting:
                case NodeState.Stopping:
                case NodeState.Initializing:
                case NodeState.Deinitializing:
                    throw new NotSupportedException("transition states are not supported");
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            // Cleanup node
            node.CallList.Clear();
            node.InitializeBehavior = MockBehavior.DoNothing;
            node.StartBehavior = MockBehavior.DoNothing;
            node.StopBehavior = MockBehavior.DoNothing;
            node.DeinitializeBehavior = MockBehavior.DoNothing;

            return node;
        }

        #endregion

        #region mock implementation

        /// <summary>
        /// Mock implementation based on the node class
        /// </summary>
        public sealed class NodeMock : Node<NodeConfigurationMock>
        {
            /// <summary>
            /// Gets or sets the behavior of the initialize calls
            /// </summary>
            public MockBehavior InitializeBehavior { get; set; }

            /// <summary>
            /// Gets or sets the behavior of the start calls
            /// </summary>
            public MockBehavior StartBehavior { get; set; }

            /// <summary>
            /// Gets or sets the behavior of the stop calls
            /// </summary>
            public MockBehavior StopBehavior { get; set; }

            /// <summary>
            /// Gets or sets the behavior of the Deinitialize calls
            /// </summary>
            public MockBehavior DeinitializeBehavior { get; set; }

            /// <summary>
            /// Gets or sets the behavior of the initialize reset calls
            /// </summary>
            public MockBehavior InitializeResetBehavior { get; set; }

            /// <summary>
            /// Gets or sets the behavior of the Reset calls
            /// </summary>
            public MockBehavior ResetBehavior { get; set; }

            /// <summary>
            /// Lists up all historical calls
            /// </summary>
            public List<string> CallList { get; }

            /// <summary>
            /// Trigger when call enters the inner handle method
            /// </summary>
            public ManualResetEvent HandlerTrigger { get; }

            public NodeMock()
            {
                CallList = new List<string>();
                HandlerTrigger = new ManualResetEvent(false);
            }

            public override Task OnInitialize(NodeConfigurationMock configuration, CancellationToken cancellationToken)
            {
                if (configuration == null)
                {
                    throw new ArgumentNullException(nameof(configuration));
                }

                return HandleBehavior(InitializeBehavior, cancellationToken, nameof(INode.Initialize));
            }

            protected override Task OnStart(CancellationToken cancellationToken)
            {
                return HandleBehavior(StartBehavior, cancellationToken, nameof(INode.Start));
            }

            protected override Task OnStop(CancellationToken cancellationToken)
            {
                return HandleBehavior(StopBehavior, cancellationToken, nameof(INode.Stop));
            }

            protected override Task OnDeinitialize(CancellationToken cancellationToken)
            {
                return HandleBehavior(DeinitializeBehavior, cancellationToken, nameof(INode.Deinitialize));
            }

            protected override Task OnReset(CancellationToken cancellationToken)
            {
                return HandleBehavior(ResetBehavior, cancellationToken, ResetName);
            }

            protected override Task OnInitializeReset(CancellationToken cancellationToken)
            {
                return HandleBehavior(InitializeResetBehavior, cancellationToken, InitializeResetName);
            }

            public Task LetFail(CancellationToken cancellationToken)
            {
                return Fail(cancellationToken);
            }

            /// <summary>
            /// Handles the current call based on the configured behavior
            /// </summary>
            /// <param name="behavior">method behavior</param>
            /// <param name="cancellationToken">cancellation token</param>
            /// <param name="callerName">caller name</param>
            private Task HandleBehavior(MockBehavior behavior, CancellationToken cancellationToken, string callerName)
            {
                // Add this call to the list
                CallList.Add(callerName);

                // Trigger event
                HandlerTrigger.Set();

                // Handle the behavior
                switch (behavior)
                {
                    case MockBehavior.DoNothing:
                        return Task.CompletedTask;
                    case MockBehavior.ThrowException:
                        throw new NotSupportedException("based on behavior");
                    case MockBehavior.WaitForCancel:
                        Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                        return Task.CompletedTask;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// List of possible behaviors of the node mock
        /// </summary>
        public enum MockBehavior
        {
            /// <summary>
            /// Just do nothing
            /// </summary>
            DoNothing,

            /// <summary>
            /// Throws a <see cref="System.NotSupportedException"/> on next call
            /// </summary>
            ThrowException,

            /// <summary>
            /// Next call blocks until cancellation token will be triggered
            /// </summary>
            WaitForCancel
        }

        /// <summary>
        /// Mock configuration
        /// </summary>
        [DataContract]
        public sealed class NodeConfigurationMock : INodeConfiguration
        {
            /// <summary>
            /// Dummy property to double check
            /// </summary>
            [DataMember]
            public string Name { get; set; }
        }

        #endregion
    }
}
