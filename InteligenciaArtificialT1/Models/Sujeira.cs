using System;
using System.Collections.Generic;
using System.Text;

namespace InteligenciaArtificialT1
{
    public class Sujeira
    {
        public int peso { get; private set; } = 1;

        public Sujeira(int valor)
        {
            peso = valor;
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
