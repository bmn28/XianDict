using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace XianDict
{
    public static class DictionaryRenderer
    {
        public static ResourceDictionary rd;

        private static Regex rx = new Regex(@"`([^`~]+)~");

        public static async Task<FlowDocument> Render(SearchResult entry, ResourceDictionary rd)
        {
            FlowDocument doc = new FlowDocument();
            //doc.FontSize = 24;

            //foreach (Moedict.Heteronym h in entry.Heteronyms)
            //{
            //    Paragraph headword = ParseLinks(entry.Title);
            //    headword.Style = (Style)rd["HeadwordStyle"];
            //    doc.Blocks.Add(headword);
            //    Paragraph pinyin = new Paragraph(new Run(h.Pinyin));
            //    pinyin.Style = (Style)rd["PinyinStyle"];
            //    doc.Blocks.Add(pinyin);

            //    Paragraph type;
            //    string currentType = h.Definitions[0].Type;
            //    if (currentType != null)
            //    {
            //        type = ParseLinks(currentType, "TypeStyle");
            //        doc.Blocks.Add(type);
            //    }

            //    List list = new List();
            //    list.MarkerStyle = TextMarkerStyle.Decimal;

            //    foreach (Moedict.Definition d in h.Definitions)
            //    {
            //        string newType = d.Type;
            //        if (currentType != null && !currentType.Equals(newType))
            //        {
            //            doc.Blocks.Add(list);
            //            list = new List();
            //            currentType = newType;
            //            type = ParseLinks(currentType, "TypeStyle");
            //            doc.Blocks.Add(type);
            //        }

            //        ListItem listItem = new ListItem(ParseLinks(d.Gloss));
            //        if (d.Examples != null)
            //        {
            //            foreach (string e in d.Examples)
            //            {
            //                listItem.Blocks.Add(ParseLinks(e, "ExampleStyle"));
            //            }
            //        }
            //        if (d.Quotes != null)
            //        {
            //            foreach (string q in d.Quotes)
            //            {
            //                listItem.Blocks.Add(ParseLinks(q, "QuoteStyle"));
            //            }
            //        }

            //        list.ListItems.Add(listItem);
            //    }
            //    doc.Blocks.Add(list);
            //    list.MarkerStyle = list.ListItems.Count > 1 ? TextMarkerStyle.Decimal : TextMarkerStyle.None;
            //}

            return doc;
        }

         

        public static Paragraph ParseLinks(string text, string style = null)
        {
            Paragraph p = new Paragraph();

            Match match = rx.Match(text);

            for (int i = 0; i < text.Length;)
            {
                if (!match.Success)
                {
                    p.Inlines.Add(new Run(text.Substring(i)));
                    break;
                }
                else
                {
                    if (match.Index > i)
                    {
                        // make a regular run
                        p.Inlines.Add(new Run(text.Substring(i, match.Index - i)));
                    }
                    // make a hyperlink
                    Hyperlink h = new Hyperlink(new Run(match.Groups[1].Value));
                    //h.Click += followHyperlink;
                    p.Inlines.Add(h);
                    i = match.Index + match.Length;
                    match = match.NextMatch();
                }
            }

            if (style != null)
            {
                p.Style = (Style)rd[style];
            }

            return p;
        }
    }

}
