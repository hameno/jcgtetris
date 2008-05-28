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
    public class Bewegung
    {
        public bool Right;
        public bool Left;
        public bool Down;
        public Bewegung()
        {
            Right = false;
            Left = false;
            Down = false;
        }

    }
    public partial class Main : Form
    {
        Timer tCount;
        Timer tMovements;
        private bool bShowMenu;
        private int iSpeedFactor;
        private int iReihen;
        private int iLevel;


        Bewegung bewegungen = new Bewegung();
        private int iReihenZumLevelup;
        private bool GameOver;
        private TetronType alterStein;
        private TetronType nextStein;
        private Difficulty Schwierigkeitsgrad;
        List<MyGraphicObject> fieldObjects = new List<MyGraphicObject>();
        List<MyGraphicObject> previewObjects = new List<MyGraphicObject>();
        Tetromino previewBlock = new Tetromino();
        List<MyText> textObjects = new List<MyText>();
        public int Level
        {
            get
            {
                return iLevel;
            }
            set
            {
                iLevel = value;
                textObjects[1].ChangeText("Level: " + iLevel.ToString());
                textObjects[1].ApplyChanges();
            }
        }
        /// <summary>
        /// Level bei sound so vielen Steinen
        /// </summary>
        private enum Difficulty : int
        {
            Einfach = 1,
            Mittel = 2,
            Schwer = 5
        }
        private void DrawMenu(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
            string Title = "JCG Tetris";
            e.Graphics.DrawString(Title, new Font(FontFamily.GenericSerif, 25f), Brushes.Blue, new PointF((this.ClientSize.Width / 2) - Title.Length * (25 / 3), 20));
            Rectangle but1 = new Rectangle(this.ClientSize.Width / 3, this.ClientSize.Height / 5, this.ClientSize.Width / 3, this.ClientSize.Height / 10);
            e.Graphics.FillRectangle(Brushes.LightBlue, but1);
            e.Graphics.DrawRectangle(Pens.Red, but1);
        }
        private void InitGame()
        {
            
            currentObject.Clear();
            groundObjects.Reset();
            groundObject.Clear();
            fieldObjects.Clear();
            textObjects.Clear();
            // Schwierigkeitsgrad auf Mittel setzen
            Schwierigkeitsgrad = Difficulty.Einfach;
            iSpeedFactor = 5;
            iReihenZumLevelup = 3;
            tCount.Interval = 400;
            tMovements.Interval = 200;
            alterStein = TetronType.I;
            iLevel = 1;
            iReihen = 0;
            //Eigenschaften der Objekte definieren
            // Größe des Feldes
            fieldS.Width = 400;
            fieldS.Height = 600;
            // Anfangspunkt von oben links des Feldes
            fieldP.X = 5;
            fieldP.Y = 50;
            // Blockgröße
            blockS.Width = fieldS.Width / 10;
            blockS.Height = blockS.Width;
            // Mittelpunkt eines blockes
            //startP.X = (fieldS.Width / 2) - (blockS.Width / 2);
            //startP.Y = blockS.Height;
            GameOver = false;
            // Spielüberschrift
            fieldObjects.Add(new MyText(this, Pens.Transparent, Brushes.SteelBlue, "JCG Tetris", FontFamily.GenericSerif, FontStyle.Bold, 30, new Point(5, 5)));
            // Spielfeld
            fieldObjects.Add(new MyRectangle(this, Pens.Black, Brushes.Transparent, new Rectangle(fieldP, fieldS)));
            // Nächster Block
            fieldObjects.Add(new MyRectangle(this, Pens.Black, Brushes.Transparent, new Rectangle(new Point(fieldP.X+fieldS.Width+5, fieldP.Y), new Size(150, 150))));
            // Spielinformationen
            Point SpielInfoPunkt = new Point(fieldP.X + fieldS.Width + 5, fieldP.Y + 155);
            fieldObjects.Add(new MyRectangle(this, Pens.Black, Brushes.Transparent, new Rectangle(SpielInfoPunkt, new Size(150, fieldS.Height - 155))));
            // Textobjekte
            textObjects.Add(new MyText(this, Pens.Black, Brushes.Black, "Speed: " + tCount.Interval.ToString(), FontFamily.GenericSansSerif, FontStyle.Regular, 15, new Point(SpielInfoPunkt.X + 1, SpielInfoPunkt.Y + 1)));
            textObjects.Add(new MyText(this, Pens.Black, Brushes.Black, "Level: " + iLevel.ToString(), FontFamily.GenericSansSerif, FontStyle.Regular, 15, new Point(SpielInfoPunkt.X + 1, SpielInfoPunkt.Y + 5 + 20)));
            textObjects.Add(new MyText(this, Pens.Black, Brushes.Black, "Reihen: " + iReihen.ToString(), FontFamily.GenericSansSerif, FontStyle.Regular, 15, new Point(SpielInfoPunkt.X + 1, SpielInfoPunkt.Y + 25 + 20)));
            textObjects.Add(new MyText(this, Pens.Black, Brushes.Black, "Nächster Block:", FontFamily.GenericSansSerif, FontStyle.Regular, 15, new Point(fieldObjects[2].Position().X + 1, fieldObjects[2].Position().Y + 1)));
        }
        public Main()
        {
            InitializeComponent();
            tCount = new Timer();
            tCount.Tick += new EventHandler(tCount_Tick);
            tMovements = new Timer();
            tMovements.Tick += new EventHandler(tMovements_Tick);
            //Eigenschaften der Oberfläche definieren
            this.ClientSize = new Size(5 + 400 + 5 + 150 + 5, 50 + 600 + 5);

            //Damit geht das Neuzeichnen viel flüssiger
            this.DoubleBuffered = true;
            
            //Startwerte für das Tetris-Game werden gesetzt
            InitGame();

            bShowMenu = false;
            StartGame();
        }

        void tMovements_Tick(object sender, EventArgs e)
        {
            if (bewegungen.Down && bewegungen.Right && !bewegungen.Left)
            {
                MoveObject(1, 0);
                if(tCount.Enabled)
                tCount_Tick(null, null);
            }
            else if (bewegungen.Down && !bewegungen.Right && bewegungen.Left)
            {
                MoveObject(-1, 0);
                if (tCount.Enabled)
                tCount_Tick(null, null);
            }
            else if (!bewegungen.Down && !bewegungen.Right && bewegungen.Left)
            {
                MoveObject(-1, 0);
            }
            else if (!bewegungen.Down && bewegungen.Right && !bewegungen.Left)
            {
                MoveObject(1, 0);
            }
            else if (bewegungen.Down && !bewegungen.Right && !bewegungen.Left)
            {
                if (tCount.Enabled)
                tCount_Tick(null, null);
            }
        }
        private void GenerateNextBlockType()
        {
            previewObjects.Clear();
            nextStein = GenerateRandomTetronType();
            previewBlock.ChangeType(nextStein);
            for (int i1 = 0; i1 < 4; i1++)
            {
                previewObjects.Add(new MyRectangle(this, previewBlock.Pen, previewBlock.Brush, new Rectangle(
                    (int)fieldObjects[2].GetRectangle().X + (int)fieldObjects[2].GetRectangle().Width / 2  + previewBlock.Points[previewBlock.ObjectRotation, i1, 0] * 30,
                    (int)fieldObjects[2].GetRectangle().Y + (int)fieldObjects[2].GetRectangle().Height / 2 + previewBlock.Points[previewBlock.ObjectRotation, i1, 1] * 30,
                    30, 30)));
                //Zeichenfläche aktualisieren
                previewObjects[i1].ApplyChanges();
            }
            if (previewBlock.TetronType == TetronType.J)
            {
                foreach (MyGraphicObject go in previewObjects)
                {
                    go.Move(-1 * 30/2, -1 * 30);
                    go.ApplyChanges();
                }
            }
            else if (previewBlock.TetronType == TetronType.L)
            {
                foreach (MyGraphicObject go in previewObjects)
                {
                    go.Move(-1 * 30/2, -1 * 30);
                    go.ApplyChanges();
                }
            }
            else if (previewBlock.TetronType == TetronType.Z)
            {
                foreach (MyGraphicObject go in previewObjects)
                {
                    go.Move(-1*30/2, -1 * 30);
                    go.ApplyChanges();
                }
            }
            else if (previewBlock.TetronType == TetronType.S)
            {
                foreach (MyGraphicObject go in previewObjects)
                {
                    go.Move(-1 * 30 / 2, -1 * 30);
                    go.ApplyChanges();
                }
            }
            else if (previewBlock.TetronType == TetronType.T)
            {
                foreach (MyGraphicObject go in previewObjects)
                {
                    go.Move(-1 * 30 / 2, -1 * 30);
                    go.ApplyChanges();
                }
            }
            else if (previewBlock.TetronType == TetronType.I)
            {
                foreach (MyGraphicObject go in previewObjects)
                {
                    go.Move(0, -1 * 30 / 2);
                    go.ApplyChanges();
                }
            }
        }
        private void ResetGame()
        {
            this.Invalidate();
            InitGame();
            StartGame();
        }
        /// <summary>
        /// Startet das Spiel
        /// </summary>
        private void StartGame()
        {
            GenerateNextBlockType();
            AddObject();
            tCount.Start();
            tMovements.Start();
        }
        private void CheckForLevelUp()
        {
            if ((iReihen % iReihenZumLevelup) == 0 && iReihen > 0)
            {
                Level += 1;
                IncreaseSpeed();
            }
        }
        void tCount_Tick(object sender, EventArgs e)
        {
            MoveObject(0,1);
            
        }
        /// <summary>
        /// Gibt einen zufälligen Steintyp zurück, Wiederholungen sind ausgeschlossen
        /// </summary>
        /// <returns>Tetrontype</returns>
        private TetronType GenerateRandomTetronType()
        {
            // neuer steintyp
            TetronType neuStein;
            // neues Random-Objekt
            Random rnd = new Random();
            do
            {
                // Wähle ein Element zwischen 1 und maximaler Anzahl
                neuStein = (TetronType)rnd.Next(1, Enum.GetValues(typeof(TetronType)).Length+1);
            }
            while(neuStein == alterStein); // solange gleich
            // Schreibe neuen in alten
            alterStein = neuStein;
            // gebe neuen steintyp zurück
            return neuStein;
        }
        private void IncreaseSpeed()
        {
            if ((tCount.Interval - (iSpeedFactor * (int)Schwierigkeitsgrad) * 10) > 0)
            {
                tCount.Interval -= (iSpeedFactor * (int)Schwierigkeitsgrad) * 10;
                
            }
            else if(tCount.Interval - 10 > 0)
            {
                tCount.Interval -= 10;
            }
            else if (tCount.Interval - 5 > 0)
            {
                tCount.Interval -= 5;
            }
            else if(tCount.Interval - 1 > 0)
            {
                tCount.Interval -= 1;
            }
            textObjects[0].ChangeText("Speed: " + tCount.Interval.ToString());
            textObjects[0].ApplyChanges();
        }
        public int Reihen
        {
            get
            {
                return iReihen;
            }
            set
            {
                iReihen = value;
                CheckForLevelUp();
                textObjects[2].ChangeText("Reihen: " + iReihen.ToString());
                textObjects[2].ApplyChanges();
            }
        }
        private void Main_Paint(object sender, PaintEventArgs e)
        {
            // Antialising
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            // Spielfeld leeren
            e.Graphics.Clear(Color.White);
            if (IsAtMenu)
            {
                DrawMenu(e);
            }
            else if (GameOver)
            {
                
                //e.Graphics.Clear(Color.Red);
                fieldObjects[1].Brush = Brushes.Red;
                fieldObjects[1].ApplyChanges();
                fieldObjects[1].Draw(e.Graphics);
                tCount.Stop();
                tMovements.Stop();
                MyText text = new MyText(this, Pens.Black, Brushes.Black, "GAME OVER", FontFamily.GenericSerif, FontStyle.Bold, 40, new Point(fieldS.Width/6, fieldS.Height/6));
                text.Draw(e.Graphics);
                foreach (MyGraphicObject to in textObjects)
                {
                    to.Draw(e.Graphics);
                }
            }
            else
            {
                // Spielinformationen
                
                // Objekte zeichnen
                foreach (MyGraphicObject go in currentObject)
                {
                    go.Draw(e.Graphics);
                }
                //Zeichnen der Bodenobjekte
                foreach (MyGraphicObject go in groundObject)
                {
                    go.Draw(e.Graphics);
                }
                foreach (MyGraphicObject go in fieldObjects)
                {
                    go.Draw(e.Graphics);
                }
                foreach (MyGraphicObject to in textObjects)
                {
                    to.Draw(e.Graphics);
                }
                foreach (MyGraphicObject go in previewObjects)
                {
                    go.Draw(e.Graphics);
                }
            }

        }
        /// <summary>
        /// Gibt zurück, ob das Spiel aktiv ist
        /// </summary>
        private bool IsRunning
        {
            get { return tCount.Enabled; }
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.N: // Neues SPiel
                    ResetGame();
                    break;
                case Keys.Left: // Nach links
                case Keys.A:
                    bewegungen.Left = true;
                    tMovements_Tick(null, null);
                    break;
                case Keys.Right: // Nach rechts
                case Keys.D:
                    bewegungen.Right = true;
                    tMovements_Tick(null, null);
                    break;
                case Keys.Up: // Rotieren
                case Keys.W:
                    if (IsRunning)
                    RotateObject();
                    break;
                case Keys.Down: // Nach unten
                case Keys.S:
                    if(tCount.Enabled)
                    tCount_Tick(null, null);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.Z:
                    Reihen++;
                    CheckForLevelUp();
                    break;
            }
        }


        private bool IsAtMenu
        {
            get { return bShowMenu; }
        }

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Left: // Nach links
                case Keys.A:
                    bewegungen.Left = false;
                    break;
                case Keys.Right: // Nach rechts
                case Keys.D:
                    bewegungen.Right = false;
                    break;
                case Keys.Down: // Nach unten
                case Keys.S:
                    bewegungen.Down = false;
                    break;
            }
        }
    }
}