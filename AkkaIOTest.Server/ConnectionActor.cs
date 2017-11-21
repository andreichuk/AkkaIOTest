using Akka.Actor;
using Akka.IO;
using System;
using System.Threading;

namespace AkkaIOTest.Server
{
    internal sealed class ConnectionActor : UntypedActor
    {
        private readonly IActorRef connection;
        
        public ConnectionActor(IActorRef connection)
        {
            this.connection = connection;
        }

        protected override void PreStart()
        {
            Console.WriteLine("connected");
            connection.Tell(new Tcp.Register(Self));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Tcp.Received msg: /*Thread.Sleep(5);*/ break;
                case Tcp.ConnectionClosed msg: ConnectionClosed(); break;

                default: Unhandled(message); break;
            }
        }

        private void ConnectionClosed()
        {
            Context.Stop(Self);
        }

        private void ConnectionClosed(Terminated message)
        {
            Context.Stop(Self);

            Console.WriteLine($"connection closed: {Sender}");
        }
    }
}
