namespace InteligenciaArtificialT1
{
    public class Sujeira
    {
        public int Peso { get; private set; } = 1;

        public Sujeira(int valor)
        {
            Peso = valor;
        }

        public Sujeira()
        {
        }

        public override string ToString()
        {
            return " s ";
        }
    }

}
