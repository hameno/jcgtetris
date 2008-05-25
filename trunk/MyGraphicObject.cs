using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Tetris
{
    public abstract class MyGraphicObject
    {
        Pen _pen;
        Brush _brush;
        GraphicsPath _path = new GraphicsPath();

        public MyGraphicObject(Pen pen, Brush brush)
        {
            _pen = pen;
            _brush = brush;
        }

        protected GraphicsPath Path
        {
            get { return _path; }
        }

        public Pen Pen
        {
            get { return _pen; }
        }

        public Brush Brush
        {
            get { return _brush; }
        }

        public virtual void Draw(Graphics g)
        {
            g.DrawPath(_pen, _path);
            g.FillPath(_brush, _path);
        }

        /// <summary>
        /// Gibt die aktuelle Position des Objekts zurück.
        /// </summary>
        public virtual Point Position()
        {
            Point p = new Point();
            p.X = Convert.ToInt16(_path.PathPoints[0].X);
            p.Y = Convert.ToInt16(_path.PathPoints[0].Y);
            return (p);
        }
        
        /// <summary>
        /// Bewegt das Objekt um deltaX in x-Richtung und deltaY in y-Richtung.
        /// </summary>
        public virtual void Move(int deltaX, int deltaY)
        {
            Matrix mat = new Matrix();
            mat.Translate(deltaX, deltaY);
            _path.Transform(mat);
        }
    }

    public class MyRectangle : MyGraphicObject
    {
        public MyRectangle(Pen pen, Brush brush, Rectangle rect)
            : base(pen, brush)
        {
            Path.AddRectangle(rect);
        }
    }

    public class MyCircle : MyGraphicObject
    {
        public MyCircle(Pen pen, Brush brush, Point center, int radius)
            : base(pen, brush)
        {
            Path.AddEllipse(center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
        }
    }

    public class MyLine : MyGraphicObject
    {
        public MyLine(Pen pen, Brush brush, Point start, Point end)
            : base(pen, brush)
        {
            Path.AddLine(start, end);
        }
    }

    public class MyText : MyGraphicObject
    {
        public MyText(Pen pen, Brush brush, string text, FontFamily family, FontStyle style, float emSize, Point origin)
            : base(pen, brush)
        {
            Path.AddString(text, family, (int)style, emSize, origin, null);
        }
    }
}
