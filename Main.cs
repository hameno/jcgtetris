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
        private bool ShowMenu;

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

            tCount = new Timer();
            tCount.Tick += new EventHandler(tCount_Tick);
            tCount.Interval = 500;

            ShowMenu = false;
            StartGame();
        }
        
        private void StartGame()
        {
            AddObject();
            tCount.Start();
        }

        void tCount_Tick(object sender, EventArgs e)
        {
            MoveObject(0,1);
        }

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            if (IsAtMenu)
            {
                DrawMenu(e);
            }
            else
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
        }

        private bool IsRunning
        {
            get { return tCount.Enabled; }
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Left:
                case Keys.A:
                    if(IsRunning)
                    MoveObject(-1, 0);
                    break;
                case Keys.Right:
                case Keys.D:
                    if (IsRunning)
                    MoveObject(1, 0);
                    break;
                case Keys.Up:
                case Keys.W:
                    if (IsRunning)
                    RotateObject();
                    break;
                case Keys.Down:
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
            get { return ShowMenu; }
        }
    }
}