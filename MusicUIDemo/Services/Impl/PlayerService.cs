using MusicUIDemo.common;
using MusicUIDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUIDemo.Services.Impl
{
    public sealed class PlayerService : IPlayerService
    {
        public double GetPlayedTime()
        {
            return FFmpegPlayer.PlayedTime();
        }

        public int GetPlayMode()
        {
            throw new NotImplementedException();
        }

        public double GetTotalTime()
        {
            throw new NotImplementedException();
        }

        public void NextMusic()
        {
            throw new NotImplementedException();
        }

        public void PauseMusic()
        {
            FFmpegPlayer.OperatePlayer("pause", "1");
        }

        public void PreviousMusic()
        {
            throw new NotImplementedException();
        }

        public void ReplacePlayList(List<Music> musics)
        {
            throw new NotImplementedException();
        }

        public void ResumeMusic()
        {
            FFmpegPlayer.OperatePlayer("resume", "1");
        }

        public void SetPlayMode(int mode)
        {
            FFmpegPlayer.OperatePlayer("play_mode", mode.ToString());
        }

        public void StopMusic()
        {
            throw new NotImplementedException();
        }
    }
}
