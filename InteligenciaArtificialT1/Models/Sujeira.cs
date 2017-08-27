using System;
using System.Collections.Generic;
using System.Text;

namespace InteligenciaArtificialT1
{
    public class Sujeira
    {
        public int count { get; private set; } = 1;

        public Sujeira(int valor)
        {
            count = valor;
        }

        public Sujeira()
        {
        }

        public override string ToString()
        {
            return "s";
        }
    }

}
