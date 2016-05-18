using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XianDict
{
    public class SearchResult : IComparable<SearchResult>
    {
        public string Traditional { get; set; }
        public string Simplified { get; set; }
        public string Pinyin { get; set; }
        public string PinyinNumbered { get; set; }
        public List<List<string>> Definitions { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            SearchResult p = obj as SearchResult;
            if (p == null)
            {
                return false;
            }

            return Traditional.Equals(p.Traditional) && PinyinNumbered.ToLower().Equals(p.PinyinNumbered.ToLower());
        }

        public bool Equals(SearchResult p)
        {
            if (p == null)
            {
                return false;
            }
            return Traditional.Equals(p.Traditional) && PinyinNumbered.ToLower().Equals(p.PinyinNumbered.ToLower());
        }

        public override int GetHashCode()
        {
            return Traditional.GetHashCode() ^ PinyinNumbered.GetHashCode();
        }

        public int CompareTo(SearchResult other)
        {
            int pinyinCompare = PinyinNumbered.CompareTo(other.PinyinNumbered);
            int traditionalCompare = Traditional.CompareTo(other.Traditional);

            if (pinyinCompare == 0)
            {
                return traditionalCompare;
            }
            return pinyinCompare;
        }
    }
}
