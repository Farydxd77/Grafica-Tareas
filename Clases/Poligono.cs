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

        // En Poligono.cs - método más robusto
        public Vector3 CalcularNormal()
        {
            if (Vertices.Count < 3) return Vector3.UnitY;

            // Usar el método de Newell para polígonos más complejos
            Vector3 normal = Vector3.Zero;

            for (int i = 0; i < Vertices.Count; i++)
            {
                Vector3 current = Vertices[i].ToVector3();
                Vector3 next = Vertices[(i + 1) % Vertices.Count].ToVector3();

                normal.X += (current.Y - next.Y) * (current.Z + next.Z);
                normal.Y += (current.Z - next.Z) * (current.X + next.X);
                normal.Z += (current.X - next.X) * (current.Y + next.Y);
            }

            return normal.LengthSquared > 0.0001f ? Vector3.Normalize(normal) : Vector3.UnitY;
        }

        public static Poligono CrearCaraCuadrada(Punto p1, Punto p2, Punto p3, Punto p4)
        {
            if (p1 == null || p2 == null || p3 == null || p4 == null)
                throw new ArgumentNullException("Ningún punto puede ser null");

            var cara = new Poligono();

            // Agregar vértices en orden counter-clockwise
            cara.AgregarVertice(p1);
            cara.AgregarVertice(p2);
            cara.AgregarVertice(p3);
            cara.AgregarVertice(p4);

            // Dos triángulos para formar el cuadrado
            // Primer triángulo: 0-1-2
            cara.AgregarIndices(0, 1, 2);
            // Segundo triángulo: 2-3-0  
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