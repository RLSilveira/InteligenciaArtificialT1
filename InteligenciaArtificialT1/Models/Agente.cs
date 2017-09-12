using System;
using System.Collections.Generic;
using System.Linq;
using InteligenciaArtificialT1.Models;

namespace InteligenciaArtificialT1
{
    public class Agente
    {

        public int CapacidadeBateria = 20;
        public int CapacidadeRepositorio = 20;

        public int Bateria { get; set; }
        public int Repositorio { get; set; }
        public bool IsRepositorioFull => Repositorio >= CapacidadeRepositorio;


        public int UltimaCelula { get; set; }
        public Ambiente Ambiente { get; set; }

        public bool AEstrelaMode { get; set; }= false;

        public Agente(int capacidadeRepositorio, int capacidadeBateria)
        {
            CapacidadeRepositorio = capacidadeRepositorio;
            Bateria = CapacidadeBateria = capacidadeBateria;
        }

        public void Run()
        {

            var bDesce = true;

            while (true)
            {

                if (Bateria < CapacidadeBateria * 0.3) // 30 %
                {
                    Recarregar();
                }

                if (Repositorio == CapacidadeRepositorio)
                {
                    Esvaziar();
                }

                Ambiente.Ponto p = null;
                if (bDesce)
                {
                    p = Ambiente.GetBaixo();

                    if (p == null)
                    {
                        // fim do mapa
                        p = Ambiente.GetDireita();
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

                    Ambiente.Move(p);

                }
                else
                {
                    p = Ambiente.GetCima();

                    if (p == null)
                    {
                        // fim do mapa
                        p = Ambiente.GetDireita();
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

                    Ambiente.Move(p);
                }

            }

        }

        void Contornar(bool bDesce)
        {
            bool esquerda;
            if (bDesce)
            {
                var p = Ambiente.GetBaixoEsquerda();
                esquerda = true;

                if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                {
                    esquerda = false;
                    p = Ambiente.GetBaixoDireita();
                }

                if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                {
                    esquerda = true;
                    p = Ambiente.GetEsquerda();
                }

                if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                {
                    esquerda = false;
                    p = Ambiente.GetDireita();
                }


                Ambiente.Move(p);


                if (!esquerda)
                {
                    DescendoVoltaEsquerda:
                    p = Ambiente.GetBaixoEsquerda();
                    if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                    {
                        p = Ambiente.GetBaixo();
                        Ambiente.Move(p);
                        goto DescendoVoltaEsquerda;
                    }
                }
                else
                {
                    DescendoVoltaDireita:
                    p = Ambiente.GetBaixoDireita();
                    if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                    {
                        p = Ambiente.GetBaixo();
                        Ambiente.Move(p);
                        goto DescendoVoltaDireita;
                    }

                }

                Ambiente.Move(p);

            }
            else // Sobe
            {
                var p = Ambiente.GetCimaEsquerda();
                esquerda = true;

                if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                {
                    esquerda = false;
                    p = Ambiente.GetCimaDireita();
                }

                if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                {
                    p = Ambiente.GetEsquerda();
                    esquerda = true;
                }

                if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                {
                    esquerda = false;
                    p = Ambiente.GetDireita();
                }


                Ambiente.Move(p);


                if (!esquerda)
                {
                    SubindoVoltaEsquerda:
                    p = Ambiente.GetCimaEsquerda();
                    if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                    {
                        p = Ambiente.GetCima();
                        Ambiente.Move(p);
                        goto SubindoVoltaEsquerda;
                    }
                }
                else
                {
                    SubindoVoltaDireita:
                    p = Ambiente.GetCimaDireita();
                    if (p == null || p.Item is Recarga || p.Item is Lixeira || p.Item is Parede)
                    {
                        p = Ambiente.GetCima();
                        Ambiente.Move(p);
                        goto SubindoVoltaDireita;
                    }

                }

                Ambiente.Move(p);

            }
        }


        public override string ToString()
        {
            return AEstrelaMode ? " A*" : " A ";
        }


        private void Esvaziar()
        {
            // Buscar pontos proximos as lixeiras
            var list = new HashSet<Ambiente.Ponto>();

            foreach (var l in Ambiente.Lixeiras)
            {
                var ccc = Ambiente.GetAdjascentes(l.X, l.Y);
                foreach (var adjascente in ccc)
                {
                    list.Add(adjascente);
                }
            }

            AEstrelaMode = true;

            // Buscar caminho para o mais proximo
            var path = GetMenorCaminho(list);

            // vai
            for (int i = 0; i < path.Count; i++)
            {
                Ambiente.Move(path[i]);
            }

            // esvaziar
            Repositorio = 0;

            //volta
            for (int i = path.Count - 2; i >= 0; i--)
            {
                Ambiente.Move(path[i]);
            }

            AEstrelaMode = false;

        }

        private void Recarregar()
        {
            // Buscar pontos proximos as lixeiras
            var list = new HashSet<Ambiente.Ponto>();

            foreach (var l in Ambiente.Recargas)
            {
                var ccc = Ambiente.GetAdjascentes(l.X, l.Y);
                foreach (var adjascente in ccc)
                {
                    list.Add(adjascente);
                }
            }

            AEstrelaMode = true;

            // Buscar caminho para o mais proximo
            var path = GetMenorCaminho(list);

            // vai
            for (int i = 0; i < path.Count; i++)
            {
                Ambiente.Move(path[i]);
            }

            Bateria = CapacidadeBateria;

            //volta
            for (int i = path.Count - 1; i >= 0; i--)
            {
                Ambiente.Move(path[i]);
            }

            AEstrelaMode = false;

        }

        List<Ambiente.Ponto> GetMenorCaminho(IEnumerable<Ambiente.Ponto> pontosDestinosList)
        {
            List<Ambiente.Ponto> path = null;

            foreach (var ponto in pontosDestinosList)
            {
                var aux = AEstrela(ponto);
                if (aux.Count < (path?.Count ?? int.MaxValue))
                {
                    path = aux;
                }
            }

            return path;
        }


        private class NodoEstrela
        {
            public int X;
            public int Y;
            public int F => G + H;
            public int G; // Custo
            public int H; // Heuristica

            public NodoEstrela Pai;

            public NodoEstrela(Ambiente.Ponto ponto)
            {
                X = ponto.X;
                Y = ponto.Y;
                G = H = 0;
                Pai = null;
            }
        }

        private List<Ambiente.Ponto> AEstrela(Ambiente.Ponto pDest)
        {
            var aberta = new List<NodoEstrela>();
            var fechada = new List<NodoEstrela>();

            aberta.Add(new NodoEstrela(Ambiente.GetAgentePosicao()));

            while (true)
            {
                if (aberta.Count == 0)
                {
                    throw new Exception("Não encontramos caminho");
                }

                var atual = aberta.OrderBy(x => x.F).First();
                aberta.Remove(atual);
                fechada.Add(atual);

                if (atual.X == pDest.X && atual.Y == pDest.Y)
                {
                    // Achei!! Montar caminho retorno
                    var ret = new List<Ambiente.Ponto>();

                    var r = fechada.Last();
                    while (r != null)
                    {
                        ret.Insert(0, new Ambiente.Ponto(r.X, r.Y, null));
                        r = r.Pai;
                    }

                    return ret;

                }

                foreach (var vizinho in Ambiente.GetAdjascentes(atual.X, atual.Y))
                {

                    if (!(vizinho.Item == null || vizinho.Item is Sujeira))
                    {
                        continue;
                    }

                    int novoG = atual.G + 1;

                    var nodoJaVisitado = aberta.Find(x => x.X == vizinho.X && x.Y == vizinho.Y);
                    if (nodoJaVisitado == null)
                    {
                        var novoNodo =
                            new NodoEstrela(vizinho)
                            {
                                H = Heuristica(vizinho.X, vizinho.Y, pDest.X, pDest.Y),
                                G = novoG,
                                Pai = atual
                            };
                        aberta.Add(novoNodo);
                    }
                    else if (nodoJaVisitado.G > novoG)
                    {
                        // Atualiza Custo
                        nodoJaVisitado.G = novoG;
                        nodoJaVisitado.Pai = atual;
                    }

                }

            }
        }

        private static int Heuristica(int posX, int posY, int goalX, int goalY)
        {
            return Math.Abs(Math.Abs(goalX - posX) + Math.Abs(goalY - posY));

        }
    }
}
