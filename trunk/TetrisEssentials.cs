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
        GraphicsPath groundObjects = new GraphicsPath();

        Point fieldP = new Point();
        Size fieldS = new Size();
        Size blockS = new Size();
        Point startP = new Point();
        Pen pen = new Pen(Color.Black, 1);

        private void AddObject()
        {
            //Altes Objekt zum Boden hinzufügen
            foreach (MyGraphicObject go in currentObject)
            {
                groundObject.Add(new MyRectangle(this, block.Pen, block.Brush, Rectangle.Round(go.GetRectangle())));
            }

            //Altes Objekt zu den zur Kollision nötigen Abfragen hinzufügen
            foreach (MyGraphicObject go in currentObject)
            {
                groundObjects.AddRectangle(Rectangle.Round(go.GetRectangle()));
            }

            //altes Objekt löschen
            currentObject.Clear();

            //Checken, ob eine Reihe fertig ist
            CheckLines();

            //neues zufälliges Objekt generieren
            startP.X = fieldP.X + (fieldS.Width / 2) - blockS.Width;
            startP.Y = fieldP.Y + blockS.Height;

            // Verändere den Typ des blocks
            block.ChangeType(nextStein);

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
            for (float i1 = fieldP.Y + fieldS.Height - (blockS.Height / 2); i1 >= fieldP.Y; i1 -= blockS.Height)
            {
                bool noBlock = false;
                for (float i2 = fieldP.X + (blockS.Width / 2); i2 <= fieldP.X + fieldS.Width; i2 += blockS.Width)
                {
                    if (!groundObjects.IsVisible(i2, i1))
                    {
                        noBlock = true;
                        break;
                    }
                }

                if (noBlock == false)
                {
                    List<int> objectsToRemove = new List<int>();
                    objectsToRemove.Clear();

                    //Objekte auf der Höhe "i1" raussuchen
                    for (int i2 = 0; i2 < groundObject.Count; i2++)
                    {
                        if (groundObject[i2].Path.IsVisible(new Point(groundObject[i2].Position().X, (int)i1)))
                        {
                            objectsToRemove.Add(i2);
                        }
                    }

                    //Objekte auf der Höhe "i1" entfernen
                    for (int iObject = 0; iObject < objectsToRemove.Count; iObject++)
                    {
                        groundObject.RemoveAt(objectsToRemove[iObject]);
                        for (int iObject2 = 0; iObject2 < objectsToRemove.Count; iObject2++)
                        {
                            if (objectsToRemove[iObject2] > objectsToRemove[iObject])
                            {
                                objectsToRemove[iObject2] -= 1;
                            }
                        }
                    }
                    
                    //Objekte unter der Höhe "i1" raussuchen und um 1 nach unten verschieben
                    for (int i2 = 0; i2 < groundObject.Count; i2++)
                    {
                        if (groundObject[i2].Position().Y <= i1 - blockS.Height)
                        {
                            groundObject[i2].Move(0, blockS.Height);
                        }
                    }

                    //Objekte mit den Kollisionobjekten anpassen
                    groundObjects.Reset();
                    foreach (MyGraphicObject go in groundObject)
                    {
                        groundObjects.AddRectangle(Rectangle.Round(go.GetRectangle()));
                    }

                    //Globale Variable ändern
                    Reihen += 1;
                }
            }
            this.Invalidate();
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
                if (groundObjects.IsVisible(
                    goCurrentObject.Position().X + (deltaX * blockS.Width),
                    goCurrentObject.Position().Y))
                {
                    borderCollision = true;
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
                if (groundObjects.IsVisible(
                    goCurrentObject.Position().X,
                    goCurrentObject.Position().Y + blockS.Width))
                {
                    groundCollision = true;
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
