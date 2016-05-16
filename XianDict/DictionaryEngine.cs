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

        public DictionaryEngine()
        {
            bool buildDicts = File.Exists("dict.sqlite");
            db = new SQLiteConnection(new SQLite.Net.Platform.Generic.SQLitePlatformGeneric(), "dict.sqlite");
            if (buildDicts)
            {
                BuildCedict();
                BuildMoedict();
            }
        }

        private void BuildCedict()
        {
            db.CreateTable<CedictEntry>();
            db.CreateTable<CedictDefinition>();
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
            db.InsertAll(entries);
            foreach (var e in entries)
            {
                foreach (var d in e.Definitions)
                {
                    d.EntryId = e.Id;
                    definitions.Add(d);
                }
            }
            db.InsertAll(definitions);
        }

        private void BuildMoedict()
        {
            char[] space = new char[] { ' ' };

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
                entry.Headword = entry.Title.Replace("`", "").Replace("~", "");
                entries.Add(entry);
            }
            db.InsertAll(entries);
            foreach (var e in entries)
            {
                foreach (var h in e.Heteronyms)
                {
                    h.EntryId = e.Id;
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
}
