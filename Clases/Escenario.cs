
namespace Opentk_2222.Clases
{
    public class Escenario
    {
        public List<Objeto> Objetos { get; set; }
        public Punto CentroMasa { get; private set; }
        public string Nombre { get; set; }

        public Escenario(string nombre = "Escenario Principal")
        {
            Nombre = nombre;
            Objetos = new List<Objeto>();
            CentroMasa = new Punto(0, 0, 0);
        }

        public void AgregarObjeto(Objeto objeto)
        {
            Objetos.Add(objeto);
            CalcularCentroMasa();
        }

        public void AgregarObjetos(params Objeto[] objetos)
        {
            foreach (var objeto in objetos)
            {
                Objetos.Add(objeto);
            }
            CalcularCentroMasa();
        }

        public void EliminarObjeto(string nombre)
        {
            Objetos.RemoveAll(o => o.Nombre == nombre);
            CalcularCentroMasa();
        }

        public Objeto BuscarObjeto(string nombre)
        {
            return Objetos.Find(o => o.Nombre == nombre);
        }

        private void CalcularCentroMasa()
        {
            if (Objetos.Count == 0)
            {
                CentroMasa = new Punto(0, 0, 0);
                return;
            }

            var centrosMasa = Objetos.Select(o => o.CentroMasa).ToList();
            CentroMasa = Punto.CalcularCentroMasa(centrosMasa);
        }

        public void LimpiarEscenario()
        {
            Objetos.Clear();
            CentroMasa = new Punto(0, 0, 0);
        }

        public override string ToString()
        {
            return $"{Nombre} - {Objetos.Count} objetos - Centro: {CentroMasa}";
        }
    }
}
