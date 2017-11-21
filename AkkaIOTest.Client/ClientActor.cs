using Akka.Actor;
using Akka.IO;
using AkkaIOTest.Shared;
using System;
using System.Net;

namespace AkkaIOTest.Client
{
    internal sealed class ClientActor : UntypedActor
    {
        private static readonly ByteString dataPiece = ByteString.FromBytes(new byte[CommunicationSettings.MaxDataSize]);

        private IActorRef io;

        protected override void PreStart()
        {
            Become(DisconnectedMode);
        }

        protected override void OnReceive(object message) { /* this method is not used */ }

        private void NetworkConnectionEstablishmentMode(object message)
        {
            switch (message)
            {
                case Tcp.Connected msg: NetworkConnectionEstablished(msg); break;
                case Tcp.CommandFailed msg: ConnectionFailed(msg); break;

                case DisconnectMessage msg: Disconnect(); break;

                default: Unhandled(message); break;
            }
        }

        private void ConnectedMode(object message)
        {
            switch (message)
            {
                case Tcp.ErrorClosed msg: ConnectionClosed(msg); break;
                case Tcp.Closed msg: ConnectionClosed(msg); break;

                case Tcp.Event msg: SendData(); break;

                case DisconnectMessage msg: Disconnect(); break;

                default: Unhandled(message); break;
            }
        }

        private void DisconnectedMode(object message)
        {
            switch (message)
            {
                case InitConnectionMessage msg: InitConnection(); break;
                default: Unhandled(message); break;
            }
        }

        private void NetworkConnectionEstablished(Tcp.Connected msg)
        {
            io = Sender;
            io.Tell(new Tcp.Register(Self));

            Console.WriteLine("connection established");
            SendData();
            Become(ConnectedMode);
        }

        private void SendData()
        {
            Console.WriteLine("sending data");
            io.Tell(Tcp.Write.Create(dataPiece, new Tcp.Event()));
        }

        private void Disconnect()
        {
            Context.Stop(io);
            io = null;
            Become(DisconnectedMode);
        }

        private void ConnectionClosed(Tcp.ConnectionClosed msg)
        {
            Console.WriteLine($"The controller has disconnected");
            Disconnect();
        }

        private void ConnectionFailed(Tcp.CommandFailed msg)
        {
            Console.WriteLine($"Cannot connect to the controller");

            Disconnect();
        }

        private void InitConnection()
        {
            var host = "localhost";
            var port = CommunicationSettings.Port;

            Context.System.Tcp().Tell(new Tcp.Connect(new DnsEndPoint(host, port)));

            Become(NetworkConnectionEstablishmentMode);
        }

        public sealed class DisconnectMessage
        {
            public static readonly DisconnectMessage Instance = new DisconnectMessage();

            private DisconnectMessage() { }
        }

        public sealed class InitConnectionMessage
        { }
    }
}
