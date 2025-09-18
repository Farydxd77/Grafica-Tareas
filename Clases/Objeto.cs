
using OpenTK;
namespace Opentk_2222.Clases
{
    public class Objeto
    {
        public string Nombre { get; set; }
        public List<Parte> Partes { get; set; }
        public Punto CentroMasa { get; private set; }
        public Vector3 Posicion { get; set; }
        public Vector3 Rotacion { get; set; }
        public Vector3 Escala { get; set; }
        public Vector3 ColorBase { get; set; }

        public Objeto(string nombre)
        {
            Nombre = nombre;
            Partes = new List<Parte>();
            CentroMasa = new Punto(0, 0, 0);
            Posicion = Vector3.Zero;
            Rotacion = Vector3.Zero;
            Escala = Vector3.One;
            ColorBase = Vector3.One;
        }

        public void AgregarParte(Parte parte)
        {
            Partes.Add(parte);
            CalcularCentroMasa();
        }

        public void AgregarPartes(params Parte[] partes)
        {
            foreach (var parte in partes)
            {
                Partes.Add(parte);
            }
            CalcularCentroMasa();
        }

        public void EliminarParte(string nombreParte)
        {
            Partes.RemoveAll(p => p.Nombre == nombreParte);
            CalcularCentroMasa();
        }

        public Parte BuscarParte(string nombreParte)
        {
            return Partes.Find(p => p.Nombre == nombreParte);
        }

        private void CalcularCentroMasa()
        {
            if (Partes.Count == 0)
            {
                CentroMasa = new Punto(0, 0, 0);
                return;
            }

            var centrosMasa = Partes.Select(p => p.CentroMasa).ToList();
            CentroMasa = Punto.CalcularCentroMasa(centrosMasa);
        }

        public List<float> ObtenerVerticesParaRender()
        {
            var vertices = new List<float>();

            foreach (var parte in Partes)
            {
                foreach (var cara in parte.Caras)
                {
                    foreach (var vertice in cara.Vertices)
                    {
                        vertices.Add(vertice.X);
                        vertices.Add(vertice.Y);
                        vertices.Add(vertice.Z);

                        var normal = cara.CalcularNormal();
                        vertices.Add(normal.X);
                        vertices.Add(normal.Y);
                        vertices.Add(normal.Z);
                    }
                }
            }

            return vertices;
        }

        public List<uint> ObtenerIndicesParaRender()
        {
            var indices = new List<uint>();
            uint baseIndex = 0;

            foreach (var parte in Partes)
            {
                foreach (var cara in parte.Caras)
                {
                    foreach (var indice in cara.Indices)
                    {
                        indices.Add(baseIndex + indice);
                    }
                    baseIndex += (uint)cara.Vertices.Count;
                }
            }

            return indices;
        }

        public Vector3 ObtenerColorParte(string nombreParte)
        {
            var parte = BuscarParte(nombreParte);
            if (parte != null && parte.Color != Vector3.One)
                return parte.Color;

            return ColorBase;
        }

        public override string ToString()
        {
            return $"{Nombre} - {Partes.Count} partes - Centro: {CentroMasa}";
        }

    }
}