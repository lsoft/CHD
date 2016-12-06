using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace CHD.Client.AssemblyResources
{
    internal static class ImageResources
    {
        //public static Uri Down = new Uri("pack://application:,,,/CHD.Client.AssemblyResources.down.png", UriKind.RelativeOrAbsolute);
        //public static Uri GreenCircle = new Uri("pack://application:,,,/CHD.Client.AssemblyResources.green_circle.png", UriKind.RelativeOrAbsolute);
        //public static Uri RedCircle = new Uri("pack://application:,,,/CHD.Client.AssemblyResources.red_circle.png", UriKind.RelativeOrAbsolute);
        //public static Uri Up = new Uri("pack://application:,,,/CHD.Client;component/AssemblyResources/up.png", UriKind.RelativeOrAbsolute);

        public static BitmapImage DownImage;
        public static BitmapImage GreenCircleImage;
        public static BitmapImage RedCircleImage;
        public static BitmapImage UpImage;
        public static BitmapImage DeleteImage;
        public static BitmapImage PauseImage;
        public static BitmapImage YeahImage;
        public static BitmapImage GoonImage;

        static ImageResources()
        {
            var ea = Assembly.GetExecutingAssembly();

            DownImage = new BitmapImage();
            DownImage.BeginInit();
            DownImage.StreamSource = ea.GetManifestResourceStream("CHD.Client.AssemblyResources.down.png");
            DownImage.EndInit();

            GreenCircleImage = new BitmapImage();
            GreenCircleImage.BeginInit();
            GreenCircleImage.StreamSource = ea.GetManifestResourceStream("CHD.Client.AssemblyResources.green_circle.png");
            GreenCircleImage.EndInit();

            RedCircleImage = new BitmapImage();
            RedCircleImage.BeginInit();
            RedCircleImage.StreamSource = ea.GetManifestResourceStream("CHD.Client.AssemblyResources.red_circle.png");
            RedCircleImage.EndInit();

            UpImage = new BitmapImage();
            UpImage.BeginInit();
            UpImage.StreamSource = ea.GetManifestResourceStream("CHD.Client.AssemblyResources.up.png");
            UpImage.EndInit();

            DeleteImage = new BitmapImage();
            DeleteImage.BeginInit();
            DeleteImage.StreamSource = ea.GetManifestResourceStream("CHD.Client.AssemblyResources.DeleteItem.png");
            DeleteImage.EndInit();

            PauseImage = new BitmapImage();
            PauseImage.BeginInit();
            PauseImage.StreamSource = ea.GetManifestResourceStream("CHD.Client.AssemblyResources.pause.png");
            PauseImage.EndInit();

            YeahImage = new BitmapImage();
            YeahImage.BeginInit();
            YeahImage.StreamSource = ea.GetManifestResourceStream("CHD.Client.AssemblyResources.yeah.png");
            YeahImage.EndInit();

            GoonImage = new BitmapImage();
            GoonImage.BeginInit();
            GoonImage.StreamSource = ea.GetManifestResourceStream("CHD.Client.AssemblyResources.goon.png");
            GoonImage.EndInit();
        }
    }
}
