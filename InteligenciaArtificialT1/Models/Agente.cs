using System;
using System.Collections.Generic;
using System.Text;
using InteligenciaArtificialT1.Models;

namespace InteligenciaArtificialT1
{
    public class Agente
    {

        public int CapacidadeBateria { get; set; }
        public int Bateria { get; set; }
        public int CapacidadeRepositorio { get; private set; }
        public int UltimaCelula { get; set; }


        public int Repositorio { get; set; }
        public Ambiente Ambiente { get; set; }

        public Agente()
        {

        }

        public void run()
        {

            var adjascentes = Ambiente.getAdjascentes();




            // varrer o mapa

            // ir para lixeira

            // is para ponto recarga

        }


        private int Heuristica()
        {
            return 0;
        }

        public override string ToString()
        {
            return "A";
        }
    }
}
