using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;
using System.Xml.Linq;

namespace XianDict
{
    public class StrokeWord
    {
        public string Character { get; }
        public List<Stroke> Strokes { get; }

        public StrokeWord(Stream input)
        {
            Strokes = new List<Stroke>();

            Stroke stroke = null;
            PathFigure path = null;
            List<Point> track = null;
            List<LineGeometry> trackSegments = null;
            bool isTrack = false;
            bool startOfTrack = false;
            Point lastPoint = new Point();

            using (XmlReader reader = XmlReader.Create(input))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name.ToString())
                        {
                            case "MoveTo":
                                if (isTrack)
                                {
                                    var nextPoint = new Point(float.Parse(reader.GetAttribute("x")), float.Parse(reader.GetAttribute("y")));
                                    track.Add(nextPoint);
                                    if (!startOfTrack)
                                    {
                                        var segment = new LineGeometry(lastPoint, nextPoint);
                                        trackSegments.Add(segment);
                                    }
                                    lastPoint = nextPoint;
                                    startOfTrack = false;
                                }
                                else
                                {
                                    path.StartPoint = (new Point(float.Parse(reader.GetAttribute("x")), float.Parse(reader.GetAttribute("y"))));
                                }
                                break;
                            case "LineTo":
                                path.Segments.Add(new LineSegment(new Point(float.Parse(reader.GetAttribute("x")), float.Parse(reader.GetAttribute("y"))), true));
                                break;
                            case "QuadTo":
                                path.Segments.Add(new QuadraticBezierSegment(new Point(float.Parse(reader.GetAttribute("x1")), float.Parse(reader.GetAttribute("y1"))),
                                    new Point(float.Parse(reader.GetAttribute("x2")), float.Parse(reader.GetAttribute("y2"))), true));
                                break;
                            case "Track":
                                isTrack = true;
                                startOfTrack = true;
                                track = stroke.track;
                                trackSegments = stroke.trackSegments;
                                break;
                            case "Outline":
                                isTrack = false;
                                path = stroke.outline;
                                break;
                            case "Stroke":
                                stroke = new Stroke();
                                Strokes.Add(stroke);
                                break;
                            case "Word":
                                Character = reader.GetAttribute("unicode");
                                break;
                        }
                    }
                }
            }

        }

        //public DrawingGroup ToDrawing()
        //{
        //    var group = new DrawingGroup();

        //    var background = new GeometryDrawing();
        //    background.Brush = Brushes.Transparent;
        //    background.Geometry = new RectangleGeometry(new Rect(new Size(2150, 2150)));
        //    group.Children.Add(background);

        //    var grid = new GeometryDrawing();
        //    grid.Pen = new Pen(Brushes.Black, 1);
        //    var gridGroup = new GeometryGroup();
        //    var nw = new Point(0, 0);
        //    var n = new Point(1075, 0);
        //    var ne = new Point(2150, 0);
        //    var w = new Point(0, 1075);
        //    var e = new Point(2150, 1075);
        //    var sw = new Point(0, 2150);
        //    var s = new Point(1075, 2150);
        //    var se = new Point(2150, 2150);
        //    gridGroup.Children.Add(new LineGeometry(nw, se));
        //    gridGroup.Children.Add(new LineGeometry(n, s));
        //    gridGroup.Children.Add(new LineGeometry(ne, sw));
        //    gridGroup.Children.Add(new LineGeometry(w, e));
        //    grid.Geometry = gridGroup;
        //    group.Children.Add(grid);

        //    var character = new GeometryDrawing();
        //    character.Brush = Brushes.Black;
        //    character.Geometry = new PathGeometry(Strokes.Select(x => x.outline)) { FillRule = FillRule.Nonzero };
        //    //group.Children.Add(character);

        //    var inverse = new GeometryDrawing();
        //    inverse.Brush = Brushes.Red;
        //    inverse.Geometry = Geometry.Combine(background.Geometry, character.Geometry, GeometryCombineMode.Xor, null);
        //    group.Children.Add(inverse);

        //    var strokeLines = new GeometryDrawing();
        //    var strokeGroup = new GeometryGroup();
        //    foreach (var stroke in Strokes)
        //    {
        //        foreach (var segment in stroke.trackSegments)
        //        {
        //            strokeGroup.Children.Add(segment);
        //        }
        //    }
        //    //trokeLines.
        //    strokeLines.Pen = new Pen(Brushes.Green, 300);
        //    strokeLines.Pen.StartLineCap = strokeLines.Pen.EndLineCap = PenLineCap.Round;
        //    strokeLines.Geometry = strokeGroup;
        //    group.Children.Add(strokeLines);

        //    DoubleAnimation da = new DoubleAnimation();
        //    da.From = 30;
        //    da.To = 100;
        //    da.Duration = new Duration(TimeSpan.FromSeconds(1));
        //    //strokeLines.BeginAnimation(GeometryGroup., da);

        //    return group;
        //}

        public class Stroke
        {
            public PathFigure outline = new PathFigure();
            public List<Point> track = new List<Point>();
            public List<LineGeometry> trackSegments = new List<LineGeometry>();
        }
    }
}
