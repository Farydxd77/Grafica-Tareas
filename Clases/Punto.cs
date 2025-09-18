using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK; // OpenTK 3.x usa OpenTK directamente


namespace Opentk_2222.Clases
{
    public class Punto
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Punto(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public static Punto CalcularCentroMasa(List<Punto> puntos)
        {
            if (puntos == null || puntos.Count == 0)
                return new Punto(0, 0, 0);

            float totalX = puntos.Sum(p => p.X);
            float totalY = puntos.Sum(p => p.Y);
            float totalZ = puntos.Sum(p => p.Z);

            return new Punto(
                totalX / puntos.Count,
                totalY / puntos.Count,
                totalZ / puntos.Count
            );
        }

        public override string ToString()
        {
            return $"({X:F2}, {Y:F2}, {Z:F2})";
        }
    }
}