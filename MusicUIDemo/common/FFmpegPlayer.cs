using MusicUIDemo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MusicUIDemo.common
{
    public class FFmpegPlayer
    {
        [DllImport(@"C:\Users\89422\source\repos\MusicUIDemo\x64\Debug\FFmpegAudio.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int create_player();
        [DllImport(@"C:\Users\89422\source\repos\MusicUIDemo\x64\Debug\FFmpegAudio.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int operate(string action,string value);        
        [DllImport(@"C:\Users\89422\source\repos\MusicUIDemo\x64\Debug\FFmpegAudio.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int input_music_urls(string[] music_urls,int num);
        [DllImport(@"C:\Users\89422\source\repos\MusicUIDemo\x64\Debug\FFmpegAudio.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int destroy_player();
        [DllImport(@"C:\Users\89422\source\repos\MusicUIDemo\x64\Debug\FFmpegAudio.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern double share_play_time();        
        [DllImport(@"C:\Users\89422\source\repos\MusicUIDemo\x64\Debug\FFmpegAudio.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int share_play_index();        
        [DllImport(@"C:\Users\89422\source\repos\MusicUIDemo\x64\Debug\FFmpegAudio.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool share_play_status();
        
        public static int CurrentPlayIndex()
        {
            return share_play_index();
        }
        public static bool GetPlayStatus()
        {
            return share_play_status();
        }
        public static long PlayedTime()
        {

            var seconds = share_play_time();
            if (seconds > 0)
            {
                return (long)seconds;
            }
            return 0;

        }

        //返回值记得之后回来改
        public static void InitPlayer()
        {
            _ = create_player();
        }
        public static void InputMusic(List<Music> musics)
        {
            List<string> musicUrls = [];
            foreach (var item in musics)
            {
                musicUrls.Add(item.FilePath);
            }
            _ =  input_music_urls([.. musicUrls], musicUrls.Count);

        }   
        public static void OperatePlayer(string action,string value)
        {
            _ = operate(action, value);
        }
        public static void DestroyPlayer()
        {
            _ = destroy_player();
        }
    }
}
