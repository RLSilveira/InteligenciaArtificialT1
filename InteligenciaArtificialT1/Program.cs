using System;
using System.Collections.Generic;
using System.Linq;

namespace InteligenciaArtificialT1
{
    class Program
    {
        static void Main(string[] args)
        {
            int TAM = 12; // Tamanho minimo
            if (args.Length > 0) TAM = Math.Max(TAM, int.Parse(args[0]));

            int iLixeiras = 1; // Lixeiras
            if (args.Length > 1) iLixeiras = Math.Max(iLixeiras, int.Parse(args[1]));
            var lixeiras = new List<Lixeira>(iLixeiras);
            for (int i = 0; i < iLixeiras; i++)
            {
                lixeiras.Add(new Lixeira());
            }

            int iRecargas = 1; // Pontos de Recargas
            if (args.Length > 2) iRecargas = Math.Max(iRecargas, int.Parse(args[2]));
            var recargas = new List<Recarga>(iRecargas);
            for (int i = 0; i < iRecargas; i++)
            {
                recargas.Add(new Recarga());
            }

            double fatorSujeira = 0.1; // Sujeira
            if (args.Length > 3) fatorSujeira = Math.Max(fatorSujeira, double.Parse(args[3]));
            var qntSujeiras = TAM * TAM * fatorSujeira;
            var sujeiras = new List<Sujeira>((int)qntSujeiras);
            for (int i = 0; i < qntSujeiras; i++)
            {
                sujeiras.Add(new Sujeira());
            }

            Console.WriteLine("Inteligência Artificial T1");
            Console.WriteLine($"TAMANHO: {TAM}");

            var agente = new Agente();

            var a = new Models.Ambiente(TAM, agente, lixeiras, recargas, sujeiras);

            Console.WriteLine(a);

            agente.run();

            Console.WriteLine("Fim !!!");
            //Console.ReadKey();
        }
    }
}