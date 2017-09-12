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

        public Agente(int capacidadeRepositorio, int capacidadeBateria)
        {
            CapacidadeRepositorio = capacidadeRepositorio;
            Bateria = CapacidadeBateria = capacidadeBateria;
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

                if (Bateria < CapacidadeBateria * 0.3) // 30 %
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
            public int x;
            public int y;
            public int f => g + h;
            public int g;
            public int h;

            public NodoEstrela pai;

            public NodoEstrela(Ambiente.Ponto ponto)
            {
                x = ponto.X;
                y = ponto.Y;
                g = h = 0;
                pai = null;
            }
        }

        private List<Ambiente.Ponto> AEstrela(Ambiente.Ponto pDest)
        {
            var aberta = new List<NodoEstrela>();
            var fechada = new List<NodoEstrela>();

            aberta.Add(new NodoEstrela(Ambiente.getAgentePosicao()));

            while (true)
            {
                if (aberta.Count == 0)
                {
                    throw new Exception("Não encontramos caminho");
                }

                var atual = aberta.OrderBy(x => x.f).First();
                aberta.Remove(atual);
                fechada.Add(atual);

                if (atual.x == pDest.X && atual.y == pDest.Y)
                {
                    // Achei!! Montar caminho retorno
                    var ret = new List<Ambiente.Ponto>();

                    var r = fechada.Last();
                    while (r != null)
                    {
                        ret.Insert(0, new Ambiente.Ponto(r.x, r.y, null));
                        r = r.pai;
                    }

                    return ret;

                }

                foreach (var vizinho in Ambiente.getAdjascentes(atual.x, atual.y))
                {

                    if (!(vizinho.Item == null || vizinho.Item is Sujeira))
                    {
                        continue;
                    }

                    int g = atual.g + 1;

                    var find = aberta.Find(x => x.x == vizinho.X && x.y == vizinho.Y);
                    if (find == null)
                    {
                        var v = new NodoEstrela(vizinho);
                        v.h = Heuristica(vizinho.X, vizinho.Y, pDest.X, pDest.Y);
                        v.g = g;
                        v.pai = atual;
                        aberta.Add(v);
                    }
                    else if (find.g > g)
                    {
                        find.g = g;
                        find.pai = atual;
                    }

                }

            }

            return null;

        }

        private int Heuristica(int posX, int posY, int goalX, int goalY)
        {
            return Math.Abs(Math.Abs(goalX - posX) + Math.Abs(goalY - posY));

        }
    }
}
