using OctoPatch.Exchange;

namespace OctoPatch
{
    /// <summary>
    /// Interface for the central grid runtime
    /// </summary>
    public interface IEngine
    {
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
    }
}
