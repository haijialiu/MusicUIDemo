using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using MusicUIDemo.common;
using MusicUIDemo.Models;
using MusicUIDemo.Models.Database;
using MusicUIDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MusicUIDemo.Models
{
    //播放器
    public class MusicPlayer : BindableBase
    {
        //播放列表
        public ObservableCollection<Music> PlayList { get; set; } = null;
        //当前播放歌曲的序号
        private int currentIndex = 0;

        public int CurrentPlayIndex
        {
            get => currentIndex;
            set
            {
                if (value < 0)
                {
                    if (PlayList.Count == 0)
                    {
                        value = 0;
                    }
                    else
                    {
                        value = PlayList.Count - 1;
                    }
                }
                else if (value >= PlayList.Count)
                {
                    value = 0;
                }
                SetProperty(ref currentIndex, value);
                //OnPropertyChanged(nameof(CurrentMusic));
                CurrentMusic = PlayList[value];
            }
        }
        private bool status = false;
        public bool PlayStatus 
        {
            get => status;
            set => SetProperty(ref status, value); 
        }
        //当前播放的音乐，其实就是PlayList[CurrentPlayIndex]
        private Music currentMusic;
        public Music CurrentMusic 
        {
            get => currentMusic;
            set => SetProperty(ref currentMusic, value);
        }

        //TODO: 音量调节(暂未实现）
        public int Volume { get; set; }
        //播放模式 0 单曲 1 顺序播放 2 列表循环
        private int mode = 1;
        public int PlayMode
        {
            get => mode;
            set => SetProperty(ref mode, value);
        }
        //当前播放时间
        private double currentTime=0;
        public double CurrentTime
        {
            get => currentTime;
            set => SetProperty(ref currentTime, value);
        }
        //时长
        private double totalTime;
        public double TotalTime
        {
            get => totalTime;
            set =>  SetProperty(ref totalTime, value);
        }

        static MusicPlayer()
        {
            FFmpegPlayer.InitPlayer();
        }
        public static void ReplacePlayList(List<Music> list)
        {
            FFmpegPlayer.InputMusic(list);
        }
        public static void Operate(string action, string value)
        {
           FFmpegPlayer.OperatePlayer(action, value);
        }
        public MusicPlayer ()
        {
           
        }

    }
}
