using System;
using System.Collections.Generic;
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
        //DrawingGroup strokeGroup;
        GeometryDrawing background = new GeometryDrawing(Brushes.Transparent, null, new RectangleGeometry(new Rect(new Size(2150, 2150))));
        GeometryDrawing grid1;
        GeometryDrawing strokeLines;
        GeometryDrawing grid2;
        GeometryDrawing character;
        GeometryDrawing inverse;
        StrokeWord word;
        Stopwatch stopwatch;
        int currentStroke;
        int strokeCount;
        double progress = 0;

        public StrokeDisplay()
        {
            StrokeWord word;
            InitializeComponent();
            using (FileStream fs = File.OpenRead(@"..\..\..\XianDict\bin\Debug\utf8\61ff.xml"))
            {
                word = new StrokeWord(fs);
            }
            //drawingImage.Drawing = word.ToDrawing();
            SetDrawing(word);

            BeginAnimate();

            //var children = ((DrawingGroup)drawingImage.Drawing).Children;
            //foreach (var stroke in word.Strokes)
            //{
            //    foreach (var segment in stroke.trackSegments)
            //    {
            //        var d = new GeometryDrawing();
            //        d.Geometry = segment;
            //        d.Pen = new Pen(Brushes.Green, 50);
            //        DoubleAnimation xa = new DoubleAnimation();
            //        xa.From = segment.StartPoint.X;
            //        xa.To = segment.EndPoint.X;
            //        children.Add(d);
            //        PointAnimation pa = new PointAnimation();
            //        pa.From = segment.StartPoint;
            //        pa.To = segment.EndPoint;
            //        pa.AutoReverse = true;
            //        segment.BeginAnimation(LineGeometry.EndPointProperty, pa);
            //    }
            //}
        }

        public void SetDrawing(StrokeWord word)
        {
            this.word = word;
            //var group = new DrawingGroup();

            group.Children.Add(background);

            grid1 = CreateGrid();
            group.Children.Add(grid1);

            character = new GeometryDrawing(Brushes.Gray, null, new PathGeometry(word.Strokes.Select(x => x.outline)) { FillRule = FillRule.Nonzero });
            group.Children.Add(character);

            //strokeGroup = new DrawingGroup();
            //group.Children.Add(strokeGroup);

            inverse = new GeometryDrawing(Brushes.White, null,
                Geometry.Combine(background.Geometry, character.Geometry, GeometryCombineMode.Xor, null));
            group.Children.Add(inverse);

            strokeLines = new GeometryDrawing();
            var strokeGroup = new GeometryGroup();
            foreach (var stroke in word.Strokes)
            {
                foreach (var segment in stroke.trackSegments)
                {
                    strokeGroup.Children.Add(segment);
                }
            }
            strokeLines.Pen = new Pen(Brushes.Black, 150);
            strokeLines.Pen.StartLineCap = strokeLines.Pen.EndLineCap = PenLineCap.Round;
            strokeLines.Geometry = strokeGroup;
            group.Children.Add(strokeLines);

            //DoubleAnimation da = new DoubleAnimation();
            //da.From = 30;
            //da.To = 100;
            //da.Duration = new Duration(TimeSpan.FromSeconds(1));
            //strokeLines.BeginAnimation(GeometryGroup., da);

            //drawingImage.Drawing = group;
        }

        private void Animate(object sender, EventArgs e)
        {
            group.Children.Clear();
            group.Children.Add(character);

            //int strokeToDraw = (int)Math.Round((stopwatch.ElapsedMilliseconds / 10000) / (double)strokeCount);

            progress = stopwatch.ElapsedMilliseconds / (double)1000;
            DrawStroke(currentStroke, progress);
            if (progress > 1)
            {
                stopwatch.Restart();
                progress = 0;
                currentStroke++;
                if (currentStroke == strokeCount)
                {
                    CompositionTarget.Rendering -= Animate;
                }
            }

            //group.Children.Add(strokeLines);
            group.Children.Add(inverse);
            group.Children.Add(grid1);

        }

        private void BeginAnimate()
        {
            stopwatch = new Stopwatch();
            strokeCount = word.Strokes.Count;
            progress = 0;
            currentStroke = 0;
            stopwatch.Start();
            CompositionTarget.Rendering += Animate;
        }

        private void DrawStroke(int i, double progress)
        {
            //var children = ((GeometryGroup)strokeLines.Geometry).Children;
            var children = group.Children;
            //children.Clear();

            List<float>.Enumerator widthEnum;
            int j;
            var strokeEnum = word.Strokes.GetEnumerator();
            StrokeWord.Stroke stroke;
            for (j = 0; j < i; j++)
            {
                strokeEnum.MoveNext();
                stroke = strokeEnum.Current;

                widthEnum = stroke.segmentWidths.GetEnumerator();
                foreach (var segment in stroke.trackSegments)
                {
                    widthEnum.MoveNext();
                    children.Add(new GeometryDrawing(null, new Pen(Brushes.Black, widthEnum.Current) {StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round }, segment));
                }
            }
            strokeEnum.MoveNext();
            stroke = strokeEnum.Current;
            double cumLength = 0;
            widthEnum = stroke.segmentWidths.GetEnumerator();
            foreach (var segment in stroke.trackSegments)
            {
                widthEnum.MoveNext();
                double dx = segment.EndPoint.X - segment.StartPoint.X;
                double dy = segment.EndPoint.Y - segment.StartPoint.Y;
                double segmentLength = Math.Sqrt(dx * dx + dy * dy);
                cumLength += segmentLength;
                if (cumLength / stroke.Length > progress)
                {
                    double segmentProgress = Math.Min(1, (segmentLength - (cumLength - (progress * stroke.Length)))/ segmentLength);
                    Point interp = new Point(segment.StartPoint.X + dx * segmentProgress, segment.StartPoint.Y + dy * segmentProgress);
                    //children.Add(new LineGeometry(segment.StartPoint, interp));
                    children.Add(new GeometryDrawing(null, new Pen(Brushes.Black, widthEnum.Current) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round }, new LineGeometry(segment.StartPoint, interp)));
                    break;
                }
                children.Add(new GeometryDrawing(null, new Pen(Brushes.Black, widthEnum.Current) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round }, segment));
            }



            //strokeLines.Pen = new Pen(Brushes.Green, 300);
            //strokeLines.Pen.StartLineCap = strokeLines.Pen.EndLineCap = PenLineCap.Round;



            //strokeLines.Geometry = strokeGroup;
            //group.Children.Add(strokeLines);
        }

        private GeometryDrawing CreateGrid()
        {
            var grid = new GeometryDrawing();
            grid.Pen = new Pen(Brushes.Black, 1);
            var gridGroup = new GeometryGroup();
            var nw = new Point(0, 0);
            var n = new Point(1075, 0);
            var ne = new Point(2150, 0);
            var w = new Point(0, 1075);
            var e = new Point(2150, 1075);
            var sw = new Point(0, 2150);
            var s = new Point(1075, 2150);
            var se = new Point(2150, 2150);
            gridGroup.Children.Add(new LineGeometry(nw, se));
            gridGroup.Children.Add(new LineGeometry(n, s));
            gridGroup.Children.Add(new LineGeometry(ne, sw));
            gridGroup.Children.Add(new LineGeometry(w, e));
            grid.Geometry = gridGroup;

            return grid;
        }
    }
}
