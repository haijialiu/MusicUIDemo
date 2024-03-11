using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Drawing.Imaging;

using Microsoft.UI.Xaml.Controls;
using System.Runtime.CompilerServices;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;
using Microsoft.UI.Xaml.Media;
using MusicUIDemo.common;

namespace MusicUIDemo.Models
{
    public class MusicItem : BindableBase
    {
        private int? id;
        private string title;
        private string artist;
        private string album;
        private string filePath;
        public TimeOnly Time;
        private BitmapImage albumImage;
        public int? Id {
            get => id; 
            set => SetProperty(ref id, value);
        } 
        public string Title
        { 
            get => title;
            set => SetProperty(ref title, value);
        }
        public string Artist {
            get => artist;
            set => SetProperty(ref artist, value);
        }
        public string Album 
        { 
            get => album;
            set => SetProperty(ref album, value);
        }
        public BitmapImage AlbumImage
        { 
            get => albumImage;
            set => SetProperty(ref albumImage, value);
        } 
        public string FilePath
        {
            get => filePath;
            set => SetProperty(ref filePath, value);
        }
        public string TimeString
        {
            get => Time.ToLongTimeString();
            set
            {
                _ = TimeOnly.TryParse(value, out TimeOnly result);
                Time = result;
                OnPropertyChanged(nameof(Time));
            }
        }

    }
}
