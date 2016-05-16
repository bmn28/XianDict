using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;

namespace XianDict
{
    public abstract class Dict
    {
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Abbreviation { get; set; }

        public Dict(string title, string shortTitle, string abbreviation)
        {
            Title = title;
            ShortTitle = shortTitle;
            Abbreviation = abbreviation;
        }

        public abstract void Build(SQLiteConnection db);
    }
}
