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
        private static FrequencyComparer freqComparer = new FrequencyComparer();

        private SQLiteAsyncConnection db;
        private ICollection<Dict> dictionaries;

        public Dict this [string key]
        {
            get { return dictionaries.Where(d => d.ShortTitle.Equals(key)).First(); }
        }

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
                db.DropTableAsync<Term>().Wait();
                db.CreateTableAsync<Term>().Wait();

                Dictionary<Tuple<string, string>, Term> index = new Dictionary<Tuple<string, string>, Term>();

                foreach (var d in dictionaries)
                {
                    d.AddToIndex(index);
                }

                db.InsertAllAsync(index.Values).Wait();
            
                ReadFrequencies();
            }
        }

        public async Task<IEnumerable<Term>> Search(CancellationToken ct, string query)
        {
            var results = new List<TermWithFreq>();
            results.AddRange(await db.QueryAsync<TermWithFreq>(ct, "SELECT * FROM Term LEFT JOIN Frequency ON Simplified = Hanzi OR Traditional = Hanzi WHERE Traditional LIKE ? OR Simplified LIKE ? ESCAPE '\\'", query + "%", query + "%"));
            var queryForms = Pinyin.ToQueryForms(query);
            int numberOfForms = queryForms.Count();
            bool allowMultiCharacter = numberOfForms > 1 || (numberOfForms > 0 && queryForms.First().IndexOf(' ') != -1);
            string limitLength = allowMultiCharacter ? "" : "Length = 1 AND ";
            foreach (var q in queryForms)
            {
                if (!string.IsNullOrWhiteSpace(q))
                {

                    results.AddRange(await db.QueryAsync<TermWithFreq>(ct,
                        "SELECT * FROM (SELECT * FROM (SELECT * FROM Term WHERE " + limitLength + "PinyinNoNumbers LIKE ? ESCAPE '\\') "
                        + " WHERE PinyinNumbered LIKE ?) LEFT JOIN Frequency ON Simplified = Hanzi OR Traditional = Hanzi", Pinyin.RemoveNumbersAndUnderscore(q) + "%", q + "%"));

                }
            }
            return results.OrderBy(r => r.Length).ThenBy(r => r.PinyinNumbered.Length).ThenByDescending(r => r.Score, freqComparer).ThenBy(r => r.PinyinNumbered);
        }

        public async Task<IEnumerable<Term>> SearchExact(CancellationToken ct, string query)
        {
            return (await db.QueryAsync<TermWithFreq>(ct, "SELECT * FROM Term LEFT JOIN Frequency ON Simplified = Hanzi OR Traditional = Hanzi WHERE Traditional = ? OR Simplified = ?", query, query)).OrderBy(r => r.Id);
        }

        private void ReadFrequencies()
        {
            db.CreateTableAsync<Frequency>().Wait();
            var frequencies = new List<Frequency>();
            var space = new char[] { ' ' };
            foreach (string line in File.ReadLines("rawdict_utf16_65105_freq.txt"))
            {
                var tokens = line.Split(space);
                frequencies.Add(new Frequency()
                {
                    Hanzi = tokens[0],
                    Score = float.Parse(tokens[1])
                });
            }
            db.InsertAllAsync(frequencies).Wait();
        }
    }

    public class FrequencyComparer : IComparer<float?>
    {
        public int Compare(float? x, float? y)
        {
            if (x == null && y == null)
                return 0;
            else if (x == null)
                return -1;
            else if (y == null)
                return 1;
            else
                return ((float)x).CompareTo((float)y);
        }
    }

    public class Term
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
        [ForeignKey(typeof(CedictEntry))]
        public int CedictEntryId { get; set; }
        [OneToOne]
        public CedictEntry CedictEntry { get; set; }
        [ForeignKey(typeof(MoedictHeteronym))]
        public int MoedictHeteronymId { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public MoedictHeteronym MoedictHeteronym { get; set; }
    }
    public class TermWithFreq : Term
    {
        public float? Score { get; set; }
    }

}
