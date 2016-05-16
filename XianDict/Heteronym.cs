using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace XianDict
{
    public class Heteronym
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public string Traditional { get; set; }
        [Indexed]
        public string Simplified { get; set; }
        [Indexed]
        public string Pinyin { get; set; }

        [ForeignKey(typeof(CedictEntry))]
        public int CedictId { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public CedictEntry Cedict { get; set; }

        [ForeignKey(typeof(MoedictHeteronym))]
        public int MoedictId { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public MoedictHeteronym Moedict { get; set; }
    }
}
