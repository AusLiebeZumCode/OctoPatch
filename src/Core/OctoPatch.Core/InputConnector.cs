using System;

namespace OctoPatch.Core
{
    public sealed class InputConnector<T> : IInputConnector, IObserver<T> where T : struct
    {
        public Guid Guid { get; }

        private readonly IObserver<T> _observer;

        public InputConnector(IObserver<T> observer, Guid guid)
        {
            Guid = guid;
            _observer = observer;
        }

        public void OnCompleted()
        {
            _observer.OnCompleted();
        }

        public void OnError(Exception exception)
        {
            _observer.OnError(exception);
        }

        public void OnNext(T value)
        {
            _observer.OnNext(value);
        }
    }
}
