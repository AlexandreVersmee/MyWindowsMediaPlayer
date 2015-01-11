using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWindowsMediaPlayer
{
    public class Media
    {
        public string path { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duree { get; set; }
        public long SizeDoc { get; set; }
        public EType Type { get; set; }
        public string FileName { get; set; }


        public Media(string path, string album, string titre, TimeSpan duration, string artist, long size, DateTime creat, string filename, EType type)
        {
            this.FileName = filename;
            this.path = path;
            this.Album = album;
            this.Title = titre;
            this.Duree = duration;
            this.Artist = artist;
            this.SizeDoc = size;
            this.Type = type;
            this.Date = creat;
        }
        public Media() { }

    }
}
