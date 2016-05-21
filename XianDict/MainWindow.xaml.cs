using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.IO.IsolatedStorage;
using System.Windows.Controls.Primitives;

namespace XianDict
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum SearchMode
        {
            Chinese, English
        }

        private DictionaryEngine engine;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private SearchMode searchMode;
        public ObservableCollection<Term> results { get; set; }
        private Term selectedEntry;
        public Term SelectedEntry
        {
            get
            {
                return selectedEntry;
            }
            set
            {
                if (selectedEntry == null || !selectedEntry.Equals(value))
                {
                    selectedEntry = value;
                    if (selectedEntry != null)
                    {
                        fdViewer.Document = DictionaryRenderer.Render(selectedEntry, engine);
                    }
                }
            }
        }
        private string query;
        public string Query
        {
            get
            {
                return query;
            }
            set
            {
                if (query == null || !query.Equals(value))
                {
                    query = value;

                }
                OnPropertyChanged("Query");
            }
        }
        private int popupIndex;
        private int popupNumberOfEntries;
        private IEnumerable<Term> popupResults;

        public MainWindow()
        {
            InitializeComponent();
            engine = new DictionaryEngine();
            results = new ObservableCollection<Term>();
            DictionaryRenderer.rd = Resources;
            this.DataContext = this;

            fdViewer.AddHandler(MouseUpEvent, new MouseButtonEventHandler(fdViewer_MouseUp), true);
            //fdViewer.Selection.Changed += fdViewer_MouseUp;
        }

        private async void Search(string query)
        {
            cts.Cancel();
            cts = new CancellationTokenSource();

            if (string.IsNullOrWhiteSpace(query))
                return;
            try
            {
                var newResults = await engine.Search(cts.Token, query);
                results.Clear();
                foreach (var result in newResults.Take(1000))
                {
                    results.Add(result);
                }
            }
            catch (Exception e) when (e is TaskCanceledException | e is OperationCanceledException)
            {
                // ok
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            searchMode = (SearchMode)(((int)searchMode + 1) % Enum.GetNames(typeof(SearchMode)).Length);
            button.Content = searchMode.ToString();
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search(query);
            Debug.WriteLine("Query = " + Query);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        protected override void OnClosed(EventArgs e)
        {
            Properties.Settings.Default.LastQuery = Query;
            Properties.Settings.Default.Save();
            base.OnClosed(e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.LastQuery))
            {
                Query = "";
            }
            else
            {
                Query = Properties.Settings.Default.LastQuery;
            }
            base.OnInitialized(e);
        }

        private async void fdViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (fdViewer.Selection != null)
            {
                var selectionStart = fdViewer.Selection.Start;
                var selectionEnd = fdViewer.Selection.End;
                var selection = new TextRange(selectionStart, selectionEnd).Text;

                if (selection.Length > 0)
                {
                    CancellationTokenSource cts = new CancellationTokenSource();
                    var tabIndex = selection.IndexOf('\t');
                    if (tabIndex != -1)
                    {
                        selection = selection.Substring(tabIndex + 1);
                    }
                    popupResults = await engine.SearchExact(cts.Token, selection);
                    popupNumberOfEntries = popupResults.Count();
                    if (popupNumberOfEntries > 0) {
                        popupIndex = 0;
                        popupNext.Visibility = popupPrev.Visibility = (Visibility)(new BooleanToVisibilityConverter().Convert(popupNumberOfEntries > 1, null, null, null));
                        popupPrev.IsEnabled = false;
                        popupViewer.Document = DictionaryRenderer.Render(popupResults.First(), engine, true);
                        popup.IsOpen = true;

                        ScrollPopupToTop();
                    }
                }
                //var pos = e.GetPosition(fdViewer);
            }
        }

        private void ScrollPopupToTop()
        {
            DependencyObject obj = popupViewer;

            do
            {
                if (VisualTreeHelper.GetChildrenCount(obj) > 0)
                    obj = VisualTreeHelper.GetChild(obj as Visual, 0);
                else
                    break;
            }
            while (!(obj is ScrollViewer));

            var scrollViewer = obj as ScrollViewer;
            scrollViewer.ScrollToTop();
        }

        private void popupPrev_Click(object sender, RoutedEventArgs e)
        {
            if (popupIndex > 0)
            {
                popupIndex--;
                popupViewer.Document = DictionaryRenderer.Render(popupResults.ElementAt(popupIndex), engine);
                ScrollPopupToTop();
                if (popupIndex == 0)
                {
                    popupPrev.IsEnabled = false;
                }
                popupNext.IsEnabled = true;
            }
        }

        private void popupNext_Click(object sender, RoutedEventArgs e)
        {
            if (popupIndex + 1 < popupNumberOfEntries)
            {
                popupIndex++;
                popupViewer.Document = DictionaryRenderer.Render(popupResults.ElementAt(popupIndex), engine);
                ScrollPopupToTop();
                if (popupIndex + 1 == popupNumberOfEntries)
                {
                    popupNext.IsEnabled = false;
                }
                popupPrev.IsEnabled = true;
            }
        }

        private void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            Thumb t = (Thumb)sender;

            t.Cursor = Cursors.SizeNWSE;
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double yadjust = popupViewer.Height + e.VerticalChange;

            double xadjust = popupViewer.Width + e.HorizontalChange;

            if ((xadjust >= 0) && (yadjust >= 0))

            {

                popupViewer.Width = xadjust;

                popupViewer.Height = yadjust;

            }
        }

        private void Thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Thumb t = (Thumb)sender;

            t.Cursor = null;
        }

        private void Thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            Thumb t = (Thumb)sender;

            t.Cursor = Cursors.SizeNWSE;
        }

        private void Thumb_MouseLeave(object sender, MouseEventArgs e)
        {
            Thumb t = (Thumb)sender;

            t.Cursor = null;
        }
    }

    public class DefinitionListToConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
