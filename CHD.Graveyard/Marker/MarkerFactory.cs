using System;
using System.IO;

namespace CHD.Graveyard.Marker
{
    public class MarkerFactory : IMarkerFactory
    {
        private const string MarkerFilePath = @"Marker\$marker";

        private readonly object _locker = new object();


        public bool IsMarkerCreated
        {
            get
            {
                lock (_locker)
                {
                    return
                        IsMarkerExists();
                }
            }
        }

        public void CreateMarker()
        {
            lock (_locker)
            {
                CreateMarkerInternal();
            }
        }

        public void SafelyDeleteMarker()
        {
            lock (_locker)
            {
                SafelyDeleteMarkerInternal();
            }
        }

        private void CreateMarkerInternal()
        {
            var fi = new FileInfo(MarkerFilePath);

            #region create directory if not exists

            if (fi.Directory != null)
            {
                var di = fi.Directory.FullName;

                if (!Directory.Exists(di))
                {
                    Directory.CreateDirectory(di);
                }
            }

            #endregion

            if (IsMarkerExists())
            {
                throw new InvalidOperationException("Marker already saved");
            }

            File.WriteAllText(MarkerFilePath, string.Empty);
        }

        private bool SafelyDeleteMarkerInternal()
        {
            if (!IsMarkerExists())
            {
                return true;
            }

            File.Delete(MarkerFilePath);

            return
                true;
        }

        private bool IsMarkerExists(
            )
        {
            return
                File.Exists(MarkerFilePath);
        }

    }
}