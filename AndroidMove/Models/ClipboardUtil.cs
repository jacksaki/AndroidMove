using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AndroidMove.Models {
    internal class ClipboardUtil {
        internal static void CopyImage(string path, int scalePercent) {
            var img = new BitmapImage(new Uri(path));
            var hoge = new BitmapImage();
            hoge.BeginInit();
            hoge.UriSource = new Uri(path);
            hoge.DecodePixelHeight=img.PixelHeight * scalePercent / 100;
            hoge.DecodePixelWidth=img.PixelWidth * scalePercent / 100;
            hoge.EndInit();
            Clipboard.SetImage(hoge);
        }
    }
}
