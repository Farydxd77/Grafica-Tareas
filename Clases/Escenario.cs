namespace Opentk_2222.Clases
{
    public class Escenario
    {
        public List<Objeto> Objetos { get; set; }
        public string Nombre { get; set; }

        public Escenario(string nombre = "Escenario Principal")
        {
            Nombre = nombre;
            Objetos = new List<Objeto>();
        }

        public void AgregarObjeto(Objeto objeto)
        {
            Objetos.Add(objeto);
        }

        public void AgregarObjetos(params Objeto[] objetos)
        {
            foreach (var objeto in objetos)
                Objetos.Add(objeto);
        }

        public void EliminarObjeto(string nombre)
        {
            Objetos.RemoveAll(o => o.Nombre == nombre);
        }

        public Objeto BuscarObjeto(string nombre)
        {
            return Objetos.Find(o => o.Nombre == nombre);
        }

        public void LimpiarEscenario()
        {
            Objetos.Clear();
        }

        //public override string ToString()
        //{
        //    return $"{Nombre} - {Objetos.Count} objetos";
        //}
    }
}