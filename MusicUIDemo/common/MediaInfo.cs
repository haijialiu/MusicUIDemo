using Microsoft.UI.Xaml.Media.Imaging;
using MusicUIDemo.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MusicUIDemo.common
{
    public class MediaInfo
    {
        //TODO: 回头路径记得改回来
        [DllImport(@"C:\Users\89422\source\repos\MediaInfoDLL\x64\Debug\MediaInfoDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int media_load(string url);
        [DllImport(@"C:\Users\89422\source\repos\MediaInfoDLL\x64\Debug\MediaInfoDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr media_title();
        [DllImport(@"C:\Users\89422\source\repos\MediaInfoDLL\x64\Debug\MediaInfoDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr media_artist();        
        [DllImport(@"C:\Users\89422\source\repos\MediaInfoDLL\x64\Debug\MediaInfoDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr media_album();
        [DllImport(@"C:\Users\89422\source\repos\MediaInfoDLL\x64\Debug\MediaInfoDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern AlbumImageInfo media_album_png();
        [DllImport(@"C:\Users\89422\source\repos\MediaInfoDLL\x64\Debug\MediaInfoDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void media_free();
        [DllImport(@"C:\Users\89422\source\repos\MediaInfoDLL\x64\Debug\MediaInfoDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern MediaInfos get_music_info();
        
        public static Music GetMusic(string filePath)
        {
            if (!File.Exists(filePath)) { throw new FileNotFoundException(); }
            int ret = media_load(filePath);
            var title = Marshal.PtrToStringAnsi(media_title());
            Music music = new(title ?? Path.GetFileNameWithoutExtension(filePath),filePath);

            if(ret >= 0)
            {
                MediaInfos infos = get_music_info();
                music.Artist = Marshal.PtrToStringAnsi(infos.artist);
                music.Album = Marshal.PtrToStringAnsi(infos.album);

                music.Time = infos.total_time;
                var albumData = infos.album_info;

                if (albumData.ImageSize > 0)
                {
                    byte[] buffer = new byte[albumData.ImageSize];
                    Marshal.Copy(albumData.Image, buffer, 0, (int)albumData.ImageSize);
                    music.AlbumImage = buffer;
                }
                else
                {
                    music.AlbumImage = null;
                }
            }
            media_free();
            return music;
        }

   

    }
    public struct AlbumImageInfo
    {

        public UInt32 ImageSize;
        public IntPtr Image;
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct MediaInfos
    {
        [FieldOffset(0)] public IntPtr title;
        [FieldOffset(8)] public IntPtr artist;
        [FieldOffset(16)] public IntPtr album;
        [FieldOffset(24)] public int total_time;
        [FieldOffset(32)] public AlbumImageInfo album_info;
    }
}
