using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XianDict
{
    public static class TradToSimp
    {
        private static Regex simplifiedVariant = new Regex(@"U\+((?>\w+))\tkSimplifiedVariant\t((?>.*))");
        //private static Regex simplifiedVariant = new Regex(@"U+((?>\w+))\tkSimplifiedVariant\tU+((?>\w+))(?: U+((?>\w+)))*");

        private static Dictionary<uint, uint[]> conversions;

        static TradToSimp()
        {
            conversions = new Dictionary<uint, uint[]>();
            foreach (string line in System.IO.File.ReadAllLines("Unihan_Variants.txt"))
            {
                var match = simplifiedVariant.Match(line);
                if (match.Success)
                {
                    uint trad = System.Convert.ToUInt32(match.Groups[1].Value, 16);
                    var simps = match.Groups[2].Value.Replace("U+", "").Split();
                    conversions[trad] = new uint[simps.Length];
                    for (int i = 0; i < simps.Length; i++)
                    {
                        conversions[trad][i] = (System.Convert.ToUInt32(simps[i], 16));
                    }
                }
            }
        }

        public static IEnumerable<string> Convert(string input)
        {
            var groups = new List<uint[]>();
            for (int i = 0; i < input.Length; i++)
            {
                uint trad = input[i];
                if (trad >= 0xD800 && trad <= 0xDBFF)
                {
                    i++;
                    uint l = input[i];
                    trad = ((trad - 0xD800) << 10) + (l - 0xDC00);
                }

                uint[] simp;
                if (conversions.TryGetValue(trad, out simp))
                {
                    groups.Add(simp);
                }
                else
                {
                    groups.Add(new uint[] { trad });
                }
            }
            var iterator = new CartesianArrayIterator<uint>(groups);
            var combinations = new List<uint[]>();
            while (iterator.MoveNext())
                combinations.Add(iterator.Current);
            var results = new List<string>();
            foreach (var combination in combinations)
            {
                StringBuilder sb = new StringBuilder();
                foreach (uint codepoint in combination)
                {
                    if (codepoint > 0xFFFF)
                    {
                        char hs = (char)((codepoint >> 10) + 0xD800);
                        char ls = (char)((codepoint & 0x03FF) + 0xDC00);
                        sb.Append(hs);
                        sb.Append(ls);
                    }
                    else
                    {
                        sb.Append((char)codepoint);
                    }
                }
                results.Add(sb.ToString());
            }
            return results;
        }
    }
}
