using System;
using System.Collections.Generic;
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
        DrawingGroup strokeGroup;

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

            var children = ((DrawingGroup)drawingImage.Drawing).Children;
            foreach (var stroke in word.Strokes)
            {
                foreach (var segment in stroke.trackSegments)
                {
                    var d = new GeometryDrawing();
                    d.Geometry = segment;
                    d.Pen = new Pen(Brushes.Green, 50);
                    DoubleAnimation xa = new DoubleAnimation();
                    xa.From = segment.StartPoint.X;
                    xa.To = segment.EndPoint.X;
                    children.Add(d);
                    PointAnimation pa = new PointAnimation();
                    pa.From = segment.StartPoint;
                    pa.To = segment.EndPoint;
                    pa.AutoReverse = true;
                    segment.BeginAnimation(LineGeometry.EndPointProperty, pa);
                }
            }
        }

        public void SetDrawing(StrokeWord word)
        {
            //var group = new DrawingGroup();

            var background = new GeometryDrawing();
            background.Brush = Brushes.Transparent;
            background.Geometry = new RectangleGeometry(new Rect(new Size(2150, 2150)));
            group.Children.Add(background);

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
            group.Children.Add(grid);

            var character = new GeometryDrawing();
            character.Brush = Brushes.Black;
            character.Geometry = new PathGeometry(word.Strokes.Select(x => x.outline)) { FillRule = FillRule.Nonzero };
            //group.Children.Add(character);

            strokeGroup = new DrawingGroup();
            group.Children.Add(strokeGroup);

            var inverse = new GeometryDrawing();
            inverse.Brush = Brushes.Red;
            inverse.Geometry = Geometry.Combine(background.Geometry, character.Geometry, GeometryCombineMode.Xor, null);
            group.Children.Add(inverse);

            //var strokeLines = new GeometryDrawing();
            //var strokeGroup = new GeometryGroup();
            //foreach (var stroke in word.Strokes)
            //{
            //    foreach (var segment in stroke.trackSegments)
            //    {
            //        strokeGroup.Children.Add(segment);
            //    }
            //}
            //strokeLines.Pen = new Pen(Brushes.Green, 300);
            //strokeLines.Pen.StartLineCap = strokeLines.Pen.EndLineCap = PenLineCap.Round;
            //strokeLines.Geometry = strokeGroup;
            ////group.Children.Add(strokeLines);

            DoubleAnimation da = new DoubleAnimation();
            da.From = 30;
            da.To = 100;
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            //strokeLines.BeginAnimation(GeometryGroup., da);

            //drawingImage.Drawing = group;
        }

    }
}
