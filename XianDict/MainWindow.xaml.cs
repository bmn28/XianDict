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
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace XianDict
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum SearchMode
        {
            中, 英
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

        IntPtr nextClipboardViewer;
        ClipboardViewer clipboardViewer;
        GridLength clipboardHeight = GridLength.Auto;
        GridLength radicalsHeight = new GridLength(300);

        RadicalInput radicalInput;

        public MainWindow()
        {
            InitializeComponent();
            engine = new DictionaryEngine();
            results = new ObservableCollection<Term>();
            this.Resources.MergedDictionaries.Add((ResourceDictionary)Application.LoadComponent(new Uri("DefaultSkin.xaml", UriKind.RelativeOrAbsolute)));
            this.Resources.MergedDictionaries.Add((ResourceDictionary)Application.LoadComponent(new Uri("DarkSkin.xaml", UriKind.RelativeOrAbsolute)));
            DictionaryRenderer.rd = Resources;
            this.DataContext = this;
            clipboardViewer = new ClipboardViewer(this);
            radicalInput = new RadicalInput(this);

            fdViewer.AddHandler(MouseUpEvent, new MouseButtonEventHandler(fdViewer_MouseUp), true);
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
            ChangeClipboardChain(new WindowInteropHelper(this).Handle, nextClipboardViewer);
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

        public async void fdViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var fdViewer = (FlowDocumentScrollViewer)sender;
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
            popupViewer.Height = popupViewer.ActualHeight;
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double yadjust = popupViewer.Height + e.VerticalChange;
            double xadjust = popupViewer.Width + e.HorizontalChange;
            popupViewer.MaxHeight = double.PositiveInfinity;
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
            if (popupViewer.Height < 768)
            {
                popupViewer.MaxHeight = 768;
            }
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

        private void DisplayClipboardData()
        {
            try
            {
                clipboardViewer.UpdateText(Clipboard.GetText());
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private void clipboardToggle_Checked(object sender, RoutedEventArgs e)
        {
            radicalsToggle.IsChecked = false;
            frame.Content = clipboardViewer;
            frame.Visibility = Visibility.Visible;
            frameGridSplitter.Visibility = Visibility.Visible;
            panelRow.Height = clipboardHeight;
        }

        private void clipboardToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            clipboardHeight = panelRow.Height;
            panelRow.Height = GridLength.Auto;
            frame.Visibility = Visibility.Collapsed;
            frameGridSplitter.Visibility = Visibility.Collapsed;
        }

        private void radicalsToggle_Checked(object sender, RoutedEventArgs e)
        {
            clipboardToggle.IsChecked = false;
            frame.Content = radicalInput;
            frame.Visibility = Visibility.Visible;
            frameGridSplitter.Visibility = Visibility.Visible;
            panelRow.Height = radicalsHeight;
        }

        private void radicalsToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            radicalsHeight = panelRow.Height;
            panelRow.Height = GridLength.Auto;
            frame.Visibility = Visibility.Collapsed;
            frameGridSplitter.Visibility = Visibility.Collapsed;
        }

        public void AppendToSearch(string input)
        {
            searchBox.Text += input;
        }

        public void Backspace()
        {
            if (searchBox.Text.Length > 0)
            {
                searchBox.Text = searchBox.Text.Remove(searchBox.Text.Length - 1);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            panelRow.MaxHeight = Math.Max(0, this.ActualHeight - 130);
        }

        #region Win32 Clipboard functionality

        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        protected static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        protected static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);



        /// <summary>
        /// AddHook Handle WndProc messages in WPF
        /// This cannot be done in a Window's constructor as a handle window handle won't at that point, so there won't be a HwndSource.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(WndProc);

                nextClipboardViewer = (IntPtr)SetClipboardViewer((int)new WindowInteropHelper(this).Handle);
            }
        }

        /// <summary>
        /// WndProc matches the HwndSourceHook delegate signature so it can be passed to AddHook() as a callback. This is the same as overriding a Windows.Form's WncProc method.
        /// </summary>
        /// <param name="hwnd">The window handle</param>
        /// <param name="msg">The message ID</param>
        /// <param name="wParam">The message's wParam value, historically used in the win32 api for handles and integers</param>
        /// <param name="lParam">The message's lParam value, historically used in the win32 api to pass pointers</param>
        /// <param name="handled">A value that indicates whether the message was handled</param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_DRAWCLIPBOARD = 0x0308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (msg)
            {
                case WM_DRAWCLIPBOARD:
                    DisplayClipboardData();
                    SendMessage(nextClipboardViewer, msg, wParam, lParam);
                    break;
                case WM_CHANGECBCHAIN:
                    if (wParam == nextClipboardViewer)
                        nextClipboardViewer = lParam;
                    else
                        SendMessage(nextClipboardViewer, msg, wParam, lParam);
                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }
        #endregion
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
