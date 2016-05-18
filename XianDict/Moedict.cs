using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SQLite.Net.Attributes;
using Newtonsoft.Json;
using SQLite.Net;
using SQLite.Net.Async;
using SQLiteNetExtensionsAsync;
using System.Text.RegularExpressions;
using SQLiteNetExtensionsAsync.Extensions;
using SQLiteNetExtensions.Attributes;

namespace XianDict
{
    public class Moedict : Dict
    {
        private static Regex extractAlternateHeadword = new Regex(@"([^（]*)（(.*)[）)]$");
        private static Regex extractAlternatePinyin = new Regex(@"([^（\(]*)[（\(](.*)[）)](.*)");

        public Moedict(SQLiteAsyncConnection db) : base(db, "MoEDict", "moedict", "MOE") { }

        public override void Build()
        {
            char[] space = new char[] { ' ' };
            db.DropTableAsync<MoedictEntry>();
            db.DropTableAsync<MoedictHeteronym>();
            db.DropTableAsync<MoedictDefinition>();
            db.DropTableAsync<MoedictQuote>();
            db.DropTableAsync<MoedictExample>();
            db.DropTableAsync<MoedictLink>();
            db.CreateTableAsync<MoedictEntry>();
            db.CreateTableAsync<MoedictHeteronym>();
            db.CreateTableAsync<MoedictDefinition>();
            db.CreateTableAsync<MoedictQuote>();
            db.CreateTableAsync<MoedictExample>();
            db.CreateTableAsync<MoedictLink>();

            string[] lines = System.IO.File.ReadAllLines("a.txt");

            var entries = new List<MoedictEntry>();
            var heteronyms = new List<MoedictHeteronym>();
            var definitions = new List<MoedictDefinition>();
            var quotes = new List<MoedictQuote>();
            var examples = new List<MoedictExample>();
            var links = new List<MoedictLink>();


            foreach (string line in lines)
            {
                string[] tokens = line.Split(space, 3);
                MoedictEntry entry = JsonConvert.DeserializeObject<MoedictEntry>(tokens[2]);
                entry.Headword = entry.Title.Replace("`", "").Replace("~", "").Trim();
                Match match = extractAlternateHeadword.Match(entry.Headword);
                if (match.Success)
                {
                    entry.Headword = match.Groups[1].Value;
                    entry.AlternateHeadword = match.Groups[2].Value;
                }
                entries.Add(entry);
            }
            db.InsertAllAsync(entries);
            foreach (var e in entries)
            {
                foreach (var h in e.Heteronyms)
                {
                    h.EntryId = e.Id;
                    if (h.Pinyin != null)
                    {
                        Match match = extractAlternatePinyin.Match(h.Pinyin);
                        if (match.Success)
                        {
                            h.Pinyin = match.Groups[1].Value.Trim();
                            h.AlternatePinyinNote = match.Groups[2].Value;
                            h.AlternatePinyin = match.Groups[3].Value.Trim();
                        }
                        h.PinyinNumbered = Pinyin.ConvertToNumbers(h.Pinyin);
                        h.AlternatePinyinNumbered = Pinyin.ConvertToNumbers(h.AlternatePinyin);
                    }
                    heteronyms.Add(h);
                }
            }
            db.InsertAllAsync(heteronyms);
            foreach (var h in heteronyms)
            {
                foreach (var d in h.Definitions)
                {
                    d.HeteronymId = h.Id;
                    definitions.Add(d);
                }
            }
            db.InsertAllAsync(definitions);
            foreach (var d in definitions)
            {
                if (d.Quotes != null)
                {
                    foreach (var quote in d.Quotes)
                    {
                        quotes.Add(new MoedictQuote() { DefinitionId = d.Id, Quote = quote });
                    }
                }
                if (d.Examples != null)
                {
                    foreach (var example in d.Examples)
                    {
                        examples.Add(new MoedictExample() { DefinitionId = d.Id, Example = example });
                    }
                }
                if (d.Links != null)
                {
                    foreach (var link in d.Links)
                    {
                        links.Add(new MoedictLink() { DefinitionId = d.Id, Link = link });
                    }
                }
            }
            db.InsertAllAsync(quotes);
            db.InsertAllAsync(examples);
            db.InsertAllAsync(links);
        }

        public override async Task<IEnumerable<SearchResult>> Search(CancellationToken ct, string query)
        {
            List<SearchResult> results = new List<SearchResult>();
            //await db.QueryAsync(ct, "")

            foreach (var s in await db.Table<MoedictEntry>().Where(s => s.Headword.StartsWith(query)).ToListAsync()) {
                await db.GetChildrenAsync(s);
                if (s.Heteronyms != null)
                {
                    foreach (var h in s.Heteronyms)
                    {
                        await db.GetChildrenAsync(h);
                        results.Add(new SearchResult() { Traditional = s.Headword, Pinyin = h.Pinyin, PinyinNumbered = h.PinyinNumbered,
                            Definitions = new List<List<string>>() { new List<string>(from d in h.Definitions select d.Definition.Replace("`", "").Replace("~", "")) } });
                    }
                }
            }
            return results;
        }
    }
    public class MoedictEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public string Headword { get; set; }
        public string AlternateHeadword { get; set; }
        [JsonProperty("t")]
        public string Title { get; set; }
        [JsonProperty("r")]
        public string Radical { get; set; }
        [JsonProperty("c")]
        public int StrokeCount { get; set; }
        [JsonProperty("n")]
        public int NonRadicalStrokeCount { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead), JsonProperty("h")]
        public List<MoedictHeteronym> Heteronyms { get; set; }
    }

    public class MoedictHeteronym
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(MoedictEntry))]
        public int EntryId { get; set; }
        [ManyToOne]
        public MoedictEntry Entry { get; set; }
        [JsonProperty("p")]
        public string Pinyin { get; set; }
        public string AlternatePinyin { get; set; }
        public string AlternatePinyinNote { get; set; }
        [Indexed]
        public string PinyinNumbered { get; set; }
        [Indexed]
        public string AlternatePinyinNumbered { get; set; }
        [Indexed, JsonProperty("b")]
        public string Bopomofo { get; set; }
        [JsonProperty("=")]
        public string AudioId { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead), JsonProperty("d")]
        public List<MoedictDefinition> Definitions { get; set; }
    }

    public class MoedictDefinition
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(MoedictHeteronym))]
        public int HeteronymId { get; set; }
        [ManyToOne]
        public MoedictHeteronym Heteronym { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("f")]
        public string Definition { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead), JsonProperty("q")]
        public List<string> Quotes { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead), JsonProperty("l")]
        public List<string> Links { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead), JsonProperty("e")]
        public List<string> Examples { get; set; }
    }

    public class MoedictQuote
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(MoedictDefinition))]
        public int DefinitionId { get; set; }
        [ManyToOne]
        public MoedictDefinition Definition { get; set; }
        public string Quote { get; set; }
    }

    public class MoedictExample
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(MoedictDefinition))]
        public int DefinitionId { get; set; }
        [ManyToOne]
        public MoedictDefinition Definition { get; set; }
        public string Example { get; set; }
    }

    public class MoedictLink
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(MoedictDefinition))]
        public int DefinitionId { get; set; }
        [ManyToOne]
        public MoedictDefinition Definition { get; set; }
        public string Link { get; set; }
    }
}
