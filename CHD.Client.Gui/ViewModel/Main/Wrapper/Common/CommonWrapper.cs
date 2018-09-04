using System;

namespace CHD.Client.Gui.ViewModel.Main.Wrapper.Common
{
    public sealed class CommonWrapper
    {
        public string WatchFolder
        {
            get;
            private set;
        }

        public CommonWrapper()
        {
            WatchFolder = string.Empty;
        }

        public CommonWrapper(string watchFolder)
        {
            if (watchFolder == null)
            {
                throw new ArgumentNullException("watchFolder");
            }

            WatchFolder = watchFolder;
        }

        public bool ChangesExists(CommonWrapper commonData)
        {
            if (commonData == null)
            {
                throw new ArgumentNullException("commonData");
            }

            var result = string.Compare(this.WatchFolder, commonData.WatchFolder) != 0;

            return
                result;
        }
    }
}