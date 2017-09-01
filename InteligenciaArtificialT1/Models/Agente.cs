using System;
using System.Collections.Generic;
using System.Linq;
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
            var bDireita = true;
            var tam = Ambiente.getTamanho();
            var visitados = new List<Ambiente.Ponto>(Ambiente.getTamanho());

            var lastX = 0;
            int count = 0;
            while (count++ < tam * 3)
            {

                Ambiente.Ponto p = null;

                var prox = Ambiente.getAdjscentesOrdenados(bDireita);
                for (int i = 0; i < prox.Length; i++)
                {
                    if (prox[i] != null && !visitados.Contains(prox[i]) && (prox[i].Item == null || prox[i].Item is Sujeira))
                    {
                        p = prox[i];
                        break;
                    }
                }

                if (p == null)
                {
                    // sem saida
                }

                if (p == null) throw new Exception("Não sei para onde ir.");

                //if (p.Item is Parede) continue;
                //if (p.Item is Recarga) continue;
                //if (p.Item is Lixeira) continue;


                if (visitados.Contains(p)) continue;

                visitados.Add(p);
                Ambiente.Move(p);

                if (lastX >= p.Y)
                    bDireita = false;
                else if (lastX <= p.Y)
                    bDireita = true;

                lastX = p.Y;
            }

        }
        // Verifica capacidade

        // Verifica bateria

        // Move para proxima casa





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
