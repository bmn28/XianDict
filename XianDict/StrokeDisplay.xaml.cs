using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XianDict
{
    /// <summary>
    /// Interaction logic for StrokeDisplay.xaml
    /// </summary>
    public partial class StrokeDisplay : Window
    {
        private static int canvasSize = 2050;

        GeometryDrawing background = new GeometryDrawing(Brushes.Transparent, null, new RectangleGeometry(new Rect(new Size(canvasSize, canvasSize))));
        GeometryDrawing grid = CreateGrid(canvasSize);
        GeometryDrawing character;
        StrokeWord word;
        List<StrokeWord> words;
        List<StrokeWord>.Enumerator enumerator;
        Stopwatch stopwatch;
        int currentStroke;
        int currentTrack;
        int strokeCount;
        double t = 0;
        bool waiting;

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

        public StrokeDisplay(char input)
        {
            InitializeComponent();
            this.DataContext = this;
            //AnimateAll();
            using (FileStream fs = File.OpenRead(getStrokeFile(input)))
            {
                SetWord(new StrokeWord(fs));
                BeginAnimate();
            }
        }

        public void SetWord(StrokeWord word)
        {
            this.word = word;
            group.Children.Clear();
            group.Children.Add(background);
            group.Children.Add(grid);

            character = new GeometryDrawing(Brushes.Gray, null,
                new PathGeometry(word.Strokes.Select(x => x.Outline)) { FillRule = FillRule.Nonzero });
            group.Children.Add(character);
        }
        public void Redraw()
        {
            group.Children.Clear();
            group.Children.Add(background);
            group.Children.Add(grid);
            character = new GeometryDrawing(Brushes.Gray, null,
                new PathGeometry(word.Strokes.Select(x => x.Outline)) { FillRule = FillRule.Nonzero });
            group.Children.Add(character);
        }

        private void BeginAnimate()
        {
            stopwatch = new Stopwatch();
            strokeCount = word.Strokes.Count;
            t = 0;
            currentStroke = 0;
            currentTrack = 0;
            waiting = true;
            stopwatch.Start();
            CompositionTarget.Rendering += Animate;
        }

        private void Animate(object sender, EventArgs e)
        {
            t = stopwatch.ElapsedMilliseconds / (double)200;
            if (waiting)
            {
                if (t > 0.5)
                {
                    waiting = false;
                    stopwatch.Restart();
                }
            }
            else
            {
                DrawTrack(currentStroke, currentTrack, t);
                if (t > 1)
                {
                    stopwatch.Restart();
                    t = 0;
                    currentTrack++;
                    if (currentTrack + 1 == word.Strokes[currentStroke].Tracks.Count)
                    {
                        currentTrack = 0;
                        currentStroke++;
                        waiting = true;
                        stopwatch.Restart();
                    }
                    if (currentStroke == strokeCount)
                    {
                        stopwatch.Stop();
                        CompositionTarget.Rendering -= Animate;
                        //AnimateNext();
                    }
                }
            }
        }

        private void DrawTrack(int strokeIndex, int trackIndex, double t)
        {
            var drawings = group.Children;

            var stroke = word.Strokes[strokeIndex];
            var outline = new PathGeometry();
            outline.Figures.Add(word.Strokes[strokeIndex].Outline);
            var inverse = Geometry.Combine(background.Geometry, outline, GeometryCombineMode.Xor, null);

            Point startPoint = stroke.Tracks[trackIndex].Point;
            Point endPoint = stroke.Tracks[trackIndex + 1].Point;

            double dx = endPoint.X - startPoint.X;
            double dy = endPoint.Y - startPoint.Y;
            double radius = 2 * stroke.Tracks[trackIndex].Size;

            if (t > 1)
            {
                t = 1;
            }

            Point p = new Point(startPoint.X + dx * ((t * 1.2) - 0.2), startPoint.Y + dy * ((t * 1.2) - 0.2));

            var circle = new EllipseGeometry(p, radius, radius);
            var clipped = Geometry.Combine(circle, inverse, GeometryCombineMode.Exclude, null);
            clipped = Geometry.Combine(clipped, background.Geometry, GeometryCombineMode.Intersect, null);
            var drawing = new GeometryDrawing(Brushes.Black, null, clipped);
            drawings.Add(drawing);
        }

        private void AnimateAll()
        {
            words = new List<StrokeWord>();
            var files = Directory.GetFiles(@"..\..\..\XianDict\bin\Debug\utf8");
            foreach (var file in files.Reverse())
            {
                using (FileStream fs = File.OpenRead(file))
                {
                    words.Add(new StrokeWord(fs));
                }
            }
            enumerator = words.GetEnumerator();
            AnimateNext();
        }

        private void AnimateNext()
        {
            if (enumerator.MoveNext())
            {
                word = enumerator.Current;
                SetWord(word);
                BeginAnimate();
            }
        }

        private static GeometryDrawing CreateGrid(int canvasSize)
        {
            var grid = new GeometryDrawing();
            grid.Pen = new Pen(Brushes.Black, 1);
            var gridGroup = new GeometryGroup();
            var nw = new Point(0, 0);
            var n = new Point(canvasSize / 2, 0);
            var ne = new Point(canvasSize, 0);
            var w = new Point(0, canvasSize / 2);
            var e = new Point(canvasSize, canvasSize / 2);
            var sw = new Point(0, canvasSize);
            var s = new Point(canvasSize / 2, canvasSize);
            var se = new Point(canvasSize, canvasSize);
            gridGroup.Children.Add(new LineGeometry(nw, se));
            gridGroup.Children.Add(new LineGeometry(n, s));
            gridGroup.Children.Add(new LineGeometry(ne, sw));
            gridGroup.Children.Add(new LineGeometry(w, e));
            grid.Geometry = gridGroup;

            return grid;
        }

        //private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    var utf8 = new UTF8Encoding();
        //    var bytes = utf8.GetBytes(textBox.Text);
        //    BitConverter.ToString(bytes);
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void animateButton_Click(object sender, RoutedEventArgs e)
        {
            Redraw();
            BeginAnimate();
        }

        public static string getStrokeFile(char input)
        {
            return @"utf8\" + ((uint)input).ToString("X4") + ".xml";
        }

        public static bool hasStrokeFile(char input)
        {
            string utf8 = ((uint)input).ToString("X4");
            return File.Exists(@"utf8\" + utf8 + ".xml");
        }
    }
}
