using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Opeten_Minecraf.Clases
{
    /// <summary>
    /// Representa un objeto 3D completo (como un cubo) con todas sus partes
    /// </summary>
    public class Objeto
    {
        public Partes Partes { get; private set; }
        public Vector3 Posicion { get; set; }
        public Vector3 Rotacion { get; set; }
        public Vector3 Escala { get; set; }
        public string Nombre { get; set; }

        public Objeto(string nombre)
        {
            Nombre = nombre;
            Partes = new Partes();
            Posicion = Vector3.Zero;
            Rotacion = Vector3.Zero;
            Escala = Vector3.One;
        }

        /// <summary>
        /// Crea un cubo completo con todas sus caras
        /// </summary>
        public static Objeto CrearCubo(string nombre)
        {
            var cubo = new Objeto(nombre);
            cubo.Partes = Partes.CrearCarasCubo();
            return cubo;
        }

        /// <summary>
        /// Obtiene la matriz de transformación del objeto
        /// </summary>
        public Matrix4 ObtenerMatrizTransformacion()
        {
            Matrix4 translation = Matrix4.CreateTranslation(Posicion);
            Matrix4 rotationX = Matrix4.CreateRotationX(Rotacion.X);
            Matrix4 rotationY = Matrix4.CreateRotationY(Rotacion.Y);
            Matrix4 rotationZ = Matrix4.CreateRotationZ(Rotacion.Z);
            Matrix4 scale = Matrix4.CreateScale(Escala);

            // Orden: Escala -> Rotación -> Traslación
            return scale * rotationX * rotationY * rotationZ * translation;
        }

        /// <summary>
        /// Obtiene todos los vértices del objeto
        /// </summary>
        public List<Vector3> ObtenerVertices()
        {
            return Partes.ObtenerTodosLosVertices();
        }

        /// <summary>
        /// Obtiene todas las coordenadas de textura del objeto
        /// </summary>
        public List<Vector2> ObtenerCoordenadasTextura()
        {
            return Partes.ObtenerTodasLasCoordenadasTextura();
        }

        /// <summary>
        /// Obtiene todos los índices del objeto
        /// </summary>
        public List<uint> ObtenerIndices()
        {
            return Partes.ObtenerTodosLosIndices();
        }

        /// <summary>
        /// Rota el objeto en el eje Y
        /// </summary>
        public void RotarY(float angulo)
        {
            Rotacion = new Vector3(Rotacion.X, Rotacion.Y + angulo, Rotacion.Z);
        }

        /// <summary>
        /// Mueve el objeto a una nueva posición
        /// </summary>
        public void Mover(Vector3 nuevaPosicion)
        {
            Posicion = nuevaPosicion;
        }

        /// <summary>
        /// Cambia la escala del objeto
        /// </summary>
        public void Escalar(Vector3 nuevaEscala)
        {
            Escala = nuevaEscala;
        }
    }
}