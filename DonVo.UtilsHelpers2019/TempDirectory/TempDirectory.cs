using System;

namespace TempDirectory
{
    public delegate void OnDispose();

    public class TempDirectory : IDisposable
    {
        private readonly OnDispose _onDispose;

        public TempDirectory(string name, string fullName, OnDispose onDispose)
        {
            Name = name;
            FullName = fullName;
            _onDispose = onDispose;
        }

        public string Name { get; }

        public string FullName { get; }

        public void Dispose()
        {
            _onDispose();
        }
    }
}
