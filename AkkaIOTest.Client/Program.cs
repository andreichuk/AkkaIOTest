using Akka.Actor;
using Akka.Configuration;
using AkkaIOTest.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaIOTest.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var config =
                ConfigurationFactory.ParseString(@"
                    akka {
                        stdout-loglevel = OFF
                        loglevel = DEBUG
                        log-config-on-start = on
                        suppress-json-serializer-warning = on

                        actor {
                            debug {
                              receive = off
                              autoreceive = off
                              lifecycle = off
                              event-stream = off
                              unhandled = on
                            }
                        }
                    }");

            using (var actorSystem = ActorSystem.Create("ClientSide", config))
            {
                var client = actorSystem.ActorOf(Props.Create<ClientActor>(), "Client");
                client.Tell(new ClientActor.InitConnectionMessage());

                Console.ReadLine();
            }
        }
    }
}
