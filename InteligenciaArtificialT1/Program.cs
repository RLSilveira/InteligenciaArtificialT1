﻿using System;
using System.Collections.Generic;
using System.Linq;

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
            var iRepositorio = 10; // Capacidade da Bateria



            foreach (var param in args)
            {

                if (param.StartsWith("/n:")) iTamanho = Math.Max(iTamanho, int.Parse(param.Replace("/n:", string.Empty)));

                if (param.StartsWith("/l:")) iLixeiras = Math.Max(iLixeiras, int.Parse(param.Replace("/l:", string.Empty)));

                if (param.StartsWith("/r:")) iRecargas = Math.Max(iRecargas, int.Parse(param.Replace("/r:", string.Empty)));

                if (param.StartsWith("/s:")) fatorSujeira = Math.Max(fatorSujeira, double.Parse(param.Replace("/s:", string.Empty)));

                if (param.StartsWith("/c:")) iRepositorio = Math.Max(iRepositorio, int.Parse(param.Replace("/c:", string.Empty)));

            }

            var lixeiras = new List<Lixeira>(iLixeiras);
            for (var i = 0; i < iLixeiras; i++) lixeiras.Add(new Lixeira());

            var recargas = new List<Recarga>(iRecargas);
            for (var i = 0; i < iRecargas; i++) recargas.Add(new Recarga());

            
            var agente = new Agente(iRepositorio);

            var ambiente = new Models.Ambiente(iTamanho, agente, lixeiras, recargas, fatorSujeira);

            Console.WriteLine(ambiente);

            agente.Run();

            Console.WriteLine("Fim !!!");
            Console.ReadKey();
        }
    }
}