using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System.Text.RegularExpressions;

namespace XianDict
{
    public class Cedict : Dict
    {
        public Cedict() : base("CC-CEDICT", "cedict", "CC") { }

        public override void Build(SQLiteConnection db)
        {
            db.DropTable<CedictEntry>();
            db.DropTable<CedictDefinition>();
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
