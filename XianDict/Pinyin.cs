using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace XianDict
{
    // https://github.com/tsroten/zhon/
    public static class Pinyin
    {
        public static Regex PinyinSyllablesIncludingR = new Regex(@"(shuang|chuang|zhuang|xiang|qiong|shuai|niang|guang|sheng|kuang|shang|jiong|huang|jiang|shuan|xiong|zhang|zheng|zhong|zhuai|zhuan|qiang|chang|liang|chuan|cheng|chong|chuai|hang|peng|chuo|piao|pian|chua|ping|yang|pang|chui|chun|chen|chan|chou|chao|chai|zhun|mang|meng|weng|shai|shei|miao|zhui|mian|yong|ming|wang|zhuo|zhua|shao|yuan|bing|zhen|fang|feng|zhan|zhou|zhao|zhei|zhai|rang|suan|reng|song|seng|dang|deng|dong|xuan|sang|rong|duan|cuan|cong|ceng|cang|diao|ruan|dian|ding|shou|xing|zuan|jiao|zong|zeng|zang|jian|tang|teng|tong|bian|biao|shan|tuan|huan|xian|huai|tiao|tian|hong|xiao|heng|ying|jing|shen|beng|kuan|kuai|nang|neng|nong|juan|kong|nuan|keng|kang|shua|niao|guan|nian|ting|shuo|guai|ning|quan|qiao|shui|gong|geng|gang|qian|bang|lang|leng|long|qing|ling|luan|shun|lian|liao|zhi|lia|liu|qin|lun|lin|luo|lan|lou|qiu|gai|gei|gao|gou|gan|gen|lao|lei|lai|que|gua|guo|nin|gui|niu|nie|gun|qie|qia|jun|kai|kei|kao|kou|kan|ken|qun|nun|nuo|xia|kua|kuo|nen|kui|nan|nou|kun|jue|nao|nei|hai|hei|hao|hou|han|hen|nai|rou|xiu|jin|hua|huo|tie|hui|tun|tui|hun|tuo|tan|jiu|zai|zei|zao|zou|zan|zen|eng|tou|tao|tei|tai|zuo|zui|xin|zun|jie|jia|run|diu|cai|cao|cou|can|cen|die|dia|xue|rui|cuo|cui|dun|cun|cin|ruo|rua|dui|sai|sao|sou|san|sen|duo|den|dan|dou|suo|sui|dao|sun|dei|zha|zhe|dai|xun|ang|ong|wai|fen|fan|fou|fei|zhu|wei|wan|min|miu|mie|wen|men|lie|chi|cha|che|man|mou|mao|mei|mai|yao|you|yan|chu|pin|pie|yin|pen|pan|pou|pao|shi|sha|she|pei|pai|yue|bin|bie|yun|nüe|lve|shu|ben|ban|bao|bei|bai|lüe|nve|ren|ran|rao|xie|re|ri|si|su|se|ru|sa|cu|ce|ca|ji|ci|zi|zu|ze|za|hu|he|ha|ju|ku|ke|qi|ka|gu|ge|ga|li|lu|le|qu|la|ni|xi|nu|ne|na|ti|tu|te|ta|xu|di|du|de|bo|lv|ba|ai|ei|ao|ou|an|en|er|da|wu|wa|wo|fu|fo|fa|nv|mi|mu|yi|ya|ye|me|mo|ma|pi|pu|po|yu|pa|bi|nü|bu|lü|e|o|a|r)+", RegexOptions.IgnoreCase);
        public static Regex PinyinNumberedIncludingR = new Regex(@"(shuang|chuang|zhuang|xiang|qiong|shuai|niang|guang|sheng|kuang|shang|jiong|huang|jiang|shuan|xiong|zhang|zheng|zhong|zhuai|zhuan|qiang|chang|liang|chuan|cheng|chong|chuai|hang|peng|chuo|piao|pian|chua|ping|yang|pang|chui|chun|chen|chan|chou|chao|chai|zhun|mang|meng|weng|shai|shei|miao|zhui|mian|yong|ming|wang|zhuo|zhua|shao|yuan|bing|zhen|fang|feng|zhan|zhou|zhao|zhei|zhai|rang|suan|reng|song|seng|dang|deng|dong|xuan|sang|rong|duan|cuan|cong|ceng|cang|diao|ruan|dian|ding|shou|xing|zuan|jiao|zong|zeng|zang|jian|tang|teng|tong|bian|biao|shan|tuan|huan|xian|huai|tiao|tian|hong|xiao|heng|ying|jing|shen|beng|kuan|kuai|nang|neng|nong|juan|kong|nuan|keng|kang|shua|niao|guan|nian|ting|shuo|guai|ning|quan|qiao|shui|gong|geng|gang|qian|bang|lang|leng|long|qing|ling|luan|shun|lian|liao|zhi|lia|liu|qin|lun|lin|luo|lan|lou|qiu|gai|gei|gao|gou|gan|gen|lao|lei|lai|que|gua|guo|nin|gui|niu|nie|gun|qie|qia|jun|kai|kei|kao|kou|kan|ken|qun|nun|nuo|xia|kua|kuo|nen|kui|nan|nou|kun|jue|nao|nei|hai|hei|hao|hou|han|hen|nai|rou|xiu|jin|hua|huo|tie|hui|tun|tui|hun|tuo|tan|jiu|zai|zei|zao|zou|zan|zen|eng|tou|tao|tei|tai|zuo|zui|xin|zun|jie|jia|run|diu|cai|cao|cou|can|cen|die|dia|xue|rui|cuo|cui|dun|cun|cin|ruo|rua|dui|sai|sao|sou|san|sen|duo|den|dan|dou|suo|sui|dao|sun|dei|zha|zhe|dai|xun|ang|ong|wai|fen|fan|fou|fei|zhu|wei|wan|min|miu|mie|wen|men|lie|chi|cha|che|man|mou|mao|mei|mai|yao|you|yan|chu|pin|pie|yin|pen|pan|pou|pao|shi|sha|she|pei|pai|yue|bin|bie|yun|nüe|lve|shu|ben|ban|bao|bei|bai|lüe|nve|ren|ran|rao|xie|re|ri|si|su|se|ru|sa|cu|ce|ca|ji|ci|zi|zu|ze|za|hu|he|ha|ju|ku|ke|qi|ka|gu|ge|ga|li|lu|le|qu|la|ni|xi|nu|ne|na|ti|tu|te|ta|xu|di|du|de|bo|lv|ba|ai|ei|ao|ou|an|en|er|da|wu|wa|wo|fu|fo|fa|nv|mi|mu|yi|ya|ye|me|mo|ma|pi|pu|po|yu|pa|bi|nü|bu|lü|e|o|a|r)r?[1-5]", RegexOptions.IgnoreCase);
        public static Regex GroupBySpaces = new Regex(@"([^ ]+)");
        public static Regex UnmarkedSyllables = new Regex(@"((?>[A-ZÜa-zü]+))(?![12345])");
        public static Regex Numbers = new Regex(@"([12345])");
        public static Regex NumbersAndQuestionMark = new Regex(@"([12345\?])");
        public static Regex NumbersAtEndOrNotFollowedBySpace = new Regex(@"([12345\?])(?! |$)");
        public static Regex Umlauts = new Regex(@"[üÜ]");
        public static Regex UmlautCandidates = new Regex(@"(nu|lu|nue|lue)", RegexOptions.IgnoreCase);

        public static char[] Semivowels = { 'i', 'u', 'ü', 'I', 'U', 'Ü' };

        public static char[] Pinyin1 = { 'ā', 'ē', 'ī', 'ō', 'ū', 'ǖ', 'Ā', 'Ē', 'Ī', 'Ō', 'Ū', 'Ǖ' };
        public static char[] Pinyin2 = { 'á', 'é', 'í', 'ó', 'ú', 'ǘ', 'Á', 'É', 'Í', 'Ó', 'Ú', 'Ǘ' };
        public static char[] Pinyin3 = { 'ǎ', 'ě', 'ǐ', 'ǒ', 'ǔ', 'ǚ', 'Ǎ', 'Ě', 'Ǐ', 'Ǒ', 'Ǔ', 'Ǚ' };
        public static char[] Pinyin4 = { 'à', 'è', 'ì', 'ò', 'ù', 'ǜ', 'À', 'È', 'Ì', 'Ò', 'Ù', 'Ǜ' };
        public static char[] Pinyin5 = { 'a', 'e', 'i', 'o', 'u', 'ü', 'A', 'E', 'I', 'O', 'U', 'Ü' };

        public static string[] SyllablesNoVs = { "shuang", "chuang", "zhuang", "xiang", "qiong", "shuai", "niang", "guang", "sheng", "kuang", "shang", "jiong", "huang", "jiang", "shuan", "xiong", "zhang", "zheng", "zhong", "zhuai", "zhuan", "qiang", "chang", "liang", "chuan", "cheng", "chong", "chuai", "hang", "peng", "chuo", "piao", "pian", "chua", "ping", "yang", "pang", "chui", "chun", "chen", "chan", "chou", "chao", "chai", "zhun", "mang", "meng", "weng", "shai", "shei", "miao", "zhui", "mian", "yong", "ming", "wang", "zhuo", "zhua", "shao", "yuan", "bing", "zhen", "fang", "feng", "zhan", "zhou", "zhao", "zhei", "zhai", "rang", "suan", "reng", "song", "seng", "dang", "deng", "dong", "xuan", "sang", "rong", "duan", "cuan", "cong", "ceng", "cang", "diao", "ruan", "dian", "ding", "shou", "xing", "zuan", "jiao", "zong", "zeng", "zang", "jian", "tang", "teng", "tong", "bian", "biao", "shan", "tuan", "huan", "xian", "huai", "tiao", "tian", "hong", "xiao", "heng", "ying", "jing", "shen", "beng", "kuan", "kuai", "nang", "neng", "nong", "juan", "kong", "nuan", "keng", "kang", "shua", "niao", "guan", "nian", "ting", "shuo", "guai", "ning", "quan", "qiao", "shui", "gong", "geng", "gang", "qian", "bang", "lang", "leng", "long", "qing", "ling", "luan", "shun", "lian", "liao", "zhi", "lia", "liu", "qin", "lun", "lin", "luo", "lan", "lou", "qiu", "gai", "gei", "gao", "gou", "gan", "gen", "lao", "lei", "lai", "que", "gua", "guo", "nin", "gui", "niu", "nie", "gun", "qie", "qia", "jun", "kai", "kei", "kao", "kou", "kan", "ken", "qun", "nun", "nuo", "xia", "kua", "kuo", "nen", "kui", "nan", "nou", "kun", "jue", "nao", "nei", "hai", "hei", "hao", "hou", "han", "hen", "nai", "rou", "xiu", "jin", "hua", "huo", "tie", "hui", "tun", "tui", "hun", "tuo", "tan", "jiu", "zai", "zei", "zao", "zou", "zan", "zen", "eng", "tou", "tao", "tei", "tai", "zuo", "zui", "xin", "zun", "jie", "jia", "run", "diu", "cai", "cao", "cou", "can", "cen", "die", "dia", "xue", "rui", "cuo", "cui", "dun", "cun", "cin", "ruo", "rua", "dui", "sai", "sao", "sou", "san", "sen", "duo", "den", "dan", "dou", "suo", "sui", "dao", "sun", "dei", "zha", "zhe", "dai", "xun", "ang", "ong", "wai", "fen", "fan", "fou", "fei", "zhu", "wei", "wan", "min", "miu", "mie", "wen", "men", "lie", "chi", "cha", "che", "man", "mou", "mao", "mei", "mai", "yao", "you", "yan", "chu", "pin", "pie", "yin", "pen", "pan", "pou", "pao", "shi", "sha", "she", "pei", "pai", "yue", "bin", "bie", "yun", "nüe", "shu", "ben", "ban", "bao", "bei", "bai", "lüe", "ren", "ran", "rao", "xie", "re", "ri", "si", "su", "se", "ru", "sa", "cu", "ce", "ca", "ji", "ci", "zi", "zu", "ze", "za", "hu", "he", "ha", "ju", "ku", "ke", "qi", "ka", "gu", "ge", "ga", "li", "lu", "le", "qu", "la", "ni", "xi", "nu", "ne", "na", "ti", "tu", "te", "ta", "xu", "di", "du", "de", "bo", "ba", "ai", "ei", "ao", "ou", "an", "en", "er", "da", "wu", "wa", "wo", "fu", "fo", "fa", "mi", "mu", "yi", "ya", "ye", "me", "mo", "ma", "pi", "pu", "po", "yu", "pa", "bi", "nü", "bu", "lü", "e", "o", "a", "r" };

        public static Dictionary<char, char[]> Vowels = new Dictionary<char, char[]>
            { { '1', Pinyin1 }, { '2', Pinyin2 }, { '3', Pinyin3 }, { '4', Pinyin4 }, { '5', Pinyin5 }, };
        public static Dictionary<char, int> VowelLookup = new Dictionary<char, int>
            { {'a', 0 }, {'e', 1 }, {'i', 2 }, {'o', 3 }, {'u', 4 }, {'ü', 5 }, {'A', 6 }, {'E', 7 }, {'I', 8 }, {'O', 9 }, {'U', 10 }, {'Ü', 11 } };

        public static Trie<string> Trie = new Trie<string>();

        static Pinyin()
        {
            foreach (var syllable in SyllablesNoVs)
            {
                Trie.Put(syllable, syllable);
            }
        }

        public static string ConvertToAccents(string input)
        {
            return input == null ? null : PinyinNumberedIncludingR.Replace(input, new MatchEvaluator(ConvertWordToAccents));
        }

        public static string ConvertWordToAccents(Match m)
        {
            string word = m.Value.Remove(m.Length - 1).Replace('v', 'ü').Replace('V', 'Ü');
            char tone = m.Value[m.Length - 1];
            if (tone == '5')
            {
                return word;
            }
            int firstVowel = word.IndexOfAny(Pinyin5);
            if (firstVowel != word.Length - 1 && Pinyin5.Contains(word[firstVowel + 1]) && Semivowels.Contains(word[firstVowel]))
            {
                return word.Remove(firstVowel + 1, 1)
                    .Insert(firstVowel + 1, Vowels[tone][VowelLookup[word[firstVowel + 1]]].ToString());
            }
            else
            {
                return word.Remove(firstVowel, 1)
                    .Insert(firstVowel, Vowels[tone][VowelLookup[word[firstVowel]]].ToString());
            }
        }

        public static string ConvertToNumbers(string input)
        {
            return input == null ? null : GroupBySpaces.Replace(input, new MatchEvaluator(ConvertWordToNumbers));
        }

        public static string ConvertWordToNumbers(Match m)
        {
            foreach (var mapping in Vowels)
            {
                int i = m.Value.IndexOfAny(mapping.Value);
                if (i != -1)
                {
                    return m.Value.Remove(i, 1)
                        .Insert(i, Pinyin5[Array.IndexOf(mapping.Value, m.Value[i])].ToString())
                        .Insert(m.Length, mapping.Key.ToString());
                }
            }
            return m.Value;
        }

        public static IEnumerable<string> ToQueryForms(string input)
        {
            // strip extra spaces
            input = input.ToLower().Trim();
            input = Regex.Replace(input, @" +", " ");

            // matches for ü
            input = Regex.Replace(input, @"(v|V|u:|U:)", "ü");

            // match on runs of letters and runs of separators
            // expected separators: spaces, numbers, apostrophes, wildcards, Chinese characters
            var alphaMatches = Regex.Matches(input, @"(?>[A-ZÜa-zü]+)");
            var separatorMatches = Regex.Matches(input, @"(?>[^A-ZÜa-zü]+)");
            var alphaRuns = alphaMatches.Cast<Match>()
                .Select(m => new { Value = m.Value, Index = m.Index, Length = m.Length, Splits = new List<string>() })
                .ToList();
            var separatorRuns = separatorMatches.Cast<Match>()
                .Select(m => new { Value = m.Value, Index = m.Index, Length = m.Length, Splits = new List<string>() { m.Value } })
                .ToList();

            // find ways to split each run into pinyin syllables
            int i = 0;
            foreach (var run in alphaRuns)
            {
                i++;
                run.Splits.AddRange(findRuns(run.Value, i == alphaMatches.Count)
                    .Select(g => string.Join(" ", g)).ToList());
            }

            var combinedRuns = alphaRuns.Union(separatorRuns).OrderBy(r => r.Index).ToList();

            // find all combinations (Cartesian product) of splits by run
            List<List<string>> combinations = new List<List<string>>();
            var iterator = new CartesianIterator<string>(combinedRuns.Select(r => r.Splits).ToList());
            while (iterator.MoveNext())
                combinations.Add(iterator.Current);

            var queries = new List<string>();
            foreach (var combination in combinations)
            {
                var query = string.Join("", combination);
                query = UnmarkedSyllables.Replace(query, "$1?");
                query = query.Replace("'", "");
                query = NumbersAtEndOrNotFollowedBySpace.Replace(query, "$1 ");
                query = Umlauts.Replace(query, @"u:");
                query = query.Replace("\\", "\\\\");
                query = query.Replace("_", "\\_").Replace("%", "\\%");
                query = query.Replace('?', '_').Replace('*', '%');
                queries.Add(query);
            }

            return queries;
        }

        public static string RemoveNumbersAndQuestionMark(string input)
        {
            return input == null ? null : NumbersAndQuestionMark.Replace(input, "");
        }

        private static List<List<string>> findRuns(string input, bool allowPartialAtEnd)
        {
            List<List<string>> results = new List<List<string>>();
            var candidates = Trie.GetAll(input);
            // A candidate is a valid syllable found at the beginning of input.
            foreach (var candidate in candidates)
            {
                // If candidate is equal (and therefore has the same length) as input, the input has been fully parsed.
                // A list containing the current element may be added to results.
                if (candidate.Length == input.Length)
                {
                    results.Add(new List<string>() { candidate });
                    if (UmlautCandidates.Match(candidate).Success)
                    {
                        results.Add(new List<string>() { candidate.Replace("u", "ü").Replace("U", "Ü") });
                    }
                }
                // Otherwise, there is remaining input to parse.
                // Recursively call findRuns on the remainder.
                // For each result (List<string>) obtained, insert the candidate at the beginning.
                else
                {
                    var remainder = input.Substring(candidate.Length);
                    var remainderRuns = findRuns(remainder, allowPartialAtEnd);
                    foreach (var remainderRun in remainderRuns)
                    {
                        var candidateRun = new List<string>() { candidate };
                        candidateRun.AddRange(remainderRun);
                        results.Add(candidateRun);
                        if (UmlautCandidates.Match(candidate).Success)
                        {
                            candidateRun = new List<string>() { candidate.Replace("u", "ü").Replace("U", "Ü") };
                            candidateRun.AddRange(remainderRun);
                            results.Add(candidateRun);
                        }
                    }
                }
            }

            // In addition to having valid candidates, the input may also be a valid prefix.
            if (allowPartialAtEnd && !Trie.Contains(input) && Trie.ContainsPrefix(input))
            {
                results.Add(new List<string>() { input });
            }

            return results;
        }

    }

    public class CartesianIterator<T> : IEnumerator<List<T>>
    {
        private List<List<T>> sets;
        private int[] cardinalities;
        private int[] indices;
        private bool started;

        public CartesianIterator(List<List<T>> sets)
        {
            this.sets = sets;
            cardinalities = new int[sets.Count];
            indices = new int[sets.Count];
            int i = 0;
            foreach (var set in sets)
            {
                cardinalities[i] = sets[i].Count;
                indices[i] = 0;
                i++;
            }
            started = false;
        }

        public List<T> Current
        {
            get
            {
                List<T> result = new List<T>();
                int i = 0;
                foreach (var set in sets)
                {
                    if (cardinalities[i] > 0)
                    {
                        result.Add(set[indices[i]]);
                    }
                    i++;
                }
                return result;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            if (!started)
            {
                started = true;
                return true;
            }
                
            bool carry = true;
            for (int i = indices.Length - 1; i >= 0; i--)
            {
                if (carry)
                {
                    if (cardinalities[i] > 0)
                    {
                        indices[i] = (indices[i] + 1) % cardinalities[i];
                    }
                    carry = (indices[i] == 0);
                }
                else
                {
                    break;
                }
            }
            return !carry;
        }

        public void Reset()
        {
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = 0;
            }
            started = false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CartesianIterator() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
