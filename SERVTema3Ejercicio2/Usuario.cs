using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SERVTema3Ejercicio2
{
    class Usuario
    {
        private Socket cliente;
        public Socket Cliente
        {
            set
            {
                cliente = value;
            }
            get
            {
                return cliente;
            }
        }

        private string userName;
        public string UserName
        {
            set
            {
                userName = value;
            }
            get
            {
                return userName;
            }
        }
        public Usuario()
        {
            
        }
    }
}
