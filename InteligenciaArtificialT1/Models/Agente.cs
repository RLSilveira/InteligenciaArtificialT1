using System;
using System.Collections.Generic;
using System.Text;

namespace InteligenciaArtificialT1
{
    public class Agente
    {

        public int CapacidadeBateria { get; set; }
        public int Bateria { get; set; }
        public int CapacidadeRepositorio { get; private set; }
        public int UltimaCelula { get; set; }


        public int Repositorio { get; set; }

        public Agente()
        {
            
        }

        public void run()
        {

        }

        public override string ToString()
        {
            return "A";
        }
    }
}
