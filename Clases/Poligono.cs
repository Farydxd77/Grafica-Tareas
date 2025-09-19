using OpenTK;
namespace Opentk_2222.Clases
{
    public class Poligono
    {
            
        public List<Punto> Vertices { set; get; }
        public List<uint> Indices { set; get; }

        public Poligono()
        {
            Vertices = new List<Punto>();
            Indices = new List<uint>();
        }

        public void AgregarVertice(Punto p)
        {
            Vertices.Add(p);
        }
        public void AgregarIndices( params uint[] indices)
        {
            foreach (var index in indices){
                Indices.Add(index);
            }
        }
        public static Poligono CrearCaraCuadrada( Punto p1, Punto p2, Punto p3, Punto p4)
        {
            var cara = new Poligono();  

            cara.AgregarVertice(p1);
            cara.AgregarVertice(p2);    
            cara.AgregarVertice(p3);
            cara.AgregarVertice(p4);

            cara.AgregarIndices(0,1,2, 2,3,0); // Dos triángulos para un cuadrado

            return cara;
        }
    }
}