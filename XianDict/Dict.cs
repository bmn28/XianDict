using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SQLite.Net;
using SQLite.Net.Async;
using System.Data;

namespace XianDict
{
    public abstract class Dict
    {
        public SQLiteAsyncConnection db { get; set; }

        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Abbreviation { get; set; }

        public Dict(SQLiteAsyncConnection db, string title, string shortTitle, string abbreviation)
        {
            Title = title;
            ShortTitle = shortTitle;
            Abbreviation = abbreviation;
            this.db = db;
        }

        public abstract void Build();

        public abstract void AddToIndex();

        public abstract Task<IEnumerable<SearchResult>> Search(CancellationToken ct, string query);
    }

}
