using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Opeten_Minecraf.Clases;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opeten_Minecraf
{
    internal class Game : GameWindow
    {
        // El objeto cubo que renderizaremos
        private Objeto cubo;

        // Variables del pipeline de renderizado
        int vao;
        int shaderProgram;
        int vbo;
        int textureVBO;
        int ebo;
        int textureID;

        // Variables de transformación
        float yRot = 0f;

        // Ancho y alto de la pantalla
        int width, height;

        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            // Centrar la ventana en el monitor
            this.width = width;
            this.height = height;
            this.CenterWindow(new Vector2i(width, height));
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Crear nuestro cubo usando el sistema de clases
            cubo = Objeto.CrearCubo("MiCubo");
            cubo.Mover(new Vector3(0f, 0f, -3f));

            // Obtener los datos del cubo
            var vertices = cubo.ObtenerVertices();
            var textCoords = cubo.ObtenerCoordenadasTextura();
            var indices = cubo.ObtenerIndices().ToArray();

            // Configurar VAO
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            // ----- VBO de Vértices -----
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(vao, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // ----- VBO de Texturas -----
            textureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, textCoords.Count * Vector2.SizeInBytes, textCoords.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(vao, 1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Desvincular VAO
            GL.BindVertexArray(0);

            // ----- EBO (Element Buffer Object) -----
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            // ----- Crear Programa de Shaders -----
            shaderProgram = GL.CreateProgram();

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource("Default.vert"));
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, LoadShaderSource("Default.frag"));
            GL.CompileShader(fragmentShader);

            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);

            // Eliminar shaders después de linkear
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // ----- Configurar Texturas -----
            textureID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // Parámetros de textura
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Cargar imagen
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult dirtTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/xd.png"), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, dirtTexture.Width, dirtTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dirtTexture.Data);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(0.6f, 0.3f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Usar el programa de shaders
            GL.UseProgram(shaderProgram);
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // Actualizar rotación del cubo
            cubo.RotarY(0.0001f);

            // Matrices de transformación
            Matrix4 model = cubo.ObtenerMatrizTransformacion();
            Matrix4 view = Matrix4.Identity;
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60.0f),
                (float)width / height,
                0.1f,
                100.0f);

            // Enviar matrices a los shaders
            int modelLocation = GL.GetUniformLocation(shaderProgram, "model");
            int viewLocation = GL.GetUniformLocation(shaderProgram, "view");
            int projectionLocation = GL.GetUniformLocation(shaderProgram, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            // Renderizar el cubo
            var indices = cubo.ObtenerIndices();
            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteVertexArray(vao);
            GL.DeleteProgram(shaderProgram);
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(textureVBO);
            GL.DeleteTexture(textureID);
            GL.DeleteBuffer(ebo);
        }

        public static string LoadShaderSource(string filePath)
        {
            string shaderSource = "";

            try
            {
                using (StreamReader reader = new StreamReader("../../../Shaders/" + filePath))
                {
                    shaderSource = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load shader source file: " + e.Message);
            }
            return shaderSource;
        }
    }
}