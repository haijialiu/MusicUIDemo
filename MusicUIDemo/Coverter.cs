using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using MusicUIDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace MusicUIDemo
{
    public interface ICoverterBase : IValueConverter
    {

    }
    public class PlayStatusToStringIcon : ICoverterBase
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool sourceValue = (bool)value;

            if (sourceValue)
            {
                return "\uE769";
            }
            else
            {
                return "\uE768";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string sourceValue = (string)value;
            if (sourceValue == "\uE769") return true;
            else if (sourceValue == "\uE768") return false;
            else
                throw new ArgumentException("输入有误");
        }
    }
    public class MusicItemToAlbumImage : ICoverterBase
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            var converter = new ByteArrayToBitmapImage();
            return converter.Convert(((Music)value).AlbumImage, targetType, parameter, language);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class MusicItemToTitle : ICoverterBase
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((Music)value).Title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class MusicItemToArtist : ICoverterBase
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((Music)value).Artist;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }    
    public class MusicToMaxTimeSeconds : ICoverterBase
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var seconds = (double)((Music)value).Time;

            return seconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }    
    public class MusicPlayerGetCurrentSeconds : ICoverterBase
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((MusicPlayer)value).CurrentTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
  
    public class MusicItemToTotalTimeString : ICoverterBase
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var time = ((Music)value).Time;
            return $"{time / 60:00}:{time % 60:00}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class TimeOnlyToLongString : ICoverterBase
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((TimeOnly) value).ToLongTimeString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class ByteArrayToBitmapImage : ICoverterBase
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value ==null) return null;
            var buffer = (byte[])value;
            MemoryStream ms = new(buffer);
            BitmapImage bitmapImage = new();
            bitmapImage.SetSource(ms.AsRandomAccessStream());
            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class SecondsToString : ICoverterBase
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var seconds = (double)value;
            return string.Format("{0:00}:{1:00}", seconds / 60, seconds % 60);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
