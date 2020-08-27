namespace OctoPatch
{
    public abstract class Adapter : IAdapter
    {
        public IInputConnector Input { get; }
        public IOutputConnector Output { get; }
    }
}
