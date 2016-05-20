using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
            //List<PathSegment> path;
            PathFigure path = null;
            List<Point> track = null;
            bool isTrack = false;

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
                                    track.Add(new Point(float.Parse(reader.GetAttribute("x")), float.Parse(reader.GetAttribute("y"))));
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
                                track = stroke.track;
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

        public class Stroke
        {
            public PathFigure outline = new PathFigure() { };
//            public PathFigure track = new PathFigure() { };
//            public List<PathSegment> outline = new List<PathSegment>();
            public List<Point> track = new List<Point>();
        }
    }
}
