using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using Newtonsoft.Json;
using SQLiteNetExtensions.Attributes;

namespace XianDict
{
    public class MoedictEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public string Headword { get; set; }
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
        [Indexed, JsonProperty("p")]
        public string Pinyin { get; set; }
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
