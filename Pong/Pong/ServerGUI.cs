using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pong
{
    public partial class ServerGUI : Form
    {
        private Server server = new Server();
        private List<Client> spectators = new List<Client>();
        private string name;
        public ServerOption option = ServerOption.freeplay;

        public ServerGUI(string name)
        {
            InitializeComponent();
            try
            {
                ipBox.Text = new WebClient().DownloadString("http://icanhazip.com");
            }
            catch (Exception)
            {
                MessageBox.Show("No internet connection");
                Application.Exit();
            }
            this.Text = $"{name}'s Server";
            this.name = name;
            ServerOptions options = new ServerOptions(this);
            options.ShowDialog();
        }

        private Client HandlePlayer()
        {
            Client player = this.server.WaitForPlayer();
            Tuple<string, int[]> results = player.FirstRecive();
            string name = results.Item1;
            int[] resolution = results.Item2;
            if (!(resolution[0] == 1920 && resolution[1] == 1080))
            {
                AddText(ConsoleTextBox, $"{name} connected but his resolution isn't good");
                player.SendServerFirstAnswer(false, this.name, this.option);
                return HandlePlayer();
            }
            player.name = name;
            player.SendServerFirstAnswer(true, this.name, this.option);
            return player;
        }
        private Client[] WaitForPlayers(int numberOfPlayers)
        {
            this.server.StartLitsening();
            AddText(ConsoleTextBox, $"Waiting for {numberOfPlayers} players...");

            Client[] players = new Client[numberOfPlayers];

            for (int i = 0; i < numberOfPlayers; i++)
            {
                players[i] = HandlePlayer();                    
                AddText(ConsoleTextBox, "Player " + players[i].name + " connected");
                AddText(Players, players[i].name + " - Player");
            }
            return players;
        }

        private void CheckIfSpectatorsConnected()
        {
            var tmpSpectators = new List<Client>(this.spectators);
            foreach (Client spectator in tmpSpectators)
            {
                if (!spectator.IsConnected())
                {
                    this.spectators.RemoveAll(x => x.Equals(spectator));
                    List<string> lines = this.Players.Lines.ToList();
                    lines.RemoveAt(spectator.lineIndex);
                    Players.Invoke((MethodInvoker)delegate ()
                    {
                        this.Players.Lines = lines.ToArray();
                    });
                    int counter = 2;
                    foreach (Client spect in this.spectators)
                    {
                        spect.lineIndex = counter;
                        counter++;
                    }
                }
            }
        }



        private void RemoveNotConnected()
        {
            Thread check = new Thread(() =>
            {
                while (true)
                {
                    //try
                    //{
                    //    CheckIfSpectatorsConnected();
                    //}
                    //catch (Exception e)
                    //{
                    //    MessageBox.Show(e.Message);
                    //    continue;
                    //}

                    CheckIfSpectatorsConnected();
                }
            });
            check.Start();
        }

        private void WaitForSpectators()
        {
            Thread wait = new Thread(() =>
            {
                while (true)
                {
                    Client spectator = HandlePlayer();
                    spectator.lineIndex = this.Players.Lines.Length - 1;
                    AddText(Players, spectator.name + " - Spectator");
                    spectator.SendInt(1);
                    this.spectators.Add(spectator);
                }
            });
            wait.Start();
        }

        private void ServerGUI_Load(object sender, EventArgs e)
        {

        }

        private void RunGame()
        {
            Thread FreePlay = new Thread(() =>
            {
                Client[] players = WaitForPlayers(2);
                Client firstPlayer = players[0];
                Client secondPlayer = players[1];
                WaitForSpectators();
                RemoveNotConnected();
            });

            Thread TwoPlayers = new Thread(() =>
            {
                Client[] players = WaitForPlayers(2);
                WaitForSpectators();
                RemoveNotConnected();
            });

            Thread FourPlayers = new Thread(() =>
            {
                Client[] players = WaitForPlayers(4);
                WaitForSpectators();
                RemoveNotConnected();
            });

            Thread EightPlayers = new Thread(() =>
            {
                Client[] players = WaitForPlayers(8);
                WaitForSpectators();
                RemoveNotConnected();
            });



            if (this.option == ServerOption.freeplay)
            {
                FreePlay.Start();
            }
            else if (this.option == ServerOption.twoP)
            {
                TwoPlayers.Start();
            }
            else if (this.option == ServerOption.fourP)
            {
                FourPlayers.Start();
            }
            else if (this.option == ServerOption.eightP)
            {
                EightPlayers.Start();
            }
        }

        private void ServerGUI_Shown(object sender, EventArgs e)
        {
            RunGame();
        }

        private void ServerGUI_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void AddText(TextBox box, string text)
        {
            box.Invoke((MethodInvoker)delegate ()
            {
                box.Text += text + Environment.NewLine;
            });
        }

        private void ClearTextBox(TextBox box)
        {
            box.Invoke((MethodInvoker)delegate ()
            {
                box.Text = "";
            });
        }
    }
}
