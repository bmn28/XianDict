using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace XianDict
{
    /// <summary>
    /// Interaction logic for RadicalInput.xaml
    /// </summary>
    public partial class RadicalInput : Page
    {
        private static string[] circledNumbers = new string[]
            { "①", "②", "③", "④", "⑤", "⑥", "⑦", "⑧", "⑨", "⑩", "⑪", "⑫", "⑬", "⑭", "⑮", "⑯", "⑰", "⑱", "⑲", "⑳",
            "㉑", "㉒", "㉓", "㉔", "㉕", "㉖", "㉗", "㉘", "㉙", "㉚", "㉛", "㉜", "㉝", "㉞", "㉟" };

        private static string[][] radicalsByStrokes;
        private static string[][][] hanziByRadicals;

        MainWindow mainWindow;
        FlowDocument mainDoc;
        FlowDocument radicalDoc;
        string radical;

        public RadicalInput(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            radicalsByStrokes = JsonConvert.DeserializeObject<string[][]>(System.IO.File.ReadAllText("radicals.txt"));
            hanziByRadicals = JsonConvert.DeserializeObject<string[][][]>(System.IO.File.ReadAllText("hanzibyradicals.txt"));

            mainDoc = CreateRadicalIndex();
            fdViewer.Document = mainDoc;
        }

        private FlowDocument CreateRadicalIndex()
        {
            FlowDocument doc = new FlowDocument() { FontFamily = new FontFamily("Microsoft JhengHei"), FontSize = 28, PagePadding = new Thickness(5) };

            int radicalNumber = 0;
            for (int i = 0; i < radicalsByStrokes.Length; i++)
            {
                var p = new Paragraph(new Run(circledNumbers[i])) { Margin = new Thickness(0) };
                for (int j = 0; j < radicalsByStrokes[i].Length; j++)
                {
                    var h = new Hyperlink(new Run(radicalsByStrokes[i][j])) { TextDecorations = null };
                    h.Click += GetRadicalDisplayFunc(radicalNumber);

                    p.Inlines.Add(h);
                    radicalNumber++;
                }

                doc.Blocks.Add(p);
            }

            return doc;
        }

        private RoutedEventHandler GetRadicalDisplayFunc(int index)
        {
            return ((obj, e) => 
            {
                radical = hanziByRadicals[index][0][0];
                returnButton.Content = radical;
                radicalDoc = CreateRadicalListing(index);
                fdViewer.Document = radicalDoc;
            });
        }
        private RoutedEventHandler GetAppendFunc(int radicalIndex, int strokeCount, int hanziIndex)
        {
            return ((obj, e) => { mainWindow.AppendToSearch(hanziByRadicals[radicalIndex][strokeCount][hanziIndex]); });
        }

        private FlowDocument CreateRadicalListing(int index)
        {
            FlowDocument doc = new FlowDocument() { FontFamily = new FontFamily("Microsoft JhengHei"), FontSize = 28, PagePadding = new Thickness(5) };

            for (int strokeCount = 0; strokeCount < hanziByRadicals[index].Length; strokeCount++)
            {
                var p = new Paragraph() { Margin = new Thickness(0) };
                if (hanziByRadicals[index][strokeCount] != null) {
                    if (strokeCount > 0)
                    {
                        if (strokeCount <= 35)
                        {
                            p.Inlines.Add(new Run(circledNumbers[strokeCount - 1]));
                        }
                        else
                        {
                            p.Inlines.Add(new Run(strokeCount + "."));
                        }
                    }
                    for (int hanziIndex = 0; hanziIndex < hanziByRadicals[index][strokeCount].Length; hanziIndex++)
                    {
                        var h = new Hyperlink(new Run(hanziByRadicals[index][strokeCount][hanziIndex])) { TextDecorations = null };
                        h.Click += GetAppendFunc(index, strokeCount, hanziIndex);
                        p.Inlines.Add(h);
                    }
                    doc.Blocks.Add(p);
                }
            }

            return doc;
        }

        private void returnButton_Click(object sender, RoutedEventArgs e)
        {
            if (fdViewer.Document == mainDoc && radicalDoc != null)
            {
                fdViewer.Document = radicalDoc;
                returnButton.Content = radical;
            } 
            else
            {
                fdViewer.Document = mainDoc;
                returnButton.Content = "？";
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Backspace();
        }
    }
}
