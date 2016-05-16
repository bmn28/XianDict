using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace XianDict
{
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
