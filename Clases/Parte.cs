using OpenTK.Mathematics;
using Opeten_Minecraf.Clases;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Opeten_Minecraf.Clases
{
    /// <summary>
    /// Gestiona todas las partes/caras que componen un objeto 3D
    /// </summary>
    public class Partes
    {
        public List<Poligono> Caras { get; private set; }

        public Partes()
        {
            Caras = new List<Poligono>();
        }

        /// <summary>
        /// Agrega una cara/polígono a la colección
        /// </summary>
        public void AgregarCara(Poligono cara)
        {
            Caras.Add(cara);
        }

        /// <summary>
        /// Crea todas las 6 caras de un cubo
        /// </summary>
        public static Partes CrearCarasCubo()
        {
            var partes = new Partes();
            uint indiceBased = 0;

            // Coordenadas de textura estándar para cada cara
            var texCoords = new Vector2[]
            {
                new Vector2(0f, 1f), // top left
                new Vector2(1f, 1f), // top right  
                new Vector2(1f, 0f), // bottom right
                new Vector2(0f, 0f)  // bottom left
            };

            // Cara frontal (Z = 0.5f)
            var caraFrontal = Poligono.CrearCaraCuadrada("Frontal",
                new Punto(-0.5f, 0.5f, 0.5f, texCoords[0].X, texCoords[0].Y), // top left
                new Punto(0.5f, 0.5f, 0.5f, texCoords[1].X, texCoords[1].Y), // top right
                new Punto(0.5f, -0.5f, 0.5f, texCoords[2].X, texCoords[2].Y), // bottom right
                new Punto(-0.5f, -0.5f, 0.5f, texCoords[3].X, texCoords[3].Y), // bottom left
                indiceBased);
            partes.AgregarCara(caraFrontal);
            indiceBased += 4;

            // Cara trasera (Z = -0.5f)
            var caraTrasera = Poligono.CrearCaraCuadrada("Trasera",
                new Punto(-0.5f, 0.5f, -0.5f, texCoords[0].X, texCoords[0].Y),
                new Punto(0.5f, 0.5f, -0.5f, texCoords[1].X, texCoords[1].Y),
                new Punto(0.5f, -0.5f, -0.5f, texCoords[2].X, texCoords[2].Y),
                new Punto(-0.5f, -0.5f, -0.5f, texCoords[3].X, texCoords[3].Y),
                indiceBased);
            partes.AgregarCara(caraTrasera);
            indiceBased += 4;

            // Cara izquierda (X = -0.5f)
            var caraIzquierda = Poligono.CrearCaraCuadrada("Izquierda",
                new Punto(-0.5f, 0.5f, -0.5f, texCoords[0].X, texCoords[0].Y),
                new Punto(-0.5f, 0.5f, 0.5f, texCoords[1].X, texCoords[1].Y),
                new Punto(-0.5f, -0.5f, 0.5f, texCoords[2].X, texCoords[2].Y),
                new Punto(-0.5f, -0.5f, -0.5f, texCoords[3].X, texCoords[3].Y),
                indiceBased);
            partes.AgregarCara(caraIzquierda);
            indiceBased += 4;

            // Cara derecha (X = 0.5f)
            var caraDerecha = Poligono.CrearCaraCuadrada("Derecha",
                new Punto(0.5f, 0.5f, 0.5f, texCoords[0].X, texCoords[0].Y),
                new Punto(0.5f, 0.5f, -0.5f, texCoords[1].X, texCoords[1].Y),
                new Punto(0.5f, -0.5f, -0.5f, texCoords[2].X, texCoords[2].Y),
                new Punto(0.5f, -0.5f, 0.5f, texCoords[3].X, texCoords[3].Y),
                indiceBased);
            partes.AgregarCara(caraDerecha);
            indiceBased += 4;

            // Cara superior (Y = 0.5f)
            var caraSuperior = Poligono.CrearCaraCuadrada("Superior",
                new Punto(-0.5f, 0.5f, -0.5f, texCoords[0].X, texCoords[0].Y),
                new Punto(0.5f, 0.5f, -0.5f, texCoords[1].X, texCoords[1].Y),
                new Punto(0.5f, 0.5f, 0.5f, texCoords[2].X, texCoords[2].Y),
                new Punto(-0.5f, 0.5f, 0.5f, texCoords[3].X, texCoords[3].Y),
                indiceBased);
            partes.AgregarCara(caraSuperior);
            indiceBased += 4;

            // Cara inferior (Y = -0.5f)
            var caraInferior = Poligono.CrearCaraCuadrada("Inferior",
                new Punto(-0.5f, -0.5f, 0.5f, texCoords[0].X, texCoords[0].Y),
                new Punto(0.5f, -0.5f, 0.5f, texCoords[1].X, texCoords[1].Y),
                new Punto(0.5f, -0.5f, -0.5f, texCoords[2].X, texCoords[2].Y),
                new Punto(-0.5f, -0.5f, -0.5f, texCoords[3].X, texCoords[3].Y),
                indiceBased);
            partes.AgregarCara(caraInferior);

            return partes;
        }

        /// <summary>
        /// Obtiene todos los vértices de todas las caras
        /// </summary>
        public List<Vector3> ObtenerTodosLosVertices()
        {
            var vertices = new List<Vector3>();
            foreach (var cara in Caras)
            {
                foreach (var punto in cara.Puntos)
                {
                    vertices.Add(punto.Posicion);
                }
            }
            return vertices;
        }

        /// <summary>
        /// Obtiene todas las coordenadas de textura de todas las caras
        /// </summary>
        public List<Vector2> ObtenerTodasLasCoordenadasTextura()
        {
            var coordenadas = new List<Vector2>();
            foreach (var cara in Caras)
            {
                foreach (var punto in cara.Puntos)
                {
                    coordenadas.Add(punto.CoordenadaTextura);
                }
            }
            return coordenadas;
        }

        /// <summary>
        /// Obtiene todos los índices de todas las caras
        /// </summary>
        public List<uint> ObtenerTodosLosIndices()
        {
            var indices = new List<uint>();
            foreach (var cara in Caras)
            {
                indices.AddRange(cara.Indices);
            }
            return indices;
        }
    }
}