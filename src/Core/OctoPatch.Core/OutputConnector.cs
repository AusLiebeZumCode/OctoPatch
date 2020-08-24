using System;

namespace OctoPatch.Core
{
    public sealed class OutputConnector : IOutputConnector
    {
        public Guid Guid { get; }

        public OutputConnector(Guid guid)
        {
            Guid = guid;
        }

        public void SendEmpty()
        {

        }

        public void SendInteger(int value)
        {

        }

        public void SendFloat(float value)
        {

        }

        public void SendBool(bool value)
        {

        }

        public void SendString(string value)
        {

        }

        public void SendBinary(byte[] value)
        {

        }

        public void SendComplex<T>(T value)
        {

        }
    }
}
