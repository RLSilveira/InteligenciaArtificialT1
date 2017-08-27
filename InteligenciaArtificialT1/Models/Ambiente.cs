using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace InteligenciaArtificialT1.Models
{
    public class Ambiente
    {
        public class Node
        {
            public int X { get; set; }
            public int Y { get; set; }
            public object Item { get; private set; }

            public Node(int x, int y, object obj)
            {
                X = x; Y = y;
                Item = obj;
            }

            public override string ToString()
            {
                //return $" {Item?.ToString() ?? "-"} ";
                return Item.ToString();
            }
        }

        int tam;
        Node[] matriz;


        public List<Node> Recargas { get; private set; }
        public List<Node> Lixeiras { get; private set; }

        Node agente;

        public Ambiente(int tamanho, Agente agente, List<Lixeira> lixeiras, List<Recarga> recargas, List<Sujeira> sujeiras)
        {
            Recargas = new List<Node>(recargas.Count);
            Lixeiras = new List<Node>(sujeiras.Count);

            tam = tamanho;
            matriz = new Node[tam * tam];

            // Inicializa agente
            this.agente = matriz[0] = new Node(0, 0, agente);
            (this.agente.Item as Agente).Ambiente = this;


            // Inicializa paredes
            int aux = tam / 3 / 2;
            int col1 = aux;
            int col2 = tam - aux - 1;

            int row1 = col1;
            int row2 = col2;

            int c1, c2;
            for (c1 = col1, c2 = col2; c1 < col1 + aux; c1++, c2--)
            {
                matriz[(row1 * tam) + c1] = new Node(row1, c1, new Parede());
                matriz[(row2 * tam) + c1] = new Node(row2, c1, new Parede());
                matriz[(row1 * tam) + c2] = new Node(row1, c2, new Parede());
                matriz[(row2 * tam) + c2] = new Node(row2, c2, new Parede());
            }
            c1--; c2++;
            for (int r = row1 + 1; r < row2; r++)
            {
                matriz[(r * tam) + c1] = new Node(r, c1, new Parede());
                matriz[(r * tam) + c2] = new Node(r, c2, new Parede());
            }

            var random = new Random();

            //TODO: Gerar lixeiras e recargas apenas dentro das paredes
            // gerar lixeiras
            int idx = 0;
            while (idx < lixeiras.Count && !isFull)
            {
                var r = random.Next(tam);
                var c = random.Next(tam);
                if (matriz[(r * tam) + c] == null)
                {
                    Lixeiras.Add(matriz[(r * tam) + c] = new Node(r, c, lixeiras[idx++]));
                }
            }

            // gerar recargas
            idx = 0;
            while (idx < recargas.Count && !isFull)
            {
                var r = random.Next(tam);
                var c = random.Next(tam);
                if (matriz[(r * tam) + c] == null)
                {
                    Recargas.Add(matriz[(r * tam) + c] = new Node(r, c, recargas[idx++]));
                    ;
                }
            }

            // gerar sujeiras
            idx = 0;
            while (idx < sujeiras.Count && !isFull)
            {
                var r = random.Next(tam);
                var c = random.Next(tam);
                if (matriz[(r * tam) + c] == null)
                {
                    matriz[(r * tam) + c] = new Node(r, c, sujeiras[idx++]);
                }
            }

        }

        internal void Move(Node node)
        {
            //if (node.Item is Sujeira)

            matriz[agente.X * tam + agente.Y] = null;
            matriz[node.X * tam + node.Y] = agente;
            agente.X = node.X;
            agente.Y = node.Y;

            Console.WriteLine(this);

        }

        public List<Node> getAdjascentes()
        {
            var nodes = new List<Node>(8);

            var x = agente.X;
            var y = agente.Y;

            for (int i = Math.Max(0, x - 1); i < agente.X + 2; i++)
            {
                for (int j = Math.Max(0, y - 1); j < agente.Y + 2; j++)
                {
                    var n = matriz[(i * tam) + j];
                    if (n == null || n.Item is Sujeira)
                    {
                        nodes.Add(n ?? new Node(i, j, null));
                    }
                }
            }


            return nodes;
        }

        private bool isFull => matriz.Count(x => x == null) == 0;

        public override string ToString()
        {
            var r = "\t";

            for (int i = 0; i < tam; i++)
            {
                for (int j = 0; j < tam; j++)
                {
                    r += $" {matriz[(i * tam) + j]?.ToString() ?? "-"} ";
                }

                r += "\n\t";

            }

            return r;

        }


    }
}
