using Akka.Actor;
using Akka.Configuration;
using AkkaIOTest.Shared;
using System;

namespace AkkaIOTest.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var akkaConfig = ConfigurationFactory.ParseString(@"
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
                }");

            using (var actorSystem = ActorSystem.Create("ServerSide", akkaConfig))
            {
                actorSystem.ActorOf(Props.Create<NetHostActor>(CommunicationSettings.Port), "Host");

                Console.ReadLine();
            }
        }
    }
}
