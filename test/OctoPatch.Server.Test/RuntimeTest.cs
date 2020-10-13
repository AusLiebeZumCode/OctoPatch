using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using OctoPatch.Descriptions;
using Xunit;

namespace OctoPatch.Server.Test
{
    /// <summary>
    /// Tests around the runtime class
    /// </summary>
    public sealed class RuntimeTest
    {
        private readonly Mock<IRepository> _repositoryMock;

        private readonly Mock<IPatch> _patchMock;

        private readonly Runtime _runtime;

        public RuntimeTest()
        {
            _repositoryMock = new Mock<IRepository>();
            _patchMock = new Mock<IPatch>();
            _runtime = new Runtime(_repositoryMock.Object, _patchMock.Object);
        }

        #region GetNodeDescriptions

        /// <summary>
        /// Calls the method and expects the same result than delivered from mock
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetNodeDescriptions_Success()
        {
            // Setup
            _repositoryMock.Setup(m => m.GetNodeDescriptions()).Returns(Enumerable.Empty<NodeDescription>()).Callback(() => { });

            // Act
            var result = await _runtime.GetNodeDescriptions(CancellationToken.None);

            // Assert
            Assert.Equal(0, result.Count());
        }



        #endregion
    }
}
