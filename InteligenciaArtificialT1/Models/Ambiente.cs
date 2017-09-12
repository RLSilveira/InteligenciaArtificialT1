using System;
using System.Collections.Generic;
using System.Linq;

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

        int _tam;
        Ponto[] _mapa;

        public int GetTamanho() { return _mapa.Count(x => x == null || x.Item is null || x.Item is Sujeira); }

        public List<Ponto> Recargas { get; private set; }
        public List<Ponto> Lixeiras { get; private set; }

        Ponto _pAgente;

        private bool bSleep;
        public Ambiente(int tamanho, Agente agente, List<Lixeira> lixeiras, List<Recarga> recargas, double fatorSujeira, bool sleep)
        {

            bSleep = sleep;

            Recargas = new List<Ponto>(recargas.Count);
            Lixeiras = new List<Ponto>(lixeiras.Count);

            _tam = tamanho;
            _mapa = new Ponto[_tam * _tam];

            // Inicializa agente
            this._pAgente = _mapa[0] = new Ponto(0, 0, agente);
            agente.Ambiente = this;


            // Inicializa paredes
            int aux = _tam / 3 / 2;
            int col1 = aux;
            int col2 = _tam - aux - 1;

            int row1 = col1;
            int row2 = col2;

            int c1, c2;
            for (c1 = col1 + aux - 2, c2 = col2 - aux + 2; c1 < col1 + aux; c1++, c2--)
            {
                _mapa[(row1 * _tam) + c1] = new Ponto(row1, c1, new Parede());
                _mapa[(row2 * _tam) + c1] = new Ponto(row2, c1, new Parede());
                _mapa[(row1 * _tam) + c2] = new Ponto(row1, c2, new Parede());
                _mapa[(row2 * _tam) + c2] = new Ponto(row2, c2, new Parede());
            }
            c1--; c2++;
            for (int r = row1 + 1; r < row2; r++)
            {
                _mapa[(r * _tam) + c1] = new Ponto(r, c1, new Parede());
                _mapa[(r * _tam) + c2] = new Ponto(r, c2, new Parede());
            }

            var random = new Random();

            //TODO: Gerar lixeiras e recargas apenas dentro das paredes
            // gerar lixeiras
            int idx = 0;
            while (idx < lixeiras.Count && !IsFull)
            {
                var r = random.Next(_tam);
                var c = random.Next(_tam);
                if ((c == c1 - 1 || c == c2 + 1 || c == 0 || c == _tam - 1) && r > row1 && r < row2)
                {
                    if (_mapa[(r * _tam) + c] == null)
                    {
                        Lixeiras.Add(_mapa[(r * _tam) + c] = new Ponto(r, c, lixeiras[idx++]));
                    }
                }
            }

            // gerar recargas
            idx = 0;
            while (idx < recargas.Count && !IsFull)
            {
                var r = random.Next(_tam);
                var c = random.Next(_tam);
                if ((c == c1 - 1 || c == c2 + 1 || c == 0 || c == _tam - 1) && r > row1 && r < row2)
                {
                    if (_mapa[(r * _tam) + c] == null)
                    {
                        Recargas.Add(_mapa[(r * _tam) + c] = new Ponto(r, c, recargas[idx++]));
                        ;
                    }
                }
            }

            // gerar sujeiras
            var qntSujeiras = _mapa.Count(x => x == null) * fatorSujeira;
            var sujeiras = new List<Sujeira>((int)qntSujeiras);
            for (var i = 0; i < qntSujeiras; i++) sujeiras.Add(new Sujeira());


            idx = 0;
            while (idx < sujeiras.Count && !IsFull)
            {
                var r = random.Next(_tam);
                var c = random.Next(_tam);
                if (_mapa[(r * _tam) + c] == null)
                {
                    _mapa[(r * _tam) + c] = new Ponto(r, c, sujeiras[idx++]);
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
                    ((Agente)_pAgente.Item).Repositorio += s.Peso;
                    break;
            }

            ((Agente)_pAgente.Item).Bateria -= 1;

            _mapa[_pAgente.X * _tam + _pAgente.Y] = new Ponto(_pAgente.X, _pAgente.Y, null);
            _mapa[node.X * _tam + node.Y] = _pAgente;
            _pAgente.X = node.X;
            _pAgente.Y = node.Y;

            Console.Clear();
            Console.WriteLine(this);

            if (bSleep)
            {
                System.Threading.Thread.Sleep(150);
            }

        }

        public List<Ponto> GetAdjascentes(int x, int y)
        {
            var nodes = new List<Ponto>(8);

            for (int i = Math.Max(0, x - 1); i < Math.Min(x + 2, _tam); i++)
            {
                for (int j = Math.Max(0, y - 1); j < Math.Min(y + 2, _tam); j++)
                {
                    var n = _mapa[(i * _tam) + j];

                    if (i == x && j == y) continue;

                    if (n?.Item == null || n.Item is Sujeira)
                    {
                        nodes.Add(n ?? new Ponto(i, j, null));
                    }
                }
            }

            return nodes;

        }


        public Ponto GetDireita() => GetVizinho(0, 1);
        public Ponto GetEsquerda() => GetVizinho(0, -1);

        public Ponto GetCima() => GetVizinho(-1, 0);
        public Ponto GetBaixo() => GetVizinho(1, 0);

        public Ponto GetCimaEsquerda() => GetVizinho(-1, -1);
        public Ponto GetBaixoEsquerda() => GetVizinho(1, -1);

        public Ponto GetCimaDireita() => GetVizinho(-1, 1);
        public Ponto GetBaixoDireita() => GetVizinho(1, 1);

        private Ponto GetVizinho(int offsetX, int offsetY)
        {
            var x = _pAgente.X + offsetX;
            var y = _pAgente.Y + offsetY;

            if (x < 0 || x >= _tam || y < 0 || y >= _tam) return null;

            return _mapa[(x * _tam) + y] ?? new Ponto(_pAgente.X + offsetX, _pAgente.Y + offsetY, null);
        }

        public Ponto[] GetAdjscentesOrdenados(bool bInvert)
        {
            var r = bInvert
                ? new[]
                {
                    GetBaixo(),
                    GetCima(),
                    GetDireita(),
                }
                : new[]
                {
                    GetCima(),
                    GetBaixo(),
                    GetEsquerda(),
                };
            return r;
        }

        private bool IsFull => _mapa.Count(x => x == null) == 0;

        public override string ToString()
        {
            var r = "Inteligencia Artificial - T1\n" +
                    " Caetano Araujo\n" +
                    " Pedro Fraga\n" +
                    " Rodrigo Leão\n" +
                    "\n\t";

            for (var i = 0; i < _tam; i++)
            {
                for (var j = 0; j < _tam; j++)
                {
                    r += $"{_mapa[(i * _tam) + j]?.ToString() ?? " - "}";
                }

                r += "\n\t";

            }

            var agente = (Agente)_pAgente.Item;

            r += "\nInfos do Sistema\n";
            r += $"Bateria:     {agente.Bateria}/{agente.CapacidadeBateria}\n";
            r += $"Repositorio: {agente.Repositorio}/{agente.CapacidadeRepositorio}\n";

            return r;

        }


        public Ponto GetAgentePosicao()
        {
            return _pAgente;
        }
    }
}
