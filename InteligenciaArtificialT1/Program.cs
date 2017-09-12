using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InteligenciaArtificialT1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parametros
            var iTamanho = 12; // Tamanho minimo
            var iLixeiras = 1; // Lixeiras
            var iRecargas = 1; // Pontos de Recargas
            var fatorSujeira = 0.1; // Sujeira
            var iRepositorio = 10; // Capacidade do Repositório
            var iBateria = 10; // Capacidade da Bateria

            var bSleep = true; // Sleep sim ou não



            foreach (var param in args)
            {

                if (param.StartsWith("/n:")) iTamanho = Math.Max(iTamanho, int.Parse(param.Replace("/n:", string.Empty)));

                if (param.StartsWith("/l:")) iLixeiras = Math.Max(iLixeiras, int.Parse(param.Replace("/l:", string.Empty)));

                if (param.StartsWith("/r:")) iRecargas = Math.Max(iRecargas, int.Parse(param.Replace("/r:", string.Empty)));

                if (param.StartsWith("/s:")) fatorSujeira = Math.Max(fatorSujeira, double.Parse(param.Replace("/s:", string.Empty)));

                if (param.StartsWith("/c:")) iRepositorio = Math.Max(iRepositorio, int.Parse(param.Replace("/c:", string.Empty)));

                if (param.StartsWith("/b:")) iBateria = Math.Max(iBateria, int.Parse(param.Replace("/b:", string.Empty)));

                if (param.StartsWith("/noSleep")) bSleep = false;

            }

            var lixeiras = new List<Lixeira>(iLixeiras);
            for (var i = 0; i < iLixeiras; i++) lixeiras.Add(new Lixeira());

            var recargas = new List<Recarga>(iRecargas);
            for (var i = 0; i < iRecargas; i++) recargas.Add(new Recarga());
            
            var agente = new Agente(iRepositorio, iBateria);

            var ambiente = new Models.Ambiente(iTamanho, agente, lixeiras, recargas, fatorSujeira, bSleep);

            Console.WriteLine(ambiente);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            agente.Run();

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            Console.WriteLine("Elapsed: " + ts);

            Console.WriteLine("Fim !!!");
            Console.ReadKey();
        }
    }
}