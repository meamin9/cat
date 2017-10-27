using Cellnet;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace UnitTest
{

    class Program
    {
        static void Client( )
        {
            Console.WriteLine("client");

          
            var ed = new EventDispatcher();

            var codec = new ProtobufCodec();

            var peer = new Connector(ed.Queue, codec).Start("127.0.0.1:7200");
            //var peer = new Connector(ed.Queue, codec).Start("192.168.0.138:8000");
            Subscribe.RegisterMessage<gamedef.SessionConnected>(ed, (msg, ses) =>
            {
                Console.WriteLine("SessionConnected");

                var ack = new proto.Echo();
                ack.content = "client";
                bool r = ses.Send(ack);
                Console.WriteLine("send------");
                Console.WriteLine(r);
            });


            Subscribe.RegisterMessage<proto.Echo>(ed, (msg, ses) =>
            {                
                var ack = new proto.Echo();
                ack.content = "hello";
                ses.Send(ack);

            });

            ed.Start();

            ed.Wait();
        }

        static void Server( )
        {
            Console.WriteLine("server");

            var ed = new EventDispatcher();

            var codec = new ProtobufCodec();

            var peer = new Acceptor(ed.Queue, codec).Start("127.0.0.1:7010");
            Subscribe.RegisterMessage<gamedef.SessionAccepted>(ed, (msg, ses) =>
            {
                Console.WriteLine("SessionAccepted");
            });

            var meter = new QPSMeter(ed, (qps) =>
            {
                Console.WriteLine("qps: {0}", qps);
            });

            Subscribe.RegisterMessage<proto.Echo>(ed, (msg, ses) =>
            {               
                meter.Acc();

                ses.Send(msg);

            });

            ed.Start();

            ed.Wait();
        }

        static void Main(string[] args)
        {
            MessageMetaSet.StaticInit(Assembly.GetExecutingAssembly(), "proto");


            if ( true || args.Length > 0 && args[0] == "c")
            {
                Client();
            }
            else
            {
                Server();
            }
            

            
        }
    }
}
