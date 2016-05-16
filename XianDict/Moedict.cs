using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using Newtonsoft.Json;
using SQLite.Net;
using SQLiteNetExtensions.Attributes;
using System.Text.RegularExpressions;

namespace XianDict
{
    public class Moedict : Dict
    {
        private static Regex extractAlternateHeadword = new Regex(@"([^（]*)（(.*)[）)]$");
        private static Regex extractAlternatePinyin = new Regex(@"([^（]*)（(.*)[）)](.*)");

        public Moedict() : base("MoEDict", "moedict", "MOE") { }

        public override void Build(SQLiteConnection db)
        {
            char[] space = new char[] { ' ' };
            db.DropTable<MoedictEntry>();
            db.DropTable<MoedictHeteronym>();
            db.DropTable<MoedictDefinition>();
            db.DropTable<MoedictQuote>();
            db.DropTable<MoedictExample>();
            db.DropTable<MoedictLink>();
            db.CreateTable<MoedictEntry>();
            db.CreateTable<MoedictHeteronym>();
            db.CreateTable<MoedictDefinition>();
            db.CreateTable<MoedictQuote>();
            db.CreateTable<MoedictExample>();
            db.CreateTable<MoedictLink>();

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
            db.InsertAll(entries);
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
            db.InsertAll(heteronyms);
            foreach (var h in heteronyms)
            {
                foreach (var d in h.Definitions)
                {
                    d.HeteronymId = h.Id;
                    definitions.Add(d);
                }
            }
            db.InsertAll(definitions);
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
            db.InsertAll(quotes);
            db.InsertAll(examples);
            db.InsertAll(links);
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
        public MoedictExample Entry { get; set; }
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
        public MoedictDefinition[] Definitions { get; set; }

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
