using Opeten_Minecraf.Clases;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace Opeten_Minecraf.Clases
{
    /// <summary>
    /// Representa una cara/polígono de un cubo, compuesta por puntos e índices
    /// </summary>
    public class Poligono
    {
        public List<Punto> Puntos { get; private set; }
        public List<uint> Indices { get; private set; }
        public string Nombre { get; set; }

        public Poligono(string nombre)
        {
            Nombre = nombre;
            Puntos = new List<Punto>();
            Indices = new List<uint>();
        }

        /// <summary>
        /// Agrega un punto al polígono
        /// </summary>
        public void AgregarPunto(Punto punto)
        {
            Puntos.Add(punto);
        }

        /// <summary>
        /// Agrega índices para formar triángulos
        /// </summary>
        public void AgregarIndices(params uint[] indices)
        {
            foreach (uint indice in indices)
            {
                Indices.Add(indice);
            }
        }

        /// <summary>
        /// Crea una cara cuadrada con 4 puntos y 2 triángulos
        /// </summary>
        public static Poligono CrearCaraCuadrada(string nombre,
            Punto puntoTopLeft, Punto puntoTopRight,
            Punto puntoBottomRight, Punto puntoBottomLeft,
            uint baseIndex)
        {
            var poligono = new Poligono(nombre);

            // Agregar los 4 puntos
            poligono.AgregarPunto(puntoTopLeft);
            poligono.AgregarPunto(puntoTopRight);
            poligono.AgregarPunto(puntoBottomRight);
            poligono.AgregarPunto(puntoBottomLeft);

            // Agregar índices para 2 triángulos
            // Triángulo 1
            poligono.AgregarIndices(baseIndex + 0, baseIndex + 1, baseIndex + 2);
            // Triángulo 2
            poligono.AgregarIndices(baseIndex + 2, baseIndex + 3, baseIndex + 0);

            return poligono;
        }
    }
}