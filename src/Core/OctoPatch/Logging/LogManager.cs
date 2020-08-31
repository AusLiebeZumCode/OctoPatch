using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace OctoPatch.Logging
{
    public static class LogManager
    {
        private static readonly ILoggerFactory nullLoggerFactory = new NullLoggerFactory();
        private static ILoggerFactory loggerFactory;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <returns></returns>
        public static ILogger<TContext> GetLogger<TContext>()
        {
            return (loggerFactory ?? nullLoggerFactory).CreateLogger<TContext>();
        }

        /// <summary>
        /// Sets the logger factory.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <exception cref="ArgumentNullException">loggerFactory</exception>
        public static void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            LogManager.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }
    }
}
