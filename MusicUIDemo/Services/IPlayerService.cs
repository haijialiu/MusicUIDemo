using MusicUIDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUIDemo.Services
{

    /// <summary>
    /// 播放器服务
    /// </summary>
    public interface IPlayerService
    {
        /// <summary>
        /// 播放
        /// </summary>
        void ResumeMusic();
        /// <summary>
        /// 暂停
        /// </summary>
        void PauseMusic();
        /// <summary>
        /// 替换播放列表
        /// </summary>
        /// <param name="musics">要替换的列表</param>
        void ReplacePlayList(List<Music> musics);
        /// <summary>
        /// 上一首歌
        /// </summary>
        void PreviousMusic();
        /// <summary>
        /// 下一首歌
        /// </summary>
        void NextMusic();
        /// <summary>
        /// 停止播放
        /// </summary>
        void StopMusic();
        /// <summary>
        /// 切换播放模式
        /// </summary>
        /// <param name="mode"></param>
        void SetPlayMode(int mode);
        /// <summary>
        /// 获取播放模式
        /// </summary>
        /// <returns>0:单曲循环，1:顺序播放，2:随机播放</returns>
        int GetPlayMode();
        /// <summary>
        /// 获取当前歌曲播放的时间
        /// </summary>
        /// <returns>已播放的秒数</returns>
        double GetPlayedTime();
        /// <summary>
        /// 获取当前歌曲总时长
        /// </summary>
        /// <returns>当前歌曲总时长的秒数</returns>
        double GetTotalTime();
            
    }
}
