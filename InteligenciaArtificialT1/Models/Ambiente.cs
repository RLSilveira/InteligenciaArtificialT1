using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;

namespace InteligenciaArtificialT1.Models
{
    public class Ambiente
    {
        public class Ponto
        {
            public int X { get; set; }
            public int Y { get; set; }
            public object Item { get; private set; }

            public Ponto(int x, int y, object obj)
            {
                X = x; Y = y;
                Item = obj;
            }

            public override string ToString()
            {
                return Item?.ToString();
            }

            public override bool Equals(object obj)
            {
                var b = this.X == (obj as Ponto)?.X && this.Y == (obj as Ponto)?.Y;
                return b;
            }

        }

        int tam;
        Ponto[] mapa;

        public int getTamanho() { return mapa.Count(x => x == null || x.Item is null || x.Item is Sujeira); }

        public List<Ponto> Recargas { get; private set; }
        public List<Ponto> Lixeiras { get; private set; }

        Ponto pAgente;

        public Ambiente(int tamanho, Agente agente, List<Lixeira> lixeiras, List<Recarga> recargas, double fatorSujeira)
        {



            Recargas = new List<Ponto>(recargas.Count);
            Lixeiras = new List<Ponto>(lixeiras.Count);

            tam = tamanho;
            mapa = new Ponto[tam * tam];

            // Inicializa agente
            this.pAgente = mapa[0] = new Ponto(0, 0, agente);
            agente.Ambiente = this;


            // Inicializa paredes
            int aux = tam / 3 / 2;
            int col1 = aux;
            int col2 = tam - aux - 1;

            int row1 = col1;
            int row2 = col2;

            int c1, c2;
            for (c1 = col1 + aux -2, c2 = col2 - aux + 2; c1 < col1 + aux; c1++, c2--)
            {
                mapa[(row1 * tam) + c1] = new Ponto(row1, c1, new Parede());
                mapa[(row2 * tam) + c1] = new Ponto(row2, c1, new Parede());
                mapa[(row1 * tam) + c2] = new Ponto(row1, c2, new Parede());
                mapa[(row2 * tam) + c2] = new Ponto(row2, c2, new Parede());
            }
            c1--; c2++;
            for (int r = row1 + 1; r < row2; r++)
            {
                mapa[(r * tam) + c1] = new Ponto(r, c1, new Parede());
                mapa[(r * tam) + c2] = new Ponto(r, c2, new Parede());
            }

            var random = new Random();

            //TODO: Gerar lixeiras e recargas apenas dentro das paredes
            // gerar lixeiras
            int idx = 0;
            while (idx < lixeiras.Count && !isFull)
            {
                var r = random.Next(tam);
                var c = random.Next(tam);
                if ((c == c1 - 1 || c == c2 + 1 || c == 0 || c == tam - 1) && r > row1 && r < row2)
                {
                    if (mapa[(r * tam) + c] == null)
                    {
                        Lixeiras.Add(mapa[(r * tam) + c] = new Ponto(r, c, lixeiras[idx++]));
                    }
                }
            }

            // gerar recargas
            idx = 0;
            while (idx < recargas.Count && !isFull)
            {
                var r = random.Next(tam);
                var c = random.Next(tam);
                if ((c == c1 - 1 || c == c2 + 1 || c == 0 || c == tam - 1) && r > row1 && r < row2)
                {
                    if (mapa[(r * tam) + c] == null)
                    {
                        Recargas.Add(mapa[(r * tam) + c] = new Ponto(r, c, recargas[idx++]));
                        ;
                    }
                }
            }

            // gerar sujeiras
            var qntSujeiras = mapa.Count(x => x == null) * fatorSujeira;
            var sujeiras = new List<Sujeira>((int)qntSujeiras);
            for (var i = 0; i < qntSujeiras; i++) sujeiras.Add(new Sujeira());


            idx = 0;
            while (idx < sujeiras.Count && !isFull)
            {
                var r = random.Next(tam);
                var c = random.Next(tam);
                if (mapa[(r * tam) + c] == null)
                {
                    mapa[(r * tam) + c] = new Ponto(r, c, sujeiras[idx++]);
                }
            }

        }

        internal void Move(Ponto node)
        {
            switch (node.Item)
            {
                case Parede _:
                case Recarga _:
                case Lixeira _:
                    throw new Exception("Não deveria chega aqui");
                case Sujeira s:
                    ((Agente)pAgente.Item).Repositorio += s.peso;
                    break;
            }

            ((Agente)pAgente.Item).Bateria -= 1;

            mapa[pAgente.X * tam + pAgente.Y] = new Ponto(pAgente.X, pAgente.Y, null);
            mapa[node.X * tam + node.Y] = pAgente;
            pAgente.X = node.X;
            pAgente.Y = node.Y;

            Console.Clear();
            Console.WriteLine(this);
            System.Threading.Thread.Sleep(150);

        }

        public List<Ponto> getAdjascentes(int x, int y)
        {
            var nodes = new List<Ponto>(8);

            for (int i = Math.Max(0, x - 1); i < Math.Min(x + 2, tam); i++)
            {
                for (int j = Math.Max(0, y - 1); j < Math.Min(y + 2, tam); j++)
                {
                    var n = mapa[(i * tam) + j];

                    if (i == x && j == y) continue;

                    if (n?.Item == null || n.Item is Sujeira)
                    {
                        nodes.Add(n ?? new Ponto(i, j, null));
                    }
                }
            }

            return nodes;

        }


        public Ponto getDireita() => getVizinho(0, 1);
        public Ponto getEsquerda() => getVizinho(0, -1);

        public Ponto getCima() => getVizinho(-1, 0);
        public Ponto getBaixo() => getVizinho(1, 0);

        public Ponto getCimaEsquerda() => getVizinho(-1, -1);
        public Ponto getBaixoEsquerda() => getVizinho(1, -1);

        public Ponto getCimaDireita() => getVizinho(-1, 1);
        public Ponto getBaixoDireita() => getVizinho(1, 1);

        private Ponto getVizinho(int offsetX, int offsetY)
        {
            var x = pAgente.X + offsetX;
            var y = pAgente.Y + offsetY;

            if (x < 0 || x >= tam || y < 0 || y >= tam) return null;

            return mapa[(x * tam) + y] ?? new Ponto(pAgente.X + offsetX, pAgente.Y + offsetY, null);
        }

        public Ponto[] getAdjscentesOrdenados(bool bInvert)
        {

            //return new[] {getCima()};

            var r = bInvert
                ? new[]
                {
                    getBaixo(),
                    getCima(),
                    getDireita(),
                }
                : new[]
                {
                    getCima(),
                    getBaixo(),
                    getEsquerda(),
                };
            return r;
        }

        private bool isFull => mapa.Count(x => x == null) == 0;

        public override string ToString()
        {
            var r = "Inteligencia Artificial\n" +
                    "Caetano\n" +
                    "Pedro\n" +
                    "Rodrigo\n" +
                    "\n\t";

            for (int i = 0; i < tam; i++)
            {
                for (int j = 0; j < tam; j++)
                {
                    r += $" {mapa[(i * tam) + j]?.ToString() ?? "-"} ";
                }

                r += "\n\t";

            }

            var agente = (Agente)pAgente.Item;

            r += "\nInfos\n";
            r += $"Bateria:     {agente.Bateria}/{agente.CapacidadeBateria}\n";
            r += $"Repositorio: {agente.Repositorio}/{agente.CapacidadeRepositorio}\n";

            return r;

        }


        public Ponto getAgentePosicao()
        {
            return pAgente;
        }
    }
}
