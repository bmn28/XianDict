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
            List<Track> tracks = null;
            bool isTrack = false;
            Point p1;
            Point p2;

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
                                    p1 = new Point(double.Parse(reader.GetAttribute("x")), double.Parse(reader.GetAttribute("y")));
                                    double size = 150;
                                    var sizeString = reader.GetAttribute("size");
                                    if (!string.IsNullOrEmpty(sizeString))
                                    {
                                        size = double.Parse(sizeString);
                                    }
                                    tracks.Add(new Track() { Point = p1, Size = size });
                                }
                                else
                                {
                                    path.StartPoint = (new Point(double.Parse(reader.GetAttribute("x")), double.Parse(reader.GetAttribute("y"))));
                                }
                                break;
                            case "LineTo":
                                p1 = new Point(double.Parse(reader.GetAttribute("x")), double.Parse(reader.GetAttribute("y")));
                                path.Segments.Add(new LineSegment(p1, true));
                                break;
                            case "QuadTo":
                                p1 = new Point(double.Parse(reader.GetAttribute("x1")), double.Parse(reader.GetAttribute("y1")));
                                p2 = new Point(double.Parse(reader.GetAttribute("x2")), double.Parse(reader.GetAttribute("y2")));
                                path.Segments.Add(new QuadraticBezierSegment(p1, p2, true));
                                break;
                            case "Track":
                                isTrack = true;
                                tracks = stroke.Tracks;
                                //startOfTrack = true;
                                //trackSegments = stroke.trackSegments;
                                break;
                            case "Outline":
                                isTrack = false;
                                path = stroke.Outline;
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
            public PathFigure Outline = new PathFigure();
            public List<Track> Tracks = new List<Track>();

            //private double? length = null;
            //public double Length
            //{
            //    get
            //    {
            //        if (length == null)
            //        {
            //            length = 0;
            //            foreach (var segment in trackSegments)
            //            {
            //                double dx = segment.StartPoint.X - segment.EndPoint.X;
            //                double dy = segment.StartPoint.Y - segment.EndPoint.Y;
            //                double segmentLength = Math.Sqrt(dx*dx+dy*dy);
            //                length += segmentLength;
            //            }
            //        }
            //        return (double)length;
            //    }
            //}
        }

        public class Track
        {
            public Point Point { get; set; }
            public double Size { get; set; }
        }
    }
}
