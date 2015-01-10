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
        public string album { get; set; }
        public string titre { get; set; }
        public string artist { get; set; }
        public DateTime creat { get; set; }
        public TimeSpan duration { get; set; }
        public long size { get; set; }
        public string filename { get; set; }

        public Media(string path, string album, string titre, TimeSpan duration, string artist, long size, DateTime creat, string filename)
        {
            this.filename = filename;
            this.path = path;
            this.album = album;
            this.titre = titre;
            this.duration = duration;
            this.artist = artist;
            this.size = size;
            this.creat = creat;
        }
        public Media() { }

    }
}
