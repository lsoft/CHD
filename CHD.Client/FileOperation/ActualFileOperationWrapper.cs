using System;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CHD.Wpf;

namespace CHD.Client.FileOperation
{
    internal class ActualFileOperationWrapper : BaseViewModel
    {
        private readonly IFileOperation _fileOperation;
        private readonly FileActionTypeEnum _type;

        public string FilePathSuffix
        {
            get
            {
                return
                    _fileOperation.FilePathSuffix.FilePathSuffix;
            }
        }

        public BitmapImage TypeImage
        {
            get
            {
                BitmapImage result = null;


                switch (_type)
                {
                    case FileActionTypeEnum.Upload:
                        result = AssemblyResources.ImageResources.UpImage;
                        break;
                    case FileActionTypeEnum.Delete:
                        result = AssemblyResources.ImageResources.DeleteImage;
                        break;
                    case FileActionTypeEnum.Download:
                        result = AssemblyResources.ImageResources.DownImage;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return
                    result;
            }
        }

        public int Progress
        {
            get
            {
                return
                    (int)(_fileOperation.Progress * 100);
            }
        }

        public BitmapImage WorkImage 
        {
            get
            {
                BitmapImage result = null;

                if (IsWorking)
                {
                    result = AssemblyResources.ImageResources.GoonImage;
                }
                else
                {
                    result = AssemblyResources.ImageResources.PauseImage;
                }

                return
                    result;
            }
        }

        public bool IsWorking
        {
            get;
            private set;
        }

        public ActualFileOperationWrapper(
            Dispatcher dispatcher,
            IFileOperation fileOperation,
            FileActionTypeEnum type
            ) : base(dispatcher)
        {
            if (fileOperation == null)
            {
                throw new ArgumentNullException("fileOperation");
            }

            _fileOperation = fileOperation;
            _type = type;

            fileOperation.ProgressChangedEvent += PusherOnProgressChanged;
        }

        public void SetIsWorking(bool isWorking)
        {
            IsWorking = isWorking;

            OnPropertyChanged(string.Empty);
        }

        protected override void DisposeViewModel()
        {
            _fileOperation.ProgressChangedEvent -= PusherOnProgressChanged;

            base.DisposeViewModel();
        }

        private void PusherOnProgressChanged(float progress)
        {
            OnPropertyChanged(string.Empty);
        }
    }
}