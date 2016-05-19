using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XianDict;

namespace XianDictTests
{
    [TestClass]
    public class PinyinTest
    {
        [TestMethod]
        public void ToQueryForms_TestEmpty()
        {
            ToQueryForms_Test("", new List<string>() { });
        }
        [TestMethod]
        public void ToQueryForms_RemoveExtraSpaces()
        {
            ToQueryForms_Test("  pian2   yi4 le ", new List<string>() { "pian2 yi4 le" });
        }

        [TestMethod]
        public void ToQueryForms_Combinations()
        {
            Pinyin.ToQueryForms("lu2 zhuang3nv LU:an");
        }

        public void ToQueryForms_Test(string input, IEnumerable<string> expected)
        {
            var result = Pinyin.ToQueryForms(input);
            var common = expected.Intersect(result);
            var incorrect = result.Except(common).ToList();
            incorrect.AddRange(common.Except(result).ToList());
            Assert.IsTrue(incorrect.Count == 0);
        }

        [TestMethod]
        public void Regex_Test()
        {
            Pinyin.ToQueryForms("qing qinglü4 lü4qing1 qinglü");
            Pinyin.ToQueryForms("chuangzaouerang");
            Pinyin.ToQueryForms("xi'an");
            Pinyin.ToQueryForms("");
            Pinyin.ToQueryForms("中文");
            Pinyin.ToQueryForms("abbbbbb");
        }

        [TestMethod]
        public void Pinyin_Trie_Test()
        {
            var results = Pinyin.Trie.GetAll("chuangzr");

            Assert.IsTrue(Pinyin.Trie.ContainsPrefix("a"));
            Assert.IsTrue(Pinyin.Trie.ContainsPrefix("an"));
            Assert.IsTrue(Pinyin.Trie.ContainsPrefix("xion"));
            Assert.IsFalse(Pinyin.Trie.ContainsPrefix("xiov"));
            Assert.IsFalse(Pinyin.Trie.ContainsPrefix("xionv"));
            Assert.IsFalse(Pinyin.Trie.ContainsPrefix("ngan"));
            Assert.IsFalse(Pinyin.Trie.ContainsPrefix("zz"));

        }

        [TestMethod]
        public void Pinyin_RemoveNumbers_Test()
        {
            Assert.AreEqual(Pinyin.RemoveNumbersAndUnderscore(""), "");
            Assert.AreEqual(Pinyin.RemoveNumbersAndUnderscore("tian1"), "tian");
            Assert.AreEqual(Pinyin.RemoveNumbersAndUnderscore("tian1xia4 wei?gong"), "tianxia weigong");
        }
    }
}
