using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace XianDict
{
    /// <summary>
    /// Interaction logic for ClipboardViewer.xaml
    /// </summary>
    public partial class ClipboardViewer : Page
    {
        private static Regex hanzi = new Regex(@"\p{IsCJKUnifiedIdeographs}|\p{IsCJKUnifiedIdeographsExtensionA}");

        private string text;
        private MainWindow mainWindow;

        public ClipboardViewer(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
            fdViewer.AddHandler(MouseUpEvent, new MouseButtonEventHandler(fdViewer_MouseUp), true);

            fdViewer.Document = new FlowDocument(new Paragraph(
                new Run("Text copied to the clipboard that contains Chinese characters will appear here."))) { FontFamily = new FontFamily("Segoe UI"), FontSize = 12, PagePadding = new Thickness(5) };
        }

        public void UpdateText(string text)
        {
            if (hanzi.IsMatch(text))
            {
                this.text = text;
                fdViewer.Document = new FlowDocument(new Paragraph(new Run(text)))
                { FontFamily = new FontFamily("Microsoft JhengHei"), PagePadding = new Thickness(5) };
            }
        }

        private async void fdViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mainWindow.fdViewer_MouseUp(sender, e);
        }
    }
}
