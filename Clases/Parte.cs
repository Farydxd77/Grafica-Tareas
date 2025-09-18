using OpenTK;
namespace Opentk_2222.Clases
{
    public class Parte
    {
        public string Nombre { get; set; }
        public List<Poligono> Caras { get; set; }
        public Punto CentroMasa { get; private set; } // NECESARIO para serialización
        public Vector3 Posicion { get; set; }
        public Vector3 Rotacion { get; set; } // NECESARIO - Game.cs lo usa
        public Vector3 Escala { get; set; }   // NECESARIO - Game.cs lo usa
        public Vector3 Color { get; set; }

        public Parte(string nombre)
        {
            Nombre = nombre ?? "Parte Sin Nombre"; // FIX: NULL check
            Caras = new List<Poligono>();
            CentroMasa = new Punto(0, 0, 0);     // FIX: Inicializar CentroMasa
            Posicion = Vector3.Zero;
            Rotacion = Vector3.Zero;             // FIX: Inicializar Rotacion
            Escala = Vector3.One;                // FIX: Inicializar Escala
            Color = Vector3.One;
        }

        public void AgregarCara(Poligono cara)
        {
            Caras.Add(cara);
            CalcularCentroMasa(); // FIX: Necesario para serialización
        }

        public void AgregarCaras(params Poligono[] caras)
        {
            foreach (var cara in caras)
                Caras.Add(cara);
            CalcularCentroMasa();
        }

        // FIX: Método necesario para mantener consistencia
        private void CalcularCentroMasa()
        {
            if (Caras.Count == 0)
            {
                CentroMasa = new Punto(0, 0, 0);
                return;
            }

            var todosPuntos = new List<Punto>();
            foreach (var cara in Caras)
            {
                todosPuntos.AddRange(cara.Vertices);
            }

            CentroMasa = Punto.CalcularCentroMasa(todosPuntos);
        }

        // MÉTODO SIMPLIFICADO PARA CREAR CUBOS
        public static Parte CrearCubo(string nombre, Vector3 posicion, Vector3 tamaño, Vector3 color)
        {
            var parte = new Parte(nombre);
            parte.Posicion = posicion;
            parte.Color = color;

            // Calcular mitades del tamaño
            float w = tamaño.X * 0.5f;
            float h = tamaño.Y * 0.5f;
            float d = tamaño.Z * 0.5f;

            // 8 vértices de un cubo centrado en el origen
            var v = new Punto[]
            {
                new Punto(-w, -h, +d), new Punto(+w, -h, +d), new Punto(+w, +h, +d), new Punto(-w, +h, +d), // Frente
                new Punto(-w, -h, -d), new Punto(+w, -h, -d), new Punto(+w, +h, -d), new Punto(-w, +h, -d)  // Atrás
            };

            // 6 caras del cubo (orden counter-clockwise)
            parte.AgregarCara(Poligono.CrearCaraCuadrada(v[0], v[1], v[2], v[3])); // Frente
            parte.AgregarCara(Poligono.CrearCaraCuadrada(v[5], v[4], v[7], v[6])); // Atrás
            parte.AgregarCara(Poligono.CrearCaraCuadrada(v[4], v[0], v[3], v[7])); // Izquierda
            parte.AgregarCara(Poligono.CrearCaraCuadrada(v[1], v[5], v[6], v[2])); // Derecha
            parte.AgregarCara(Poligono.CrearCaraCuadrada(v[4], v[5], v[1], v[0])); // Abajo
            parte.AgregarCara(Poligono.CrearCaraCuadrada(v[3], v[2], v[6], v[7])); // Arriba

            return parte;
        }

        public override string ToString()
        {
            return $"{Nombre} - {Caras.Count} caras - Centro: {CentroMasa} - Pos: {Posicion}";
        }
    }
}