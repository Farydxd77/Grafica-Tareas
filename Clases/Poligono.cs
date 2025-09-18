
using OpenTK;
namespace Opentk_2222.Clases
{
    public class Poligono
    {
        public List<Punto> Vertices { get; set; }
        public List<uint> Indices { get; set; }
        public Punto CentroMasa { get; private set; }

        public Poligono(int capacidadVertices = 4)
        {
            Vertices = new List<Punto>(capacidadVertices);
            Indices = new List<uint>(capacidadVertices * 2);
            CentroMasa = new Punto(0, 0, 0);
        }

        public void AgregarVertice(Punto punto)
        {
            Vertices.Add(punto);
            CalcularCentroMasa();
        }

        public void AgregarIndice(uint indice)
        {
            if (indice >= Vertices.Count)
                throw new ArgumentException($"Indice {indice} no existe solo hay {Vertices.Count} vertices");

            Indices.Add(indice);
        }

        public void AgregarIndices(params uint[] indices)
        {
            foreach (uint indice in indices)
            {
                Indices.Add(indice);
            }
        }

        private void CalcularCentroMasa()
        {
            CentroMasa = Punto.CalcularCentroMasa(Vertices);
        }

        public Vector3 CalcularNormal()
        {
            if (Vertices.Count < 3) return Vector3.UnitY;

            Vector3 v1 = Vertices[1].ToVector3() - Vertices[0].ToVector3();
            Vector3 v2 = Vertices[2].ToVector3() - Vertices[0].ToVector3();
            Vector3 normal = Vector3.Cross(v1, v2);

            return normal.LengthSquared > 0 ? Vector3.Normalize(normal) : Vector3.UnitY;
        }

        public static Poligono CrearCaraCuadrada(Punto p1, Punto p2, Punto p3, Punto p4)
        {
            if (p1 == null || p2 == null || p3 == null || p4 == null)
                throw new ArgumentNullException("Ningún punto puede ser null");

            var cara = new Poligono();

            cara.AgregarVertice(p1);
            cara.AgregarVertice(p2);
            cara.AgregarVertice(p3);
            cara.AgregarVertice(p4);

            cara.AgregarIndices(0, 1, 2);
            cara.AgregarIndices(2, 3, 0);

            return cara;
        }

        public void Limpiar()
        {
            Vertices.Clear();
            Indices.Clear();
            CentroMasa = new Punto(0, 0, 0);
        }
    }
}