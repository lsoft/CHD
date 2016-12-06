using System;
using System.Threading;
using CHD.FileSystem.Watcher;

namespace CHD.Pull.Components
{
    public class ExcluderWrapper : IDisposable
    {
        private readonly IExcluder _excluder;
        private readonly string _filepath;

        private int _disposed = 0;

        public ExcluderWrapper(
            IExcluder excluder,
            string filepath
            )
        {
            if (excluder == null)
            {
                throw new ArgumentNullException("excluder");
            }
            if (filepath == null)
            {
                throw new ArgumentNullException("filepath");
            }

            _excluder = excluder;
            _filepath = filepath;

            excluder.AddToExcluded(filepath);
        }

        public void Dispose()
        {
            var disposed = Interlocked.Exchange(ref _disposed, 1);
            if (disposed == 0)
            {
                _excluder.RemoveFromExcluded(_filepath);
            }
        }
    }
}