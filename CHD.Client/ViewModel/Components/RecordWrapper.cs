using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CHD.Client.Marker.History;

namespace CHD.Client.ViewModel.Components
{
    internal class RecordWrapper
    {
        private readonly Record _record;

        public string TimeString
        {
            get
            {
                return
                    _record.Time.ToString("yyyy.MM.dd HH:mm:ss.fff");
            }
        }


        public string Success
        {
            get
            {
                return
                    _record.Success.ToString();
            }
        }

        public string Taken
        {
            get
            {
                return
                    _record.Taken.ToString();
            }
        }

        public string ExceptionInformation
        {
            get
            {
                return
                    _record.ExceptionInformation;
            }
        }

        public BitmapImage SuccessImage
        {
            get
            {
                var result =
                    _record.Success ? AssemblyResources.ImageResources.GreenCircleImage : AssemblyResources.ImageResources.RedCircleImage;

                return
                    result;
            }
        }

        public BitmapImage TakenImage
        {
            get
            {
                var result =
                    _record.Taken ? AssemblyResources.ImageResources.UpImage : AssemblyResources.ImageResources.DownImage;

                return
                    result;
            }
        }

        public RecordWrapper(
            Record record
            )
        {
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }
            _record = record;
        }
    }
}
