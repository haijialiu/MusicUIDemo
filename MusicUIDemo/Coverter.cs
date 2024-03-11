using Microsoft.UI.Xaml.Data;
using MusicUIDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return ((MusicItem)value).AlbumImage;
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
            return ((MusicItem)value).Title;
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
            return ((MusicItem)value).Artist;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }    
    public class MusicItemToMaxTimeSeconds : ICoverterBase
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var time = ((MusicItem)value).Time;

            return (double)time.Hour * 3600 + time.Minute * 60 + time.Second;
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
            return ((MusicPlayer)value).PlayedSeconds;
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
            return ((MusicItem)value).Time.ToLongTimeString();
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

}
