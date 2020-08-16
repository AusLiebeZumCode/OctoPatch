using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace OctoPatch.Test
{
    /// <summary>
    /// Collection of tests around the Engine
    /// </summary>
    public sealed class EngineTest
    {
        #region Engine lifecycle

        /// <summary>
        /// Tests the proper state switch on start and stop
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task StateChangeOnEmptyEngine()
        {
            var repository = new Mock<IRepository>();

            // Setup
            IEngine engine = new Engine(repository.Object);
            Assert.Equal(EngineState.Stopped, engine.State);

            // Test 1: Execute Stop -> no change
            await engine.Stop(CancellationToken.None);
            Assert.Equal(EngineState.Stopped, engine.State);

            // Test 2: Start
            await engine.Start(CancellationToken.None);
            Assert.Equal(EngineState.Running, engine.State);

            // Test 3: Start again -> no change
            await engine.Start(CancellationToken.None);
            Assert.Equal(EngineState.Running, engine.State);

            // Test 4: Stop
            await engine.Stop(CancellationToken.None);
            Assert.Equal(EngineState.Stopped, engine.State);
        }

        #endregion

        #region Node management

        #endregion
    }
}
