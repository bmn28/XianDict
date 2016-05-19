using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SQLiteNetExtensionsAsync.Extensions;
using SQLite.Net.Async;
using SQLite.Net;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System.Text.RegularExpressions;

namespace XianDict
{
    public class Cedict : Dict
    {
        public Cedict(SQLiteAsyncConnection db) : base(db, "CC-CEDICT", "cedict", "CC") { }

        public override void AddToIndex()
        {
            var task = db.Table<CedictEntry>().ToListAsync();
            var entries = task.Result;
            var indices = new List<IndexedTerm>();

            foreach (var entry in entries)
            {
                IndexedTerm index = new IndexedTerm()
                {
                    Traditional = entry.Traditional,
                    Simplified = entry.Simplified,
                    Pinyin = Pinyin.ConvertToAccents(entry.Pinyin),
                    PinyinNumbered = entry.Pinyin,
                    PinyinNoNumbers = Pinyin.RemoveNumbersAndUnderscore(entry.Pinyin),
                    Length = entry.Traditional.Length,
                    TableName = "CedictEntry",
                    TableId = entry.Id
                };
                indices.Add(index);
            }
            db.InsertAllAsync(indices).Wait();
        }

        public override void Build()
        {
            db.DropTableAsync<CedictEntry>();
            db.DropTableAsync<CedictDefinition>();
            db.CreateTableAsync<CedictEntry>();
            db.CreateTableAsync<CedictDefinition>();
            Regex rx = new Regex(@"([^ ]+) ([^ ]+) \[([^]]+)] /(.+)/$");
            List<CedictEntry> entries = new List<CedictEntry>();
            List<CedictDefinition> definitions = new List<CedictDefinition>();
            foreach (string line in System.IO.File.ReadLines("cedict_ts.u8"))
            {
                Match match = rx.Match(line);
                if (match.Success)
                {
                    CedictEntry entry = new CedictEntry()
                    {
                        Traditional = match.Groups[1].Value,
                        Simplified = match.Groups[2].Value,
                        Pinyin = match.Groups[3].Value
                    };
                    entry.Definitions = new List<CedictDefinition>(match.Groups[4].Value.Split(new char[] { '/' })
                            .Select(p => new CedictDefinition() { Definition = p, Entry = entry }));
                    entries.Add(entry);
                }
            }
            db.InsertAllAsync(entries);
            foreach (var e in entries)
            {
                foreach (var d in e.Definitions)
                {
                    d.EntryId = e.Id;
                    definitions.Add(d);
                }
            }
            db.InsertAllAsync(definitions);
        }

        public override async Task<IEnumerable<SearchResult>> Search(CancellationToken ct, string query)
        {
            var results = new List<SearchResult>();
            //try
            {
                //var entries = await db.Table<CedictEntry>().Where(p => p.Traditional.StartsWith(query)).ToListAsync();
                var entries = await db.QueryAsync<CedictEntry>(ct, "SELECT * FROM CedictEntry WHERE Traditional LIKE ? ESCAPE '\\'", query + "%");
                foreach (var q in Pinyin.ToQueryForms(query))
                {
                    var newEntries = await db.QueryAsync<CedictEntry>(ct,
                        "SELECT * FROM (SELECT * FROM CedictEntry WHERE PinyinNoNumbers LIKE ? ESCAPE '\\') " 
                        + "WHERE Pinyin LIKE ? ESCAPE '\\'", Pinyin.RemoveNumbersAndUnderscore(q) + "%", q + "%");
                    entries.AddRange(newEntries);

                }


                foreach (var s in entries)
                {
                    var definitions = await db.QueryAsync<CedictDefinition>(ct, "SELECT * FROM CedictDefinition WHERE EntryId = ?", s.Id);
                    //var definitions = await db.Table<CedictDefinition>().Where(d => d.EntryId == s.Id).ToListAsync();
                    results.Add(new SearchResult()
                    {
                        Traditional = s.Traditional,
                        Simplified = s.Simplified,
                        Pinyin = Pinyin.ConvertToAccents(s.Pinyin),
                        PinyinNumbered = s.Pinyin,
                        Definitions = new List<List<string>>() { new List<string>(from d in definitions select d.Definition) }
                    });
                }
            }

            return results;
        }
    }

    public class CedictEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public string Traditional { get; set; }
        [Indexed]
        public string Simplified { get; set; }
        [Indexed]
        public string Pinyin { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead)]
        public List<CedictDefinition> Definitions { get; set; }
    }

    public class CedictDefinition
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(CedictEntry))]
        public int EntryId { get; set; }
        [ManyToOne]
        public CedictEntry Entry { get; set; }
        public string Definition { get; set; }
    }
}
