using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace InteligenciaArtificialT1.Models
{
    public class Ambiente
    {
        private class Casa
        {
            public object item { get; set; }

            public override string ToString()
            {
                return $" {item?.ToString() ?? "."} ";
            }
        }

        int tam, countMatriz = 0;
        Casa[,] matriz;


        public Ambiente(int tamanho, Agente agente, List<Lixeira> lixeiras, List<Recarga> recargas, List<Sujeira> sujeiras)
        {
            tam = tamanho;
            matriz = new Casa[tamanho, tamanho];

            // Inicializa matriz vazia
            for (int i = 0; i < tamanho; i++)
            {
                for (int j = 0; j < tamanho; j++)
                {
                    matriz[i, j] = new Casa();
                }
            }

            // Inicializa agente
            matriz[0, 0].item = agente;
            countMatriz++;

            // Inicializa paredes
            int aux = tam / 3 / 2;
            int col1 = aux;
            int col2 = tam - aux - 1;

            int row1 = col1;
            int row2 = col2;

            int c1, c2;
            for (c1 = col1, c2 = col2; c1 < col1 + aux; c1++, c2--)
            {
                matriz[row1, c1].item = new Parede(); countMatriz++;
                matriz[row2, c1].item = new Parede(); countMatriz++;
                matriz[row1, c2].item = new Parede(); countMatriz++;
                matriz[row2, c2].item = new Parede(); countMatriz++;
            }
            c1--; c2++;
            for (int r = row1 + 1; r < row2; r++)
            {
                matriz[r, c1].item = new Parede(); countMatriz++;
                matriz[r, c2].item = new Parede(); countMatriz++;
            }

            var random = new Random();

            //TODO: Gerar lixeiras e recargas apenas dentro das paredes
            // gerar lixeiras
            int idx = 0;
            while (idx < lixeiras.Count)
            {
                var r = random.Next(tam);
                var c = random.Next(tam);
                if(matriz[r,c].item == null)
                {
                    matriz[r, c].item = lixeiras[idx++]; countMatriz++;
                }
            }

            // gerar recargas
            idx = 0;
            while (idx < recargas.Count)
            {
                var r = random.Next(tam);
                var c = random.Next(tam);
                if (matriz[r, c].item == null)
                {
                    matriz[r, c].item = recargas[idx++]; countMatriz++;
                }
            }

            // gerar sujeiras
            idx = 0;
            while (idx < sujeiras.Count && countMatriz < matriz.Length)
            {
                var r = random.Next(tam);
                var c = random.Next(tam);
                if (matriz[r, c].item == null)
                {
                    matriz[r, c].item = sujeiras[idx++]; countMatriz++;
                }
            }

        }

        public override string ToString()
        {
            var r = "\t";

            for (int i = 0; i < tam; i++)
            {
                for (int j = 0; j < tam; j++)
                {
                    r += matriz[i, j];
                }

                r += "\n\t";

            }
            return r;
        }

        void PreencheMatriz(List<Lixeira> lixeiras, List<Recarga> recargas)
        {

        }
    }
}
