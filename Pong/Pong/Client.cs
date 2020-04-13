using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Pong
{
    public class Client
    {
        private TcpClient client;
        private NetworkStream stream;
        public string name;
        public int lineIndex;
        public Client(string ip, int port)
        {
            this.client = new TcpClient(ip, port);
            this.stream = this.client.GetStream();
        }
        public Client(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();
        }
        public bool IsSpectator()
        {
            return GetTurn();
        }
        public void SendInt(int num)
        {
            this.stream.Write(new byte[1] { (byte)num }, 0, 1);
        }

        public void SendServerFirstAnswer(bool isAccepted, string name, ServerOption serverOption)
        {
            byte[] nameArr = Utilities.StringToByteArray(name);
            byte[] toSend = new byte[12];
            for (int i = 0; i < nameArr.Length; i++)
            {
                toSend[i] = nameArr[i];
            }
            toSend[10] = Convert.ToByte(isAccepted? 1 : 0);
            toSend[11] = Convert.ToByte((int)serverOption);
            this.stream.Write(toSend, 0, toSend.Length);
        }

        public Tuple<string, bool, ServerOption> ReciveServerFirstAnswer()
        {
            byte[] buffer = new byte[12];
            this.stream.Read(buffer, 0, 12);
            string name = Utilities.ByteArrayToString(buffer.Take(10).Where(b => b != 0).ToArray());
            bool isAccepted = buffer[10] != 0;
            ServerOption serverOption = (ServerOption)Convert.ToInt32(buffer[11]);
            return new Tuple<string, bool, ServerOption>(name, isAccepted, serverOption);
        }

        public void FirstSend(string name, int[] resolution)
        {
            byte[] nameArr = Utilities.StringToByteArray(name);
            byte[] toSend = new byte[12];
            for (int i = 0; i < nameArr.Length; i++)
            {
                toSend[i] = nameArr[i];
            }
            toSend[10] = Convert.ToByte(Math.Round((double)resolution[0] / 10));
            toSend[11] = Convert.ToByte(Math.Round((double)resolution[1] / 10));
            this.stream.Write(toSend, 0, toSend.Length);
        }

        public Tuple<string, int[]> FirstRecive()
        {
            byte[] buffer = new byte[12];
            this.stream.Read(buffer, 0, 12);
            string name = Utilities.ByteArrayToString(buffer.Take(10).Where(b => b != 0).ToArray());
            int[] resolution = new int[2] { Convert.ToInt32(buffer[10] * 10), Convert.ToInt32(buffer[11] * 10) };
            return new Tuple<string, int[]>(name, resolution);
        }

        public bool GetTurn()
        {
            byte[] buffer = new byte[1];
            this.stream.Read(buffer, 0, 1);
            return buffer[0] != 0;
        }
        public bool IsConnected()
        {
            return !((this.client.Client.Poll(1, SelectMode.SelectRead) && (this.client.Client.Available == 0)) || !this.client.Client.Connected);
        }
        public void Close()
        {
            this.client.Close();
        }
    }
}
