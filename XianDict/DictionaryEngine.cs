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
using SQLite.Net.Async;
using System.Threading;
using System.Data;

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
                db.DropTableAsync<IndexedTerm>().Wait();
                db.CreateTableAsync<IndexedTerm>().Wait();

                foreach (var d in dictionaries)
                {
                    d.AddToIndex();
                }
            }

        }

        public async Task<IEnumerable<IndexedTerm>> Search(CancellationToken ct, string query)
        {
            var results = new List<IndexedTerm>();
            results.AddRange(await db.QueryAsync<IndexedTerm>(ct, "SELECT * FROM IndexedTerm WHERE Traditional LIKE ? ESCAPE '\\'", query + "%"));
            var queryForms = Pinyin.ToQueryForms(query);
            int numberOfForms = queryForms.Count();
            bool allowMultiCharacter = numberOfForms > 1 || (numberOfForms > 0 && queryForms.First().IndexOf(' ') != -1);
            foreach (var q in queryForms)
            {
                if (!string.IsNullOrWhiteSpace(q))
                {
                    if (allowMultiCharacter)
                    {
                        results.AddRange(await db.QueryAsync<IndexedTerm>(ct,
                            "SELECT * FROM (SELECT * FROM IndexedTerm WHERE PinyinNoNumbers LIKE ? ESCAPE '\\') "
                            + " WHERE PinyinNumbered LIKE ?", Pinyin.RemoveNumbersAndUnderscore(q) + "%", q + "%"));
                    }
                    else
                    {
                        results.AddRange(await db.QueryAsync<IndexedTerm>(ct,
                            "SELECT * FROM (SELECT * FROM IndexedTerm WHERE Length = 1 AND PinyinNoNumbers LIKE ? ESCAPE '\\') "
                            + " WHERE PinyinNumbered LIKE ?", Pinyin.RemoveNumbersAndUnderscore(q) + "%", q + "%"));
                    }
                }
            }
            return results.OrderBy(r => r.Length).ThenBy(r => r.PinyinNumbered);
        }

    //    public async Task<IEnumerable<SearchResult>> Search(CancellationToken ct, string query)
    //    {
    //        var map = new Dictionary<string, Dictionary<string, SearchResult>>();
    //        var results = new List<SearchResult>();
    //        var resultsBegin = new List<SearchResult>();
    //        //var resultsMiddle = new List<SearchResult>();

    //        foreach (var d in dictionaries)
    //        {
    //            foreach (var r in await d.Search(ct, query))
    //            {
    //                Dictionary<string, SearchResult> entry;
    //                if (map.TryGetValue(r.Traditional, out entry))
    //                {
    //                    SearchResult existingResult;
    //                    if (map[r.Traditional].TryGetValue(r.PinyinNumbered, out existingResult))
    //                    {
    //                        existingResult.Definitions.AddRange(r.Definitions);
    //                    }
    //                }
    //                else
    //                {
    //                    var newDict = new Dictionary<string, SearchResult>();
    //                    map[r.Traditional] = newDict;
    //                    newDict[r.PinyinNumbered] = r;
    //                }
    //            }
    //        }
    //        foreach (var e in map)
    //        {
    //            foreach (var h in e.Value)
    //            {
    //                //if (h.Value.Traditional.Equals(query))
    //                    results.Add(h.Value);
    //                //else if (h.Value.Traditional.StartsWith(query))
    //                //    resultsBegin.Add(h.Value);
    //                //else
    //                    //resultsMiddle.Add(h.Value);
    //            }
    //        }
    //        results.Sort();
    //        resultsBegin.Sort();
    //        //resultsMiddle.Sort();
    //        results.AddRange(resultsBegin);
    //        //results.AddRange(resultsMiddle);
    //        return results;
    //    }
    }


    public class IndexedTerm
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public string Traditional { get; set; }
        [Indexed]
        public string Simplified { get; set; }
        public string Pinyin { get; set; }
        [Indexed]
        public string PinyinNumbered { get; set; }
        [Indexed]
        public string PinyinNoNumbers { get; set; }
        [Indexed]
        public int Length { get; set; }
        public string TableName { get; set; }
        public int TableId { get; set; }
    }

}
