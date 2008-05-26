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
        Tetromino block = new Tetromino();
        List<MyGraphicObject> currentObject = new List<MyGraphicObject>();
        List<MyGraphicObject> groundObject = new List<MyGraphicObject>();

        Size blockS = new Size();
        Point startP = new Point();
        Pen pen = new Pen(Color.Black,1);
        Brush brush;

        public Main()
        {
            InitializeComponent();

            //Eigenschaften der Oberfläche definieren
            this.ClientSize = new Size(500, 500);

            //Eigenschaften der Objekte definieren
            blockS.Width = this.ClientSize.Width / 10;
            blockS.Height = this.ClientSize.Width / 10;
            startP.X = (this.ClientSize.Width / 2) - (blockS.Width / 2);
            startP.Y = blockS.Height;
            pen.Color = Color.Black;
            pen.Width = 1;
            brush = Brushes.Navy;

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
            
            //altes Objekt löschen
            currentObject.Clear();
            
            //neues zufälliges Objekt generieren
            startP.X = (this.ClientSize.Width / 2) - (blockS.Width / 2);
            startP.Y = blockS.Height;
            block.ChangeType(TetronType.T);
            for (int i1 = 0; i1 < 4; i1++)
            {
                currentObject.Add(new MyRectangle(block.Pen, block.Brush, new Rectangle(
                    startP.X + block.Points[block.ObjectRotation, i1, 0] * blockS.Width,
                    startP.Y + block.Points[block.ObjectRotation, i1, 1] * blockS.Height,
                    blockS.Width, blockS.Height)));
            }
          
            //Zeichenfläche aktualisieren
            this.Invalidate();
        }

        private void RotateObject()
        {
            startP.X = currentObject[0].Position().X;
            startP.Y = currentObject[0].Position().Y;

            block.Rotate();

            bool rotationCollision = false;
            for (int i1 = 0; i1 < 4; i1++)
            {
                MyGraphicObject tmpGo = new MyRectangle(block.Pen, block.Brush, new Rectangle(
                        startP.X + block.Points[block.ObjectRotation, i1, 0] * blockS.Width,
                        startP.Y + block.Points[block.ObjectRotation, i1, 1] * blockS.Height,
                        blockS.Width, blockS.Height));

                if ((BorderCollision(0, 0, tmpGo) == true) ||
                    (GroundCollision(0, 0, tmpGo) == true))
                {
                    rotationCollision = true;
                }
            }

            if (rotationCollision == true)
            {
                block.RotateBack();
            }
            else
            {
                currentObject.Clear();
                for (int i1 = 0; i1 < 4; i1++)
                {
                    currentObject.Add(new MyRectangle(block.Pen, block.Brush, new Rectangle(
                        startP.X + block.Points[block.ObjectRotation, i1, 0] * blockS.Width,
                        startP.Y + block.Points[block.ObjectRotation, i1, 1] * blockS.Height,
                        blockS.Width, blockS.Height)));
                }
                this.Invalidate();
            }
        }

        private void MoveObject(int deltaX, int deltaY)
        {
            bool borderCollision = false;
            foreach (MyGraphicObject goCurrentObject in currentObject)
            {
                if (BorderCollision(deltaX, deltaY, goCurrentObject) == true)
                {
                    borderCollision = true;
                    break;
                }
            }
            bool groundCollision = false;
            foreach (MyGraphicObject goCurrentObject in currentObject)
            {
                if (GroundCollision(deltaX, deltaY, goCurrentObject) == true)
                {
                    groundCollision = true;
                    break;
                }
            }

            if (borderCollision == false && groundCollision == false)
            {
                foreach (MyGraphicObject goCurrentObject in currentObject)
                {
                    goCurrentObject.Move(deltaX * blockS.Width, deltaY * blockS.Height);
                }
                this.Invalidate();
            }
            else if (groundCollision == true)
            {
                AddObject();
            }
        }

        private bool BorderCollision(int deltaX, int deltaY, MyGraphicObject goCurrentObject)
        {
            bool borderCollision = false;
            if ((goCurrentObject.Position().X + (deltaX * blockS.Width) <= this.ClientSize.Width - blockS.Width) &&
                (goCurrentObject.Position().X + (deltaX * blockS.Width) >= blockS.Width / 2))
            {
                foreach (MyGraphicObject go in groundObject)
                {
                    if ((go.Position().X == goCurrentObject.Position().X + (deltaX * blockS.Width)) &&
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
            return borderCollision;
        }

        private bool GroundCollision(int deltaX, int deltaY, MyGraphicObject goCurrentObject)
        {
            bool groundCollision = false;
            if (goCurrentObject.Position().Y + (deltaY * blockS.Height) <= this.ClientSize.Height - blockS.Height)
            {
                foreach (MyGraphicObject go in groundObject)
                {
                    if ((go.Position().Y == goCurrentObject.Position().Y + blockS.Height) &&
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
            return groundCollision;
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Left:
                case Keys.A:
                    MoveObject(-1, 0);
                    break;
                case Keys.Right:
                case Keys.D:
                    MoveObject(1, 0);
                    break;
                case Keys.Up:
                case Keys.W:
                    RotateObject();
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }
    }
}