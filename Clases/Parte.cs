using OpenTK;
namespace Opentk_2222.Clases
{
    public class Parte
    {
        public string Nombre { get; set; }
        public List<Poligono> Caras { get; set; }
        public Punto CentroMasa { get; private set; }
        public Vector3 Posicion { get; set; }
        public Vector3 Rotacion { get; set; }
        public Vector3 Escala { get; set; }
        public Vector3 Color { get; set; }

        public Parte(string nombre)
        {
            Nombre = nombre;
            Caras = new List<Poligono>();
            CentroMasa = new Punto(0, 0, 0);
            Posicion = Vector3.Zero;
            Rotacion = Vector3.Zero;
            Escala = Vector3.One;
            Color = Vector3.One;
        }

        public void AgregarCara(Poligono cara)
        {
            Caras.Add(cara);
            CalcularCentroMasa();
        }

        public void AgregarCaras(params Poligono[] caras)
        {
            foreach (var cara in caras)
            {
                Caras.Add(cara);
            }
            CalcularCentroMasa();
        }

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

        public static Parte CrearCubo(string nombre, Vector3 posicion, Vector3 tamaño, Vector3 color)
        {
            var parte = new Parte(nombre);
            parte.Posicion = posicion;
            parte.Color = color;

            float x = posicion.X, y = posicion.Y, z = posicion.Z;
            float w = tamaño.X / 2f, h = tamaño.Y / 2f, d = tamaño.Z / 2f;

            // Vértices del cubo - ORDEN CORRECTO
            var vertices = new Punto[]
            {
                // Cara frontal (Z+)
                new Punto(x - w, y - h, z + d), // 0 - inferior izquierda
                new Punto(x + w, y - h, z + d), // 1 - inferior derecha  
                new Punto(x + w, y + h, z + d), // 2 - superior derecha
                new Punto(x - w, y + h, z + d), // 3 - superior izquierda
                
                // Cara trasera (Z-)
                new Punto(x - w, y - h, z - d), // 4 - inferior izquierda
                new Punto(x + w, y - h, z - d), // 5 - inferior derecha
                new Punto(x + w, y + h, z - d), // 6 - superior derecha
                new Punto(x - w, y + h, z - d), // 7 - superior izquierda
            };

            // CREAR CARAS CON ORDEN CORRECTO (COUNTER-CLOCKWISE CUANDO SE VEN DESDE AFUERA)

            // Cara frontal (Z+) - mirando hacia nosotros
            var caraFrontal = Poligono.CrearCaraCuadrada(vertices[0], vertices[1], vertices[2], vertices[3]);

            // Cara trasera (Z-) - mirando hacia atrás  
            var caraTrasera = Poligono.CrearCaraCuadrada(vertices[5], vertices[4], vertices[7], vertices[6]);

            // Cara izquierda (X-)
            var caraIzquierda = Poligono.CrearCaraCuadrada(vertices[4], vertices[0], vertices[3], vertices[7]);

            // Cara derecha (X+)
            var caraDerecha = Poligono.CrearCaraCuadrada(vertices[1], vertices[5], vertices[6], vertices[2]);

            // Cara inferior (Y-)
            var caraInferior = Poligono.CrearCaraCuadrada(vertices[4], vertices[5], vertices[1], vertices[0]);

            // Cara superior (Y+)
            var caraSuperior = Poligono.CrearCaraCuadrada(vertices[3], vertices[2], vertices[6], vertices[7]);

            parte.AgregarCaras(caraFrontal, caraTrasera, caraIzquierda, caraDerecha, caraInferior, caraSuperior);

            return parte;
        }

        public override string ToString()
        {
            return $"{Nombre} - {Caras.Count} caras - Centro: {CentroMasa}";
        }
    }
}