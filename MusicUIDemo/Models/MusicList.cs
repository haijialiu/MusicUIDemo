using MusicUIDemo.common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUIDemo.Models
{
    public class MusicList : BindableBase
    {
        public int Id { get; set; }
        public bool IsPlayList { get; set; } = false;
        private string listTitle;
        public string ListTitle
        {
            get => listTitle;
            set => SetProperty(ref listTitle, value);
        }
        public ObservableCollection<MusicItem> MusicItems { get; set; } = null;

    }
}
