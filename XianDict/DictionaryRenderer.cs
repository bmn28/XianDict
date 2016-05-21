using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace XianDict
{
    public static class DictionaryRenderer
    {
        public static ResourceDictionary rd;

        private static Regex rx = new Regex(@"`([^`~]+)~");

        public static FlowDocument Render(Term term, DictionaryEngine engine, bool minimal = false)
        {
            FlowDocument doc = new FlowDocument();
            
            Paragraph headword = ParseLinks(term.Traditional);
            headword.Style = (Style)rd["HeadwordStyle"];

            if (!minimal && term.Traditional.Length == 1 && StrokeDisplay.hasStrokeFile(term.Traditional[0]))
            {
                var strokeOrderButton = new Button();
                strokeOrderButton.Content = "Stroke Order";
                strokeOrderButton.Margin = new Thickness(25,5,5,1);
                strokeOrderButton.Padding = new Thickness(4);
                strokeOrderButton.FontSize = 12;
                strokeOrderButton.Click += ((obj, e) => { new StrokeDisplay(term.Traditional[0]).Show(); });
                headword.Inlines.Add(new InlineUIContainer(strokeOrderButton));
                //headword.Inlines.Add(new Run("\t"));
                //Hyperlink h = new Hyperlink(new Run("Stroke order"));
                //h.Click += ((obj, e) => { new StrokeDisplay(term.Traditional[0]).Show(); } );
                //headword.Inlines.Add(h);
            }
            doc.Blocks.Add(headword);

            Paragraph pinyin = new Paragraph(new Run(term.Pinyin));
            pinyin.Style = (Style)rd["PinyinStyle"];
            doc.Blocks.Add(pinyin);


            if (term.CedictEntryId != 0)
            {
                doc.Blocks.Add(new BlockUIContainer(new Separator() { Margin = new Thickness(-3, 3, -3, 3) }));

                var entry = ((Cedict)engine["cedict"]).LookupEntry(term.CedictEntryId).Result;
                Paragraph heading = new Paragraph(new Run("CC"));
                heading.Style = (Style)rd["HeadingStyle"];
                doc.Blocks.Add(heading);

                List list = new List();
                list.MarkerStyle = TextMarkerStyle.Decimal;

                foreach (CedictDefinition d in entry.Definitions)
                {
                    ListItem listItem = new ListItem(new Paragraph(new Run(d.Definition)) { Style = (Style)rd["EnglishParagraph"] });
                    list.ListItems.Add(listItem);
                }
                doc.Blocks.Add(list);
                if (list.ListItems.Count == 1)
                {
                    list.MarkerStyle = TextMarkerStyle.None;
                    list.Padding = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    list.MarkerStyle = TextMarkerStyle.Decimal;
                }
            }


            if (term.MoedictHeteronymId != 0)
            {
                doc.Blocks.Add(new BlockUIContainer(new Separator() { Margin = new Thickness(-3, 3, -3, 3) }));

                var heteronym = ((Moedict)engine["moedict"]).LookupHeteronym(term.MoedictHeteronymId).Result;
                Paragraph heading = new Paragraph(new Run("MOE"));
                heading.Style = (Style)rd["HeadingStyle"];
                doc.Blocks.Add(heading);
                Paragraph type;
                string currentType = heteronym.Definitions[0].Type;
                if (currentType != null)
                {
                    type = ParseLinks(currentType, "TypeStyle", true);
                    var uic = new BlockUIContainer() {  };
                    uic.Child = new Border() { BorderThickness = new Thickness(1), BorderBrush = System.Windows.Media.Brushes.Gray,Padding = new Thickness(2,2,2,0), CornerRadius = new CornerRadius(2), HorizontalAlignment = HorizontalAlignment.Left };
                    ((Border)uic.Child).Child = new TextBlock(type.Inlines.FirstInline) { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0), Foreground = System.Windows.Media.Brushes.Gray };
                    doc.Blocks.Add(uic);
                }

                List list = new List();
                list.MarkerStyle = TextMarkerStyle.Decimal;

                foreach (MoedictDefinition d in heteronym.Definitions)
                {
                    string newType = d.Type;
                    if (currentType != null && !currentType.Equals(newType))
                    {
                        if (list.ListItems.Count == 1)
                        {
                            list.MarkerStyle = TextMarkerStyle.None;
                            list.Padding = new Thickness(0, 0, 0, 0);
                        }
                        else
                        {
                            list.MarkerStyle = TextMarkerStyle.Decimal;
                        }
                        doc.Blocks.Add(list);
                        doc.Blocks.Add(new BlockUIContainer(new Separator() { Margin = new Thickness(10, 6, 10, 6) }));
                        list = new List();
                        currentType = newType;
                        type = ParseLinks(currentType, "TypeStyle", true);
                        var uic = new BlockUIContainer() { };
                        uic.Child = new Border() { BorderThickness = new Thickness(1), BorderBrush = System.Windows.Media.Brushes.Gray, Padding = new Thickness(2, 2, 2, 0), CornerRadius = new CornerRadius(2), HorizontalAlignment = HorizontalAlignment.Left };
                        ((Border)uic.Child).Child = new TextBlock(type.Inlines.FirstInline) { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0), Foreground = System.Windows.Media.Brushes.Gray };
                        doc.Blocks.Add(uic);
                    }

                    ListItem listItem = new ListItem(ParseLinks(d.Definition));
                    if (d.Examples != null)
                    {
                        foreach (var e in d.Examples)
                        {
                            listItem.Blocks.Add(ParseLinks(e.Example, "ExampleStyle"));
                        }
                    }
                    if (d.Quotes != null)
                    {
                        foreach (var q in d.Quotes)
                        {
                            listItem.Blocks.Add(ParseLinks(q.Quote, "QuoteStyle"));
                        }
                    }
                    list.ListItems.Add(listItem);
                }
                doc.Blocks.Add(list);
                if (list.ListItems.Count == 1)
                {
                    list.MarkerStyle = TextMarkerStyle.None;
                    list.Padding = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    list.MarkerStyle = TextMarkerStyle.Decimal;
                }
                //list.MarkerStyle = list.ListItems.Count > 1 ? TextMarkerStyle.Decimal : TextMarkerStyle.None;
            }

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

         

        public static Paragraph ParseLinks(string text, string style = null, bool noLinks = true)
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
                    if (noLinks)
                    {
                        p.Inlines.Add(new Run(match.Groups[1].Value));
                    }
                    else
                    {
                        Hyperlink h = new Hyperlink(new Run(match.Groups[1].Value));
                        //h.Click += followHyperlink;
                        p.Inlines.Add(h);
                    }
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
