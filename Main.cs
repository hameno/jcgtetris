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
        Timer tCount;
        private bool bShowMenu;
        private int iSpeedFactor;
        private int iReihen;
        private int iLevel;
        private int iReihenZumLevelup;
        private TetronType alterStein;
        private Difficulty Schwierigkeitsgrad;
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
        
        public Main()
        {
            InitializeComponent();

            //Eigenschaften der Oberfläche definieren
            this.ClientSize = new Size(400, 600);

            //Damit geht das Neuzeichnen viel flüssiger
            this.DoubleBuffered = true;
            
            //Startwerte für das Tetris-Game werden gesetzt
            InitGame();
            // Schwierigkeitsgrad auf Mittel setzen
            Schwierigkeitsgrad = Difficulty.Mittel;
            tCount = new Timer();
            tCount.Tick += new EventHandler(tCount_Tick);
            tCount.Interval = 500;
            iSpeedFactor = 5;
            iReihenZumLevelup = 5;
            alterStein = TetronType.I;
            iLevel = 1;
            iReihen = 0;
            bShowMenu = false;
            StartGame();
        }
        /// <summary>
        /// Startet das Spiel
        /// </summary>
        private void StartGame()
        {
            AddObject();
            tCount.Start();
        }
        private void CheckForLevelUp()
        {
            if ((iReihen % iReihenZumLevelup) == 0)
            {
                iLevel++;
                IncreaseSpeed();
            }
        }
        void tCount_Tick(object sender, EventArgs e)
        {
            MoveObject(0,1);
            
        }
        private TetronType GenerateRandomTetronType()
        {
            
            TetronType neuStein;
            // neues Random-Objekt
            Random rnd = new Random();
            // Wähle ein Element zwischen 1 und maximaler Anzahl
            int irnd = rnd.Next(1, Enum.GetValues(typeof(TetronType)).Length);
            if ((TetronType)irnd == alterStein)
            {
                MessageBox.Show("Musste neu generieren");
                neuStein = GenerateRandomTetronType();

            }
            else
            {
                neuStein = (TetronType)irnd;
            }
            // Verändere den Typ des block
            alterStein = neuStein;
            return neuStein;
        }
        private void IncreaseSpeed()
        {
            if ((tCount.Interval - (iSpeedFactor*(int)Schwierigkeitsgrad) * iReihen) > 0)
            {
                tCount.Interval -= (iSpeedFactor*(int)Schwierigkeitsgrad) * iReihen;
            }
        }

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            if (IsAtMenu)
            {
                DrawMenu(e);
            }
            else
            {
                Font f= new Font(FontFamily.GenericSansSerif, 20);
                
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(Color.White);
                e.Graphics.DrawString("Speed: " + tCount.Interval.ToString(), f, Brushes.Black,1, 1);
                e.Graphics.DrawString("Level: " + iLevel.ToString(), f, Brushes.Black, 1, 21);
                foreach (MyGraphicObject go in currentObject)
                {
                    go.Draw(e.Graphics);
                }
                foreach (MyGraphicObject go in groundObject)
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
                case Keys.Left: // Nach links
                case Keys.A:
                    if(IsRunning)
                    MoveObject(-1, 0);
                    break;
                case Keys.Right: // Nach rechts
                case Keys.D:
                    if (IsRunning)
                    MoveObject(1, 0);
                    break;
                case Keys.Up: // Rotieren
                case Keys.W:
                    if (IsRunning)
                    RotateObject();
                    break;
                case Keys.Down: // Nach unten
                case Keys.S:
                    if (IsRunning)
                    tCount_Tick(null, null);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private bool IsAtMenu
        {
            get { return bShowMenu; }
        }
    }
}