﻿using System;
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
using SQLite.Net.Async;
using System.Threading;

namespace XianDict
{
    public class DictionaryEngine
    {
        private SQLiteAsyncConnection db;
        private ICollection<Dict> dictionaries;

        public DictionaryEngine()
        {
            db = new SQLiteAsyncConnection(() => new SQLiteConnectionWithLock(
                new SQLite.Net.Platform.Generic.SQLitePlatformGeneric(), 
                new SQLiteConnectionString("dict.sqlite", storeDateTimeAsTicks: false)));

            dictionaries = new List<Dict>();
            dictionaries.Add(new Cedict(db));
            dictionaries.Add(new Moedict(db));

            bool buildDicts = !File.Exists("dict.sqlite");
            if (buildDicts)
            {
                foreach (var d in dictionaries)
                {
                    d.Build();
                }
            }


            var cedictEntries = db.Table<CedictEntry>();
            var moedictEntries = db.Table<MoedictEntry>();
            var moedictHeteronyms = db.Table<MoedictHeteronym>();
            //var query = from c in cedictEntries
            //            join e in moedictEntries
        }


        public async Task<IEnumerable<SearchResult>> Search(CancellationToken ct, string query)
        {
            var map = new Dictionary<string, Dictionary<string, SearchResult>>();
            var results = new List<SearchResult>();
            var resultsBegin = new List<SearchResult>();
            //var resultsMiddle = new List<SearchResult>();

            foreach (var d in dictionaries)
            {
                foreach (var r in await d.Search(ct, query))
                {
                    Dictionary<string, SearchResult> entry;
                    if (map.TryGetValue(r.Traditional, out entry))
                    {
                        SearchResult existingResult;
                        if (map[r.Traditional].TryGetValue(r.PinyinNumbered, out existingResult))
                        {
                            existingResult.Definitions.AddRange(r.Definitions);
                        }
                    }
                    else
                    {
                        var newDict = new Dictionary<string, SearchResult>();
                        map[r.Traditional] = newDict;
                        newDict[r.PinyinNumbered] = r;
                    }
                }
            }
            foreach (var e in map)
            {
                foreach (var h in e.Value)
                {
                    //if (h.Value.Traditional.Equals(query))
                        results.Add(h.Value);
                    //else if (h.Value.Traditional.StartsWith(query))
                    //    resultsBegin.Add(h.Value);
                    //else
                        //resultsMiddle.Add(h.Value);
                }
            }
            results.Sort();
            resultsBegin.Sort();
            //resultsMiddle.Sort();
            results.AddRange(resultsBegin);
            //results.AddRange(resultsMiddle);
            return results;
        }
    }
}
