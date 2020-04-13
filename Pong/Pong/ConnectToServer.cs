using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pong
{
    public partial class ConnectToServer : Form
    {
        private PongGame pongForm;

        public ConnectToServer(PongGame form)
        {
            InitializeComponent();
            this.pongForm = form;
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Client server = new Client(this.ipBox.Text, Program.PORT);
                this.Hide();
                this.pongForm.server = server;
                pongForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error");
            }
        }

        private void ConnectToServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
