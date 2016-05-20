using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XianDict;
using System.IO;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace XianDictTests
{
    [TestClass]
    public class StrokeWordTest
    {
        [TestMethod]
        public void Constructor_Test()
        {
            List<StrokeWord> words = new List<StrokeWord>();
            var files = Directory.GetFiles(@"..\..\..\XianDict\bin\Debug\utf8");
            foreach (var file in files)
            {
                using (FileStream fs = File.OpenRead(file))
                {
                    StrokeWord word = new StrokeWord(fs);
                    words.Add(word);
                }
            }
        }

        [TestMethod]
        public void Window_Test()
        {
            StrokeDisplay d = new StrokeDisplay();
            d.ShowDialog();
        }
    }
}
