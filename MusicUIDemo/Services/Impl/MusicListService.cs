using Microsoft.EntityFrameworkCore;
using MusicUIDemo.Models;
using MusicUIDemo.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUIDemo.Services.Impl
{
    public sealed class MusicListService : IMusicListService
    {
        private readonly DataContext context = new();
        public async Task CreateMusicListAsync(MusicList list)
        {
            context.MusicList.Add(list);
            await context.SaveChangesAsync();
        }

        public async Task DeleteMusicListAsync(MusicList musicList)
        {
            context.MusicList.Remove(musicList);
            await context.SaveChangesAsync();
        }

        public async Task<List<MusicList>> GetAllMusicListsAsync()
        {
            return await context.MusicList.Include(list => list.Musics).ToListAsync();
        }

        //public Task<Music> GetMusicByIdAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
