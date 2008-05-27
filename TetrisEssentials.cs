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
        MyGraphicObject groundObject2;

        Point fieldP = new Point();
        Size fieldS = new Size();
        Size blockS = new Size();
        Point startP = new Point();
        Pen pen = new Pen(Color.Black, 1);

        private void AddObject()
        {
            //altes Objekt mit Boden verbinden
            foreach (MyGraphicObject go in currentObject)
            {
                groundObject.Add(go);
            }

            //altes Objekt löschen
            currentObject.Clear();

            //Checken, ob eine Reihe fertig ist


            //neues zufälliges Objekt generieren
            startP.X = fieldP.X + (fieldS.Width / 2) - blockS.Width;
            startP.Y = fieldP.Y + blockS.Height;

            // Verändere den Typ des blocks
            block.ChangeType(GenerateRandomTetronType());

            //Neuen Block erstellen
            for (int i1 = 0; i1 < 4; i1++)
            {
                currentObject.Add(new MyRectangle(this, block.Pen, block.Brush, new Rectangle(
                    startP.X + block.Points[block.ObjectRotation, i1, 0] * blockS.Width,
                    startP.Y + block.Points[block.ObjectRotation, i1, 1] * blockS.Height,
                    blockS.Width, blockS.Height)));
                //Zeichenfläche aktualisieren
                currentObject[i1].ApplyChanges();
            }

            //GameOver-Bedingung überprüfen
            bool groundCollision = false;
            foreach (MyGraphicObject goCurrentObject in currentObject)
            {
                if (GroundCollision(0, 0, goCurrentObject) == true)
                {
                    groundCollision = true;
                    break;
                }
            }
            if (groundCollision == true)
            {
                //GameOver
                GameOver = true;
            }
        }

        private void CheckLines()
        {
            
            //groundObject2.Path.
            
            /*
            for (int i1 = fieldS.Height - blockS.Height; i1 >= 0; i1 -= blockS.Height)
            {

                bool noBlock = false;
                do
                {
                    groundObject[
                }
                while (noBlock != true);
            }
             * */
        }

        private void RotateObject()
        {
            startP.X = currentObject[0].Position().X;
            startP.Y = currentObject[0].Position().Y;
            short oldRotation = block.ObjectRotation;

            block.Rotate();

            bool rotationCollision = false;
            for (int i1 = 0; i1 < 4; i1++)
            {
                MyGraphicObject tmpGo = new MyRectangle(this, block.Pen, block.Brush, new Rectangle(
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
                for (int i1 = 0; i1 < 4; i1++)
                {
                    currentObject[i1].Move(
                        (block.Points[block.ObjectRotation, i1, 0] - block.Points[oldRotation, i1, 0]) * blockS.Width,
                        (block.Points[block.ObjectRotation, i1, 1] - block.Points[oldRotation, i1, 1]) * blockS.Height);
                    //Zeichenfläche aktualisieren
                    currentObject[i1].ApplyChanges();
                }
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
                    //Zeichenfläche aktualisieren
                    goCurrentObject.ApplyChanges();
                }
            }
            else if (groundCollision == true)
            {
                AddObject();
            }
        }

        private bool BorderCollision(int deltaX, int deltaY, MyGraphicObject goCurrentObject)
        {
            bool borderCollision = false;
            if ((goCurrentObject.Position().X + (deltaX * blockS.Width) <= fieldP.X + fieldS.Width - blockS.Width) &&
                (goCurrentObject.Position().X + (deltaX * blockS.Width) >= fieldP.X))
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
            if (goCurrentObject.Position().Y + (deltaY * blockS.Height) <= fieldP.Y + fieldS.Height - blockS.Height)
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
    }
}
