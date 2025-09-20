using static Opentk_2222.Game;

namespace Opentk_2222.Clases
{
    public class Escenario
    {
        public List<Objeto> Objetos { get; set; }
        public string Nombre { get; set; }

        public Escenario(string nombre)
        {
            Nombre = nombre ?? "Escenario Sin Nombre";
            Objetos = new List<Objeto>();
        }

        public void AgregarObjeto(Objeto obj)
        {
            Objetos.Add(obj);
        }
        public void AgregarObjetos(params Objeto[] objd)
        {
            foreach (var obj in objd)
                Objetos.Add(obj);
        }
        public void Limpiar()
        {
            Objetos.Clear();
        }

    }
}