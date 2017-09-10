using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using InteligenciaArtificialT1.Models;

namespace InteligenciaArtificialT1
{
    public class Agente
    {

        public int CapacidadeBateria = 50;
        public int CapacidadeRepositorio;

        public int Bateria { get; set; }
        public int Repositorio { get; set; }


        public int UltimaCelula { get; set; }
        public Ambiente Ambiente { get; set; }

        public Agente(int capacidadeRepositorio)
        {
            CapacidadeRepositorio = capacidadeRepositorio;
            Bateria = CapacidadeBateria;
        }

        public void Run()
        {
            var bDireita = true;
            var tam = Ambiente.getTamanho();

            Ambiente.Ponto ultimaPosicao = Ambiente.getAgentePosicao();

            int col = ultimaPosicao.X;
            var bDesce = true;

            Ambiente.Ponto p = null;

            while (true)
            {

                if (Bateria < CapacidadeBateria * 0.1) // 10 %
                {
                    Recarregar();
                }

                if (Repositorio == CapacidadeRepositorio)
                {
                    Esvaziar();
                }

                if (bDesce)
                {
                    p = Ambiente.getBaixo();

                    if (p == null)
                    {
                        // fim do mapa
                        p = Ambiente.getDireita();
                        bDesce = false;

                        if (p == null)
                        {
                            break; // Fim
                        };
                    }

                    if (p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                    {
                        // Modo contorno
                        Contornar(bDesce);
                        continue;
                    }

                    ultimaPosicao = p;
                    Ambiente.Move(p);

                }
                else
                {
                    p = Ambiente.getCima();

                    if (p == null)
                    {
                        // fim do mapa
                        p = Ambiente.getDireita();
                        bDesce = true;

                        if (p == null)
                        {
                            break; // Fim
                        };
                    }

                    if (p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                    {
                        // Modo contorno
                        Contornar(bDesce);
                        continue;
                    }

                    ultimaPosicao = p;
                    Ambiente.Move(p);
                }

            }

        }

        void Contornar(bool bDesce)
        {
            bool esquerda = true;
            if (bDesce)
            {
                var p = Ambiente.getBaixoEsquerda();
                esquerda = true;

                if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                {
                    esquerda = false;
                    p = Ambiente.getBaixoDireita();
                }


                if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                {
                    esquerda = true;
                    p = Ambiente.getEsquerda();
                }

                if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                {
                    esquerda = false;
                    p = Ambiente.getDireita();
                }

                Ambiente.Move(p);


                if (!esquerda)
                {
                    DescendoVoltaEsquerda:
                    p = Ambiente.getBaixoEsquerda();
                    if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                    {
                        p = Ambiente.getBaixo();
                        Ambiente.Move(p);
                        goto DescendoVoltaEsquerda;
                    }
                }
                else
                {
                    DescendoVoltaDireita:
                    p = Ambiente.getBaixoDireita();
                    if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                    {
                        p = Ambiente.getBaixo();
                        Ambiente.Move(p);
                        goto DescendoVoltaDireita;
                    }

                }

                Ambiente.Move(p);

            }
            else // Sobe
            {
                var p = Ambiente.getCimaEsquerda();
                esquerda = true;

                if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                {
                    esquerda = false;
                    p = Ambiente.getCimaDireita();
                }

                if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                {
                    p = Ambiente.getEsquerda();
                    esquerda = true;
                }

                if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                {
                    esquerda = false;
                    p = Ambiente.getDireita();
                }


                Ambiente.Move(p);


                if (!esquerda)
                {
                    SubindoVoltaEsquerda:
                    p = Ambiente.getCimaEsquerda();
                    if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                    {
                        p = Ambiente.getCima();
                        Ambiente.Move(p);
                        goto SubindoVoltaEsquerda;
                    }
                }
                else
                {
                    SubindoVoltaDireita:
                    p = Ambiente.getCimaDireita();
                    if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                    {
                        p = Ambiente.getCima();
                        Ambiente.Move(p);
                        goto SubindoVoltaDireita;
                    }

                }

                Ambiente.Move(p);
            }
        }

        private int Heuristica()
        {
            return 0;
        }

        public override string ToString()
        {
            return "A";
        }


        private void Esvaziar()
        {
            // Buscar pontos proximos as lixeiras
            var list = new HashSet<Ambiente.Ponto>();

            foreach (var l in Ambiente.Lixeiras)
            {
                var ccc = Ambiente.getAdjascentes(l.X, l.Y);
                foreach (var adjascente in ccc)
                {
                    list.Add(adjascente);
                }
            }

            // Buscar caminho para o mais proximo
            var path = AStar(list);

            // vai
            for (int i = 0; i < path.Count; i++)
            {
                Ambiente.Move(path[i]);
            }

            //volta
            for (int i = path.Count - 1; i >= 0; i--)
            {
                Ambiente.Move(path[i]);
            }

        }

        private void Recarregar()
        {
            // Buscar pontos proximos as lixeiras
            var list = new HashSet<Ambiente.Ponto>();

            foreach (var l in Ambiente.Recargas)
            {
                var ccc = Ambiente.getAdjascentes(l.X, l.Y);
                foreach (var adjascente in ccc)
                {
                    list.Add(adjascente);
                }
            }

            // Buscar caminho para o mais proximo
            var path = AStar(list);

            // vai
            for (int i = 0; i < path.Count; i++)
            {
                Ambiente.Move(path[i]);
            }

            //volta
            for (int i = path.Count - 1; i >= 0; i--)
            {
                Ambiente.Move(path[i]);
            }
        }

        List<Ambiente.Ponto> AStar(IEnumerable<Ambiente.Ponto> pontosDestinosList)
        {
            var valor = int.MaxValue;
            List<Ambiente.Ponto> path = null;

            foreach (var ponto in pontosDestinosList)
            {
                
            }


            return path;
        }
    }
}
