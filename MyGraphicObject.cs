using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Tetris
{
    public class MyGraphicObject
    {
        Pen _pen;
        Brush _brush;
        Rectangle _bounds;
        Control _control;
        GraphicsPath _path = new GraphicsPath();

        public MyGraphicObject(Control control, Pen pen, Brush brush)
        {
            _pen = pen;
            _brush = brush;
            _control = control;
        }

        public GraphicsPath Path
        {
            get { return _path; }
        }

        public Pen Pen
        {
            get { return _pen; }
            set { _pen = value; }
        }

        public Brush Brush
        {
            get { return _brush; }
            set { _brush = value; }
        }

        public void SetBounds()
        {
            _bounds = Rectangle.Ceiling(_path.GetBounds());
            _bounds.X -= 1;
            _bounds.Width += 2;
            _bounds.Y -= 1;
            _bounds.Height += 2;
        }

        public void ApplyChanges()
        {
            _control.Invalidate(_bounds);
            SetBounds();
            _control.Invalidate(_bounds);
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
        public RectangleF GetRectangle()
        {
            return Path.GetBounds();
        }
    }

    public class MyRectangle : MyGraphicObject
    {
        public MyRectangle(Control control, Pen pen, Brush brush, Rectangle rect)
            : base(control, pen, brush)
        {
            Path.AddRectangle(rect);
            SetBounds();
        }

    }

    public class MyCircle : MyGraphicObject
    {
        public MyCircle(Control control, Pen pen, Brush brush, Point center, int radius)
            : base(control, pen, brush)
        {
            Path.AddEllipse(center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
            SetBounds();
        }
    }

    public class MyLine : MyGraphicObject
    {
        public MyLine(Control control, Pen pen, Brush brush, Point start, Point end)
            : base(control, pen, brush)
        {
            Path.AddLine(start, end);
            SetBounds();
        }
    }

    public class MyText : MyGraphicObject
    {
        public MyText(Control control, Pen pen, Brush brush, string text, FontFamily family, FontStyle style, float emSize, Point origin)
            : base(control, pen, brush)
        {
            Path.AddString(text, family, (int)style, emSize, origin, null);
            SetBounds();
            //Path.GetType()
        }
    }
}
