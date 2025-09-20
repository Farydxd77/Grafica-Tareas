using OpenTK;
using static Opentk_2222.Game;
namespace Opentk_2222.Clases
{
    public class Objeto
    {
        public string Nombre { get; set; }
        public List<Parte> Partes { get; set; }
        public Punto CentroMasa { get; private set; } // NECESARIO para serialización
        public Vector3 Posicion { get; set; }
        public Vector3 Rotacion { get; set; }         // MANTENIDO para compatibilidad
        public Vector3 Escala { get; set; }           // MANTENIDO para compatibilidad
        public Vector3 ColorBase { get; set; }

        public Objeto(string nombre)
        {
            Nombre = nombre ?? "Objeto Sin Nombre"; // FIX: NULL check
            Partes = new List<Parte>(); 
            CentroMasa = new Punto(0, 0, 0);        // FIX: Inicializar CentroMasa
            Posicion = Vector3.Zero;
            Rotacion = Vector3.Zero;                // FIX: Inicializar Rotacion
            Escala = Vector3.One;                   // FIX: Inicializar Escala
            ColorBase = Vector3.One;
        }

        public void AgregarParte(Parte parte)
        {
            Partes.Add(parte);
            CalcularCentroMasa(); // FIX: Necesario para mantener consistencia
        }

        public void AgregarPartes(params Parte[] partes)
        {
            foreach (var parte in partes)
                Partes.Add(parte);
            CalcularCentroMasa();
        }


        // FIX: Método necesario para mantener consistencia con serialización
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
    }
}