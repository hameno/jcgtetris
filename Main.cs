using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Main : Form
    {
        List<MyGraphicObject> currentObject = new List<MyGraphicObject>();
        List<MyGraphicObject> groundObject = new List<MyGraphicObject>();

        public Main()
        {
            InitializeComponent();
            this.ClientSize = new Size(500, 500);
            //Damit geht das Neuzeichnen viel flüssiger
            this.DoubleBuffered = true;

            AddObject();

            Timer tCount = new Timer();
            tCount.Tick += new EventHandler(tCount_Tick);
            tCount.Interval = 500;
            tCount.Enabled = true;
        }

        void tCount_Tick(object sender, EventArgs e)
        {
            MoveObject(0,1);
        }

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.Clear(Color.White);
            foreach (MyGraphicObject go in currentObject)
            {
                go.Draw(e.Graphics);
            }
            foreach (MyGraphicObject go in groundObject)
            {
                go.Draw(e.Graphics);
            }
        }

        private void AddObject()
        {
            //altes Objekt mit Boden verbinden
            foreach (MyGraphicObject go in currentObject)
            {
                groundObject.Add(go);
            }
            
            //neues Objekt hinzufügen
            Pen pen = new Pen(Color.Black,1);
            Brush brush = Brushes.Navy;
            currentObject.Clear();
            
            //Eigenschaften der Objekte definieren
            Point startP = new Point();
            Size blockS = new Size();
            startP.X = (this.ClientSize.Width / 2) - (this.ClientSize.Width / 10 / 2);
            startP.Y = 0;
            blockS.Width = this.ClientSize.Width / 10;
            blockS.Height = this.ClientSize.Width / 10;

            //zufälliges Objekt generieren
            currentObject.Add(new MyRectangle(pen, brush, new Rectangle(startP.X, startP.Y, blockS.Width, blockS.Height)));
            currentObject.Add(new MyRectangle(pen, brush, new Rectangle(startP.X, startP.Y + blockS.Height, this.ClientSize.Width / 10, this.ClientSize.Width / 10)));
            currentObject.Add(new MyRectangle(pen, brush, new Rectangle(startP.X - blockS.Width, startP.Y + blockS.Height, this.ClientSize.Width / 10, this.ClientSize.Width / 10)));
            currentObject.Add(new MyRectangle(pen, brush, new Rectangle(startP.X + blockS.Width, startP.Y + blockS.Height, this.ClientSize.Width / 10, this.ClientSize.Width / 10)));
            
            //Zeichenfläche aktualisieren
            this.Invalidate();
        }

        private void RotateObject()
        {

        }

        private void MoveObject(int deltaX, int deltaY)
        {
            bool borderCollision = false;
            foreach (MyGraphicObject goCurrentObject in currentObject)
            {
                if ((goCurrentObject.Position().X + (deltaX * this.ClientSize.Width / 10) <= this.ClientSize.Width - (this.ClientSize.Width / 10)) &&
                    (goCurrentObject.Position().X + (deltaX * this.ClientSize.Width / 10) >= this.ClientSize.Width / 10 / 2))
                {
                    foreach (MyGraphicObject go in groundObject)
                    {
                        if ((go.Position().X == goCurrentObject.Position().X + (deltaX * this.ClientSize.Width / 10)) &&
                            (go.Position().Y == goCurrentObject.Position().Y))
                        {
                            borderCollision = true;
                            break;
                        }
                    }
                }
                else
                {
                    borderCollision = true;
                }
            }

            bool groundCollision = false;
            foreach (MyGraphicObject goCurrentObject in currentObject)
            {
                if (goCurrentObject.Position().Y + (deltaY * this.ClientSize.Width / 10) <= this.ClientSize.Height - (this.ClientSize.Width / 10))
                {
                    foreach (MyGraphicObject go in groundObject)
                    {
                        if ((go.Position().Y == goCurrentObject.Position().Y + (this.ClientSize.Width / 10)) &&
                            (go.Position().X == goCurrentObject.Position().X))
                        {
                            groundCollision = true;
                            break;
                        }
                    }
                }
                else
                {
                    groundCollision = true;
                }
            }

            if (borderCollision == false && groundCollision == false)
            {
                foreach (MyGraphicObject goCurrentObject in currentObject)
                {
                    goCurrentObject.Move(deltaX * this.ClientSize.Width / 10, deltaY * this.ClientSize.Width / 10);
                }
                this.Invalidate();
            }
            else if (groundCollision == true)
            {
                AddObject();
            }
        }

        private void Main_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch ((Keys)e.KeyChar)
            {
                case (Keys)'a':
                    MoveObject(-1, 0);
                    break;
                case (Keys)'d':
                    MoveObject(1, 0);
                    break;
                case (Keys)'w':
                    RotateObject();
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }
    }
}