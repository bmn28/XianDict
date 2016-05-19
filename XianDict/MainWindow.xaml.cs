using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public ObservableCollection<IndexedTerm> results { get; set; }
        private IndexedTerm selectedEntry;
        public IndexedTerm SelectedEntry
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
                        //DisplayEntry(selectedEntry);
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
                    Search(query);
                }
                OnPropertyChanged("Query");
            }
        }

        private CancellationTokenSource cts = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
            engine = new DictionaryEngine();
            results = new ObservableCollection<IndexedTerm>();
            this.DataContext = this;

            Query = "";
        }

        private async void Search(string query)
        {
            //cts.Cancel(false);
            //cts.
            if (string.IsNullOrWhiteSpace(query))
                return;
            var newResults = await engine.Search(query);
            results.Clear();
            foreach (var result in newResults)
            {
                results.Add(result);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            searchMode = (SearchMode)(((int)searchMode + 1) % Enum.GetNames(typeof(SearchMode)).Length);
            button.Content = searchMode.ToString();
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
