using OctoPatch.ContentTypes;
using Xunit;

namespace OctoPatch.Test.ContentTypes
{
    /// <summary>
    /// Collection of tests around content type "all"
    /// </summary>
    public sealed class AllContentTypeTest
    {
        #region IsSupported

        /// <summary>
        /// Check if bool is rated as supported
        /// </summary>
        [Fact]
        public void IsSupportedType_bool_Success()
        {
            var contentType = new AllContentType();
            Assert.True(contentType.IsSupportedType<bool>());
            Assert.True(contentType.IsSupportedType(typeof(bool)));
        }

        /// <summary>
        /// Check if int is rated as supported
        /// </summary>
        [Fact]
        public void IsSupportedType_int_Success()
        {
            var contentType = new AllContentType();
            Assert.True(contentType.IsSupportedType<int>());
            Assert.True(contentType.IsSupportedType(typeof(int)));
        }

        /// <summary>
        /// Check if string is rated as supported
        /// </summary>
        [Fact]
        public void IsSupportedType_string_Success()
        {
            var contentType = new AllContentType();
            Assert.True(contentType.IsSupportedType<string>());
            Assert.True(contentType.IsSupportedType(typeof(string)));
        }

        /// <summary>
        /// Check if byte[] is rated as supported
        /// </summary>
        [Fact]
        public void IsSupportedType_binary_Success()
        {
            var contentType = new AllContentType();
            Assert.True(contentType.IsSupportedType<byte[]>());
            Assert.True(contentType.IsSupportedType(typeof(byte[])));
        }

        /// <summary>
        /// Check if a ref type is rated as supported
        /// </summary>
        [Fact]
        public void IsSupportedType_ref_Success()
        {
            var contentType = new AllContentType();
            Assert.True(contentType.IsSupportedType<AllContentType>());
            Assert.True(contentType.IsSupportedType(typeof(AllContentType)));
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Test behavior of normalize data
        /// </summary>
        /// <param name="input">input value</param>
        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(+1)]
        [InlineData(int.MaxValue)]
        public void Normalize_int_SameAsInput(int input)
        {
            // Setup
            var contentType = new AllContentType();

            // Act
            var output = contentType.NormalizeValue(input);

            // Assert
            Assert.Equal(input, output);
        }

        #endregion
    }
}
