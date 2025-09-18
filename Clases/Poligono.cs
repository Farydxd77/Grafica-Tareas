using OpenTK;
namespace Opentk_2222.Clases
{
    public class Poligono
    {
        public List<Punto> Vertices { get; set; }
        public List<uint> Indices { get; set; }

        public Poligono()
        {
            Vertices = new List<Punto>();
            Indices = new List<uint>();
        }

        public void AgregarVertice(Punto punto)
        {
            Vertices.Add(punto);
        }

        public void AgregarIndices(params uint[] indices)
        {
            foreach (uint indice in indices)
                Indices.Add(indice);
        }

        // MÉTODO SIMPLIFICADO PARA CALCULAR NORMAL
        public Vector3 CalcularNormal()
        {
            if (Vertices.Count < 3) return Vector3.UnitY;

            // Usar los primeros 3 vértices para calcular la normal
            Vector3 v1 = Vertices[0].ToVector3();
            Vector3 v2 = Vertices[1].ToVector3();
            Vector3 v3 = Vertices[2].ToVector3();

            Vector3 edge1 = v2 - v1;
            Vector3 edge2 = v3 - v1;
            Vector3 normal = Vector3.Cross(edge1, edge2);

            return normal.LengthSquared > 0.0001f ? Vector3.Normalize(normal) : Vector3.UnitY;
        }

        // MÉTODO ESTÁTICO SIMPLIFICADO PARA CREAR CARAS
        public static Poligono CrearCaraCuadrada(Punto p1, Punto p2, Punto p3, Punto p4)
        {
            var cara = new Poligono();

            // Agregar vértices
            cara.AgregarVertice(p1);
            cara.AgregarVertice(p2);
            cara.AgregarVertice(p3);
            cara.AgregarVertice(p4);

            // Dos triángulos: 0-1-2 y 2-3-0
            cara.AgregarIndices(0, 1, 2, 2, 3, 0);

            return cara;
        }
    }
}