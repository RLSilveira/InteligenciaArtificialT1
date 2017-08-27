using System;
using System.Collections.Generic;
using System.Text;

namespace InteligenciaArtificialT1
{
    public class Lixeira
    {

        public int Capacidade { get; private set; }


        public Lixeira(int capacidade)
        {
            Capacidade = capacidade;
        }

        public Lixeira()
        {
        }

        public override string ToString()
        {
            return "L";
        }
    }
}
