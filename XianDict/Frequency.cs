using SQLite.Net.Attributes;

namespace XianDict
{
    class Frequency
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public string Hanzi { get; set; }
        public float Score { get; set; }
    }
}
