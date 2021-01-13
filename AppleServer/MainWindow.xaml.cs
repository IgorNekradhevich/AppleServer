using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;

namespace AppleServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //NetworkStream networkStream;
        TcpListener listener;
        bool isStarted;
        List<TcpClient> clients;
        string[] apples;
        public MainWindow()
        {
            InitializeComponent();

        }

        void sendApples()
        {
            string applesCount = "";//0|0|1|0|0|
            for (int i = 0; i < 9; i++)
            {
                applesCount += apples[i].ToString() + "|";
            }
           // applesCount = "0|1|2|3|4|5|6|7|8|9|10";
            byte[] buffer = Encoding.UTF8.GetBytes(applesCount);

            foreach (TcpClient client in clients)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
        }
        void ServerListner ()
        {
            while (isStarted)
            {
              clients.Add (listener.AcceptTcpClient());
              sendApples();
              Task.Factory.StartNew(() => { ClientListner(clients[clients.Count - 1]);});
            }
        }

        void ClientListner(TcpClient client)
        {
            bool isConnected = true;
            NetworkStream networkStream = client.GetStream();
           
            while(isStarted && isConnected)
            {
                try
                {
                    byte[] buffer = new byte[250];//e3
                    networkStream.Read(buffer, 0, 250);
                    Console.WriteLine(Encoding.UTF8.GetString(buffer));
                    
                    sendApples();
                }
                catch
                {
                    Console.WriteLine("DISCONNECTED");
                    clients.Remove(client);
                    isConnected = false;
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int port;
            try
            {
                port = Convert.ToInt32(TBPort.Text);
            }
            catch
            {
                MessageBox.Show("Неверно указан порт");
                return;
            }
            if (!isStarted)
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                clients = new List<TcpClient>();

                isStarted = true;

                apples = new string[9];
                for (int i = 0; i < 9; i++)
                    apples[i]="0";

                Task.Factory.StartNew(() => { ServerListner(); });
            }
            else
            {
                MessageBox.Show("Сервер уже запущен");
            }
        }
    }
}
