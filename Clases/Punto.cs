using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Mathematics;

namespace Opeten_Minecraf.Clases
{
    /// <summary>
    /// Representa un punto en el espacio 3D con coordenadas de textura
    /// </summary>
    public class Punto
    {
        public Vector3 Posicion { get; set; }
        public Vector2 CoordenadaTextura { get; set; }

        public Punto(Vector3 posicion, Vector2 coordenadaTextura)
        {
            Posicion = posicion;
            CoordenadaTextura = coordenadaTextura;
        }

        public Punto(float x, float y, float z, float u, float v)
        {
            Posicion = new Vector3(x, y, z);
            CoordenadaTextura = new Vector2(u, v);
        }
    }
}