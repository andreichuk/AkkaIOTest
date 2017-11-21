using Akka.Actor;
using Akka.IO;
using System.Net;

namespace AkkaIOTest.Server
{
    public sealed class NetHostActor : UntypedActor
    {
        private readonly int port;

        private int connectionCounter;

        public NetHostActor(int port)
        {
            this.port = port;
            connectionCounter = 0;
        }

        protected override void PreStart()
        {
            Context.System.Tcp().Tell(new Tcp.Bind(Self, new IPEndPoint(IPAddress.Any, port)));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Tcp.Bound msg: Bound(msg); break;
                case Tcp.Connected msg: NewConnection(msg); break;
                default: Unhandled(message); break;
            }
        }

        private void NewConnection(Tcp.Connected msg)
        {
            connectionCounter += 1;
            var communicator = Context.ActorOf(Props.Create(() => new ConnectionActor(Sender)), "connection-" + connectionCounter);
        }

        private void Bound(Tcp.Bound msg)
        {
            // log if necessary
        }
    }
}
