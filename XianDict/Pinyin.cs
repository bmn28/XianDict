using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace XianDict
{
    // https://github.com/tsroten/zhon/
    public static class Pinyin
    {
        public static Regex PinyinNumbered = new Regex(@"(shuang|chuang|zhuang|xiang|qiong|shuai|niang|guang|sheng|kuang|shang|jiong|huang|jiang|shuan|xiong|zhang|zheng|zhong|zhuai|zhuan|qiang|chang|liang|chuan|cheng|chong|chuai|hang|peng|chuo|piao|pian|chua|ping|yang|pang|chui|chun|chen|chan|chou|chao|chai|zhun|mang|meng|weng|shai|shei|miao|zhui|mian|yong|ming|wang|zhuo|zhua|shao|yuan|bing|zhen|fang|feng|zhan|zhou|zhao|zhei|zhai|rang|suan|reng|song|seng|dang|deng|dong|xuan|sang|rong|duan|cuan|cong|ceng|cang|diao|ruan|dian|ding|shou|xing|zuan|jiao|zong|zeng|zang|jian|tang|teng|tong|bian|biao|shan|tuan|huan|xian|huai|tiao|tian|hong|xiao|heng|ying|jing|shen|beng|kuan|kuai|nang|neng|nong|juan|kong|nuan|keng|kang|shua|niao|guan|nian|ting|shuo|guai|ning|quan|qiao|shui|gong|geng|gang|qian|bang|lang|leng|long|qing|ling|luan|shun|lian|liao|zhi|lia|liu|qin|lun|lin|luo|lan|lou|qiu|gai|gei|gao|gou|gan|gen|lao|lei|lai|que|gua|guo|nin|gui|niu|nie|gun|qie|qia|jun|kai|kei|kao|kou|kan|ken|qun|nun|nuo|xia|kua|kuo|nen|kui|nan|nou|kun|jue|nao|nei|hai|hei|hao|hou|han|hen|nai|rou|xiu|jin|hua|huo|tie|hui|tun|tui|hun|tuo|tan|jiu|zai|zei|zao|zou|zan|zen|eng|tou|tao|tei|tai|zuo|zui|xin|zun|jie|jia|run|diu|cai|cao|cou|can|cen|die|dia|xue|rui|cuo|cui|dun|cun|cin|ruo|rua|dui|sai|sao|sou|san|sen|duo|den|dan|dou|suo|sui|dao|sun|dei|zha|zhe|dai|xun|ang|ong|wai|fen|fan|fou|fei|zhu|wei|wan|min|miu|mie|wen|men|lie|chi|cha|che|man|mou|mao|mei|mai|yao|you|yan|chu|pin|pie|yin|pen|pan|pou|pao|shi|sha|she|pei|pai|yue|bin|bie|yun|nüe|lve|shu|ben|ban|bao|bei|bai|lüe|nve|ren|ran|rao|xie|re|ri|si|su|se|ru|sa|cu|ce|ca|ji|ci|zi|zu|ze|za|hu|he|ha|ju|ku|ke|qi|ka|gu|ge|ga|li|lu|le|qu|la|ni|xi|nu|ne|na|ti|tu|te|ta|xu|di|du|de|bo|lv|ba|ai|ei|ao|ou|an|en|er|da|wu|wa|wo|fu|fo|fa|nv|mi|mu|yi|ya|ye|me|mo|ma|pi|pu|po|yu|pa|bi|nü|bu|lü|e|o|a|r)r?[1-5]", RegexOptions.IgnoreCase);
        public static Regex GroupBySpaces = new Regex(@"([^ ]+)");

        public static char[] Semivowels = { 'i', 'u', 'ü', 'I', 'U', 'Ü' };

        public static char[] Pinyin1 = { 'ā', 'ē', 'ī', 'ō', 'ū', 'ǖ', 'Ā', 'Ē', 'Ī', 'Ō', 'Ū', 'Ǖ' };
        public static char[] Pinyin2 = { 'á', 'é', 'í', 'ó', 'ú', 'ǘ', 'Á', 'É', 'Í', 'Ó', 'Ú', 'Ǘ' };
        public static char[] Pinyin3 = { 'ǎ', 'ě', 'ǐ', 'ǒ', 'ǔ', 'ǚ', 'Ǎ', 'Ě', 'Ǐ', 'Ǒ', 'Ǔ', 'Ǚ' };
        public static char[] Pinyin4 = { 'à', 'è', 'ì', 'ò', 'ù', 'ǜ', 'À', 'È', 'Ì', 'Ò', 'Ù', 'Ǜ' };
        public static char[] Pinyin5 = { 'a', 'e', 'i', 'o', 'u', 'ü', 'A', 'E', 'I', 'O', 'U', 'Ü' };

        public static Dictionary<char, char[]> Vowels = new Dictionary<char, char[]>
            { { '1', Pinyin1 }, { '2', Pinyin2 }, { '3', Pinyin3 }, { '4', Pinyin4 }, { '5', Pinyin5 }, };
        public static Dictionary<char, int> VowelLookup = new Dictionary<char, int>
            { {'a', 0 }, {'e', 1 }, {'i', 2 }, {'o', 3 }, {'u', 4 }, {'ü', 5 }, {'A', 6 }, {'E', 7 }, {'I', 8 }, {'O', 9 }, {'U', 10 }, {'Ü', 11 } };

        public static string ConvertToAccents(string input)
        {
            return input == null ? null : PinyinNumbered.Replace(input, new MatchEvaluator(ConvertWordToAccents));
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
    }
}
