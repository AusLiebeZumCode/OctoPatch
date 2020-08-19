using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using OctoPatch.Core;
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

        #region wrong parameters

        /// <summary>
        /// Tests the behavior with broken configuration
        /// </summary>
        /// <param name="configuration">configuration samples</param>
        [Theory]
        [InlineData(null)] // null is invalid
        [InlineData("")] // empty string is invalid
        [InlineData("rubbish data")] // not deserializable 
        public async Task InvalidConfigurationTest(string configuration)
        {
            // Setup
            var node = await CreateNode(NodeState.Uninitialized);

            // Act && Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(() =>
                node.Initialize(configuration, CancellationToken.None));
        }

        #endregion

        #region Exception handling

        /// <summary>
        /// Tests behavior when exceptions happen in initialize
        /// </summary>
        [Fact]
        public async Task ExceptionsDuringInitialize()
        {
            // Setup
            var node = await CreateNode(NodeState.Uninitialized);
            node.InitializeBehavior = MockBehavior.ThrowException;

            // Act
            await Assert.ThrowsAsync<NotSupportedException>(() =>
                node.Initialize("{}", CancellationToken.None));

            // Assert
            Assert.Equal(NodeState.InitializationFailed, node.State);
            Assert.Single(node.CallList);
            Assert.Equal(nameof(INode.Initialize), node.CallList[0]);
        }

        /// <summary>
        /// Tests behavior when exceptions happen in initialize reset (Exception should not bubble up)
        /// </summary>
        [Fact]
        public async Task ExceptionsDuringInitializeReset()
        {
            // Setup
            var node = await CreateNode(NodeState.InitializationFailed);
            node.InitializeResetBehavior = MockBehavior.ThrowException;

            // Act
            await node.Initialize( "{}", CancellationToken.None);

            // Assert
            Assert.Equal(NodeState.Stopped, node.State);
            Assert.Equal(2, node.CallList.Count);
            Assert.Equal(InitializeResetName, node.CallList[0]);
            Assert.Equal(nameof(INode.Initialize), node.CallList[1]);
        }

        /// <summary>
        /// Tests behavior when exception happens in start
        /// </summary>
        [Fact]
        public async Task ExceptionsDuringStart()
        {
            // Setup
            var node = await CreateNode(NodeState.Stopped);
            node.StartBehavior = MockBehavior.ThrowException;

            // Act
            await Assert.ThrowsAsync<NotSupportedException>(() =>
                node.Start(CancellationToken.None));

            // Assert
            Assert.Equal(NodeState.Failed, node.State);
            Assert.Single(node.CallList);
            Assert.Equal(nameof(INode.Start), node.CallList[0]);
        }

        /// <summary>
        /// Tests behavior when exception happens in stop
        /// </summary>
        [Fact]
        public async Task ExceptionsDuringStop()
        {
            // Setup
            var node = await CreateNode(NodeState.Running);
            node.StopBehavior = MockBehavior.ThrowException;

            // Act
            await Assert.ThrowsAsync<NotSupportedException>(() =>
                node.Stop(CancellationToken.None));

            // Assert
            Assert.Equal(NodeState.Failed, node.State);
            Assert.Single(node.CallList);
            Assert.Equal(nameof(INode.Stop), node.CallList[0]);
        }

        /// <summary>
        /// Tests behavior when exception happens in reset
        /// </summary>
        [Fact]
        public async Task ExceptionsDuringReset()
        {
            // Setup
            var node = await CreateNode(NodeState.Failed);
            node.ResetBehavior = MockBehavior.ThrowException;

            // Act
            await node.Start(CancellationToken.None);

            // Assert
            Assert.Equal(NodeState.Running, node.State);
            Assert.Equal(2, node.CallList.Count);
            Assert.Equal(ResetName, node.CallList[0]);
            Assert.Equal(nameof(INode.Start), node.CallList[1]);
        }

        /// <summary>
        /// Tests behavior when exceptions happen in deinitialize (does not bubble up)
        /// </summary>
        [Fact]
        public async Task ExceptionsDuringDeinitialize()
        {
            // Setup
            var node = await CreateNode(NodeState.Stopped);
            node.DeinitializeBehavior = MockBehavior.ThrowException;

            // Act
            await node.Deinitialize(CancellationToken.None);

            // Assert
            Assert.Equal(NodeState.Uninitialized, node.State);
            Assert.Single(node.CallList);
            Assert.Equal(nameof(INode.Deinitialize), node.CallList[0]);
        }

        #endregion

        #region Cancellation handling

        /// <summary>
        /// Checks behavior of a cancelled initialize call
        /// </summary>
        /// <param name="initState">initial state</param>
        /// <param name="exceptedState">expected state</param>
        [Theory]
        [InlineData(NodeState.Uninitialized, NodeState.Uninitialized)]
        [InlineData(NodeState.InitializationFailed, NodeState.Uninitialized)]
        [InlineData(NodeState.Stopped, NodeState.Uninitialized)]
        [InlineData(NodeState.Running, NodeState.Uninitialized)]
        [InlineData(NodeState.Failed, NodeState.Uninitialized)]
        public async Task CancellationDuringInitialize(NodeState initState, NodeState exceptedState)
        {
            // Setup
            var node = await CreateNode(initState);
            node.InitializeBehavior = MockBehavior.WaitForCancel;

            // Act
            await ExecuteWithToken(node, node.InitializeTrigger, (n, t) => n.Initialize( "{}", t));

            // Assert
            Assert.Equal(exceptedState, node.State);
        }

        /// <summary>
        /// Checks behavior of a cancelled start call
        /// </summary>
        /// <param name="initState">initial state</param>
        /// <param name="exceptedState">expected state</param>
        [Theory]
        [InlineData(NodeState.Stopped, NodeState.Stopped)]
        [InlineData(NodeState.Failed, NodeState.Stopped)]
        public async Task CancellationDuringStart(NodeState initState, NodeState exceptedState)
        {
            // Setup
            var node = await CreateNode(initState);
            node.StartBehavior = MockBehavior.WaitForCancel;

            // Act
            await ExecuteWithToken(node, node.StartTrigger, (n, t) => n.Start(t));

            // Assert
            Assert.Equal(exceptedState, node.State);
        }

        /// <summary>
        /// Checks behavior of a cancelled stop call
        /// </summary>
        /// <param name="initState">initial state</param>
        /// <param name="exceptedState">expected state</param>
        [Theory]
        [InlineData(NodeState.Running, NodeState.Running)]
        public async Task CancellationDuringStop(NodeState initState, NodeState exceptedState)
        {
            // Setup
            var node = await CreateNode(initState);
            node.StopBehavior = MockBehavior.WaitForCancel;

            // Act
            await ExecuteWithToken(node, node.StopTrigger, (n, t) => n.Stop(t));

            // Assert
            Assert.Equal(exceptedState, node.State);
        }

        /// <summary>
        /// Checks behavior of a cancelled initialize call
        /// </summary>
        /// <param name="initState">initial state</param>
        /// <param name="exceptedState">expected state</param>
        [Theory]
        [InlineData(NodeState.Stopped, NodeState.Stopped)]
        [InlineData(NodeState.Running, NodeState.Stopped)]
        [InlineData(NodeState.Failed, NodeState.Stopped)]
        public async Task CancellationDuringDeinitialize(NodeState initState, NodeState exceptedState)
        {
            // Setup
            var node = await CreateNode(initState);
            node.DeinitializeBehavior = MockBehavior.WaitForCancel;

            // Act
            await ExecuteWithToken(node, node.DeinitializeTrigger, (n, t) => n.Deinitialize(t));

            // Assert
            Assert.Equal(exceptedState, node.State);
        }

        /// <summary>
        /// Checks behavior of a cancelled initialize reset call (implicit)
        /// </summary>
        [Fact]
        public async Task CancellationDuringInitializeReset()
        {
            // Setup
            var node = await CreateNode(NodeState.InitializationFailed);
            node.InitializeResetBehavior = MockBehavior.WaitForCancel;

            // Act
            await ExecuteWithToken(node, node.InitializeResetTrigger, (n, t) => n.Initialize( "{}", t));

            // Assert
            Assert.Equal(NodeState.InitializationFailed, node.State);
        }

        /// <summary>
        /// Checks behavior of a cancelled initialize call (implicit)
        /// </summary>
        [Fact]
        public async Task CancellationDuringReset()
        {
            // Setup
            var node = await CreateNode(NodeState.Failed);
            node.ResetBehavior = MockBehavior.WaitForCancel;

            // Act
            await ExecuteWithToken(node, node.ResetTrigger, (n, t) => n.Start(t));

            // Assert
            Assert.Equal(NodeState.Failed, node.State);
        }

        /// <summary>
        /// Executes and cancels the given action
        /// </summary>
        /// <param name="node">reference to the node</param>
        /// <param name="resetEvent">fitting reset event for cancellation wait</param>
        /// <param name="action">delegate</param>
        private async Task ExecuteWithToken(NodeMock node, ManualResetEvent resetEvent, Func<INode, CancellationToken, Task> action)
        {
            using var tokenSource = new CancellationTokenSource();

            // Start method
            var executionTask = action(node, tokenSource.Token);

            // Wait until the call enters the inner handler
            resetEvent.WaitOne();

            // cancel process
            tokenSource.Cancel();

            // Wait for end of execution
            await Assert.ThrowsAsync<TaskCanceledException>(() => executionTask);
        }

        #endregion

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
            return ExecuteTest((n, t) => n.Initialize( "{}", t), currentState, exception, targetState, expectedCalls);
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
                    await node.Initialize( "{}", CancellationToken.None);
                    break;
                case NodeState.Running:
                    await node.Initialize( "{}", CancellationToken.None);
                    await node.Start(CancellationToken.None);
                    break;
                case NodeState.InitializationFailed:
                    node.InitializeBehavior = MockBehavior.ThrowException;
                    try
                    {
                        await node.Initialize("{}", CancellationToken.None);
                    }
                    catch (Exception)
                    {
                        // This is expected to go into target state
                    }

                    break;
                case NodeState.Failed:
                    await node.Initialize( "{}", CancellationToken.None);
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
            node.Reset();

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
            /// Trigger when call enters OnInitialize
            /// </summary>
            public ManualResetEvent InitializeTrigger { get; }

            /// <summary>
            /// Trigger when call enters OnInitializeReset
            /// </summary>
            public ManualResetEvent InitializeResetTrigger { get; }

            /// <summary>
            /// Trigger when call enters OnStart
            /// </summary>
            public ManualResetEvent StartTrigger { get; }

            /// <summary>
            /// Trigger when call enters OnStop
            /// </summary>
            public ManualResetEvent StopTrigger { get; }

            /// <summary>
            /// Trigger when call enters OnInitialize
            /// </summary>
            public ManualResetEvent ResetTrigger { get; }

            /// <summary>
            /// Trigger when call enters OnInitialize
            /// </summary>
            public ManualResetEvent DeinitializeTrigger { get; }

            public NodeMock() : base(Guid.NewGuid())
            {
                CallList = new List<string>();
                InitializeTrigger = new ManualResetEvent(false);
                InitializeResetTrigger = new ManualResetEvent(false);
                StartTrigger = new ManualResetEvent(false);
                StopTrigger = new ManualResetEvent(false);
                ResetTrigger = new ManualResetEvent(false);
                DeinitializeTrigger = new ManualResetEvent(false);
            }

            public void Reset()
            {
                CallList.Clear();

                InitializeBehavior = MockBehavior.DoNothing;
                InitializeResetBehavior = MockBehavior.DoNothing;
                StartBehavior = MockBehavior.DoNothing;
                StopBehavior = MockBehavior.DoNothing;
                ResetBehavior = MockBehavior.DoNothing;
                DeinitializeBehavior = MockBehavior.DoNothing;

                InitializeTrigger.Reset();
                InitializeResetTrigger.Reset();
                StartTrigger.Reset();
                StopTrigger.Reset();
                ResetTrigger.Reset();
                DeinitializeTrigger.Reset();
            }

            protected override Task OnInitialize(NodeConfigurationMock configuration, CancellationToken cancellationToken)
            {
                return HandleBehavior(InitializeBehavior, InitializeTrigger, cancellationToken, nameof(INode.Initialize));
            }

            protected override Task OnStart(CancellationToken cancellationToken)
            {
                return HandleBehavior(StartBehavior, StartTrigger, cancellationToken, nameof(INode.Start));
            }

            protected override Task OnStop(CancellationToken cancellationToken)
            {
                return HandleBehavior(StopBehavior, StopTrigger, cancellationToken, nameof(INode.Stop));
            }

            protected override Task OnDeinitialize(CancellationToken cancellationToken)
            {
                return HandleBehavior(DeinitializeBehavior, DeinitializeTrigger, cancellationToken, nameof(INode.Deinitialize));
            }

            protected override Task OnReset(CancellationToken cancellationToken)
            {
                return HandleBehavior(ResetBehavior, ResetTrigger, cancellationToken, ResetName);
            }

            protected override Task OnInitializeReset(CancellationToken cancellationToken)
            {
                return HandleBehavior(InitializeResetBehavior, InitializeResetTrigger, cancellationToken, InitializeResetName);
            }

            public Task LetFail(CancellationToken cancellationToken)
            {
                return Fail(cancellationToken);
            }

            /// <summary>
            /// Handles the current call based on the configured behavior
            /// </summary>
            /// <param name="behavior">method behavior</param>
            /// <param name="resetEvent">related reset event</param>
            /// <param name="cancellationToken">cancellation token</param>
            /// <param name="callerName">caller name</param>
            private Task HandleBehavior(MockBehavior behavior, ManualResetEvent resetEvent, CancellationToken cancellationToken, string callerName)
            {
                // Add this call to the list
                CallList.Add(callerName);

                // Trigger event
                resetEvent.Set();

                // Handle the behavior
                switch (behavior)
                {
                    case MockBehavior.DoNothing:
                        return Task.CompletedTask;
                    case MockBehavior.ThrowException:
                        throw new NotSupportedException("based on behavior");
                    case MockBehavior.WaitForCancel:
                        return Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
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
