using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SQLite.Net;
using SQLite.Net.Attributes;
using Newtonsoft.Json;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;
using System.Text.RegularExpressions;

namespace XianDict
{
    public class DictionaryEngine
    {
        private SQLiteConnection db;
        private ICollection<Dict> dictionaries;

        public DictionaryEngine()
        {
            dictionaries = new List<Dict>();
            dictionaries.Add(new Cedict());
            dictionaries.Add(new Moedict());

            bool buildDicts = !File.Exists("dict.sqlite");
            db = new SQLiteConnection(new SQLite.Net.Platform.Generic.SQLitePlatformGeneric(), "dict.sqlite");
            //if (buildDicts)
            {
                foreach (var d in dictionaries)
                {
                    d.Build(db);
                }
            }

            var cedictEntries = db.Table<CedictEntry>();
            var moedictEntries = db.Table<MoedictEntry>();
            var moedictHeteronyms = db.Table<MoedictHeteronym>();

            //var query = from c in cedictEntries
            //            join e in moedictEntries
        }

    }
}
