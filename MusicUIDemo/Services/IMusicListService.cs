using MusicUIDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUIDemo.Services
{
    /// <summary>
    /// 播放列表相关的服务
    /// </summary>
    public interface IMusicListService
    {
        /// <summary>
        /// 获取全部播放列表
        /// </summary>
        /// <returns>全部播放列表（包括用户和系统的）</returns>
        Task<List<MusicList>> GetAllMusicListsAsync();

        //Task<Music> GetMusicByIdAsync(int id);
        /// <summary>
        /// 创建播放列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task CreateMusicListAsync(MusicList list);
        /// <summary>
        /// 删除播放列表
        /// </summary>
        /// <param name="id">播放列表Id</param>
        /// <returns></returns>
        Task DeleteMusicListAsync(MusicList list);

    }
}
