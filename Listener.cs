using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Host
{
    // Class representing the listener that listens for TCP connections from client
    class Listener
    {
        private readonly int PORT_NO;
        private readonly string SERVER_IP;

        private readonly TcpListener listener;
        private readonly IPAddress ipaddress;

        private readonly Simulator sim;

        // Constructor
        // param sim; Simulator to pass the received message to
        public Listener(Simulator simulator)
        {
            PORT_NO = Int32.Parse(ConfigurationManager.AppSettings.Get("port_no"));
            SERVER_IP = ConfigurationManager.AppSettings.Get("server_ip");
            ipaddress = IPAddress.Parse(SERVER_IP);
            listener = new TcpListener(ipaddress, PORT_NO);
            sim = simulator;
        }

        // Main method to listen for TCP connections and passing them to the controller
        public void Listen() 
        {
            Console.WriteLine("Listening...");
            listener.Start();
            Boolean luister = true;

            while (luister)
            {
                TcpClient client = listener.AcceptTcpClient();

                NetworkStream nwStream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];

                int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received : " + dataReceived);

                if (dataReceived.Equals("Client_Ready"))
                {
                    string response = sim.NextAction();
                    byte[] bytesToReturn = Encoding.UTF8.GetBytes(response);
                    nwStream.Write(bytesToReturn, 0, bytesToReturn.Length);
                    if (response.Equals("Stop_"))
                    {
                        Console.WriteLine("Luisteren gestopt");
                        luister = false;
                    }
                }

                if (dataReceived.Equals("Client_Get_Commit"))
                {
                    byte[] data = sim.GetCurrentCommitAsBytes();
                    byte[] dataLength = BitConverter.GetBytes(data.Length);
                    byte[] package = new byte[4 + data.Length];
                    dataLength.CopyTo(package, 0);
                    data.CopyTo(package, 4);

                    int bytesSent = 0;
                    int bytesLeft = package.Length;

                    while (bytesLeft > 0)
                    {
                        int nextPacketSize = (bytesLeft > buffer.Length) ? buffer.Length : bytesLeft;

                        nwStream.Write(package, bytesSent, nextPacketSize);
                        bytesSent += nextPacketSize;
                        bytesLeft -= nextPacketSize;

                    }
                }

                if (dataReceived.StartsWith("Client_GetFile_"))
                {
                    string pad = dataReceived.Remove(0, 15);
                    byte[] data = sim.GetFileAsBytes(pad);
                    byte[] dataLength = BitConverter.GetBytes(data.Length);
                    byte[] package = new byte[4 + data.Length];
                   
                    dataLength.CopyTo(package, 0);
                    data.CopyTo(package, 4);

                    int bytesSent = 0;
                    int bytesLeft = package.Length;

                    while (bytesLeft > 0)
                    {
                        int nextPacketSize = (bytesLeft > buffer.Length) ? buffer.Length : bytesLeft;

                        nwStream.Write(package, bytesSent, nextPacketSize);
                        bytesSent += nextPacketSize;
                        bytesLeft -= nextPacketSize;

                    }

                }

            }

        }
    }
}

