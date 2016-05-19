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
        private string query;
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
                        fdViewer.Document = DictionaryRenderer.Render(selectedEntry, engine, Resources);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        private CancellationTokenSource cts = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
            engine = new DictionaryEngine();
            results = new ObservableCollection<Term>();
            this.DataContext = this;
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
