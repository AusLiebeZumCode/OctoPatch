using System;

namespace OctoPatch.Core
{
    public sealed class OutputConnector<T> : IOutputConnector, IObservable<T> where T : struct
    {
        public Guid Guid { get; }

        private readonly IObservable<T> _subject;

        public OutputConnector(IObservable<T> subject, Guid guid)
        {
            Guid = guid;
            _subject = subject;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}
