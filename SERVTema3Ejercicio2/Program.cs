using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SERVTema3Ejercicio2
{
    class Program
    {
        static readonly private object l = new object();
        static private List<Usuario> usuarios = new List<Usuario>();
        static void Main(string[] args)
        {
            IPEndPoint ie = new IPEndPoint(IPAddress.Any, 31416);
            Socket s = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
            s.Bind(ie);
            s.Listen(10);
            Console.WriteLine("Server waiting at port {0}", ie.Port);
            while (true)
            {
                Socket cliente = s.Accept();
                Usuario usuario = new Usuario();
                usuario.Cliente = cliente;
                usuarios.Add(usuario);
                Thread hilo = new Thread(hiloCliente);
                hilo.Start(usuario);
            }
        }

        static void hiloCliente(object participante)
        {
            bool running = true;
            string mensaje;
            string user = "";
            bool primeraConexion = true;
            Usuario usuario = (Usuario)participante;
            IPEndPoint ieCliente = (IPEndPoint)usuario.Cliente.RemoteEndPoint;
            Console.WriteLine("Connected with client {0} at port {1}",
            ieCliente.Address, ieCliente.Port);

            using (NetworkStream ns = new NetworkStream(usuario.Cliente))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {
                mensaje = "Bienvenido, introduce tu usuario:";
                sw.WriteLine(mensaje);
                sw.Flush();
                Console.WriteLine(running);
                while (running)
                {
                    Console.WriteLine("akika bucle");
                    try
                    {
                        if (primeraConexion)
                        {
                            user = sr.ReadLine();
                            usuario.UserName = user;
                        }
                        else
                        {
                            mensaje = sr.ReadLine();
                            switch (mensaje)
                            {
                                case "#salir":
                                    sw.WriteLine("akika salir");
                                    running = false;
                                    usuarios.Remove(usuario);
                                    break;
                                case "#lista":
                                    int indice = 1;
                                    lock (l) 
                                    {
                                        foreach (Usuario u in usuarios)
                                        {
                                            IPEndPoint ieCliente2 = (IPEndPoint)u.Cliente.RemoteEndPoint;
                                            Console.WriteLine(string.Format("{0}.- {1}@{2}", indice, u.UserName, ieCliente2.Address));
                                            sw.WriteLine(string.Format("{0}.- {1}@{2}", indice, u.UserName, ieCliente2.Address));
                                            indice++;
                                        }

                                    }
                                    break;
                            }
                            sw.Flush();
                        }

                        if (mensaje != null && running)
                        {
                            foreach (Usuario u in usuarios)
                            {
                                using (NetworkStream nss = new NetworkStream(u.Cliente))
                                using (StreamReader srr = new StreamReader(nss))
                                using (StreamWriter sww = new StreamWriter(nss))
                                {
                                    if (u.Cliente != usuario.Cliente)
                                    {
                                        if (primeraConexion)
                                        {
                                            Console.WriteLine(string.Format("{0}@{1}: {2}",
                                             user, ieCliente.Address, "Ha entrado al chat"));
                                            sww.WriteLine(string.Format("{0}@{1}: {2}",
                                             user, ieCliente.Address, "Ha entrado al chat"));
                                        }
                                        else
                                        {
                                            sww.WriteLine(string.Format("{0}@{1}: {2}",
                                            user, ieCliente.Address, mensaje));
                                        }
                                        sww.Flush();
                                    }
                                }
                            }
                        }
                        else 
                        {
                            running = false;
                        }
                        primeraConexion = false;
                    }
                    catch (IOException)
                    {
                        break;
                    }
                }
                Console.WriteLine("Finished connection with {0}:{1}",
                ieCliente.Address, ieCliente.Port);
            }
            usuario.Cliente.Close();
        }
    }
}
