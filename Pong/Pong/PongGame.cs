using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pong
{
    public partial class PongGame : Form
    {
        public Client server;
        private string name;
        private Panel playground;

        public PongGame(string name)
        {
            InitializeComponent();
            this.name = name;
        }

        private void InitializeMyComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.Bounds = Screen.PrimaryScreen.Bounds;

            this.playground = new Panel();
            this.playground.Dock = DockStyle.Fill;
            this.Controls.Add(playground);
        }

        private void PongGame_Shown(object sender, EventArgs e)
        {
            InitializeMyComponent();
            int[] resolution = new int[2] { this.playground.Width, this.playground.Height };
            ExitFullScreen();
            this.server.FirstSend(this.name, resolution);
            Tuple<string, bool, ServerOption> results = this.server.ReciveServerFirstAnswer();
            if (results.Item2)
            {
                MessageBox.Show($"You have joined {results.Item1}'s server successfully \nThe gamemode is {results.Item3.ToString()}");
            }
            else
            {
                MessageBox.Show($"You cannot join {results.Item1}'s server because" +
                    $" your screen resolution is {resolution[0]},{resolution[1]} and not 1920,1080");
                Application.Exit();
            }
        }

        private void PongGame_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void PongGame_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
            if (e.KeyCode == Keys.F)
            {
                ExitFullScreen();
            }

        }
        private void ExitFullScreen()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.TopMost = false;
            this.Bounds = new Rectangle(new Point(this.playground.Width / 2 , this.playground.Height / 2), new Size(300 , 300));
        }
    }
}
