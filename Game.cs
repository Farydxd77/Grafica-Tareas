// ===================== Game.cs COMPLETO PARA OPENTK 3.1.0 CON SERIALIZACIÓN =====================
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Opentk_2222.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
// O puede ser:

namespace Opentk_2222
{
    internal class Game : GameWindow
    {
        // Variables del juego
        private Escenario escenario;
        private Dictionary<string, ObjectRenderData> renderObjects;
        private int shaderProgram;
        private Vector3 cameraPos = new Vector3(3f, 2f, 4f);
        private Vector3 cameraTarget = new Vector3(0f, -0.5f, 0f);
        private Vector3 lightPos = new Vector3(2f, 4f, 2f);
        private float rotationTime;

        // VARIABLES PARA TRANSFORMACIONES
        private int objetoSeleccionado = 0;
        private Vector3 rotacionManual = Vector3.Zero;
        private Vector3 translacionManual = Vector3.Zero;
        private Vector3 escalaManual = Vector3.One;
        private bool reflectionX = false, reflectionY = false, reflectionZ = false;
        private bool modoTransformacion = false;
        private float velocidadTransformacion = 2.0f;

        // Control de teclas para evitar repetición
        private bool tabPressed = false;
        private bool xPressed = false, yPressed = false, zPressed = false;
        private bool spacePressed = false;
        private bool[] numberPressed = new bool[10];

        // NUEVAS VARIABLES PARA SERIALIZACIÓN
        private bool f5Pressed = false;  // Control para F5 (Guardar)
        private bool f9Pressed = false;  // Control para F9 (Cargar)

        public struct ParteInfo
        {
            public string Nombre;
            public Vector3 Posicion;
            public Vector3 Tamaño;
            public Vector3 Color;

            public ParteInfo(string nombre, Vector3 posicion, Vector3 tamaño, Vector3 color)
            {
                Nombre = nombre;
                Posicion = posicion;
                Tamaño = tamaño;
                Color = color;
            }
        }

        // Constructor OpenTK 3.1.0
        public Game() : base(1024, 768, GraphicsMode.Default, "Setup de Computadora 3D - OpenTK 3.1.0")
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.ClearColor(0.95f, 0.95f, 0.95f, 1.0f);

            InicializarShaders();
            CrearEscenaSimple();
            PrepararBuffersRenderizado();

            MostrarControles();
        }

        private void MostrarControles()
        {
            Console.WriteLine("=== CONTROLES OPENTK 3.1.0 ===");
            Console.WriteLine("Tab: Cambiar modo (Cámara/Objetos)");
            Console.WriteLine("1-4: Seleccionar objeto (0=Todos)");
            Console.WriteLine("F5: Guardar escena | F9: Cargar última escena");
            Console.WriteLine("");
            Console.WriteLine("MODO CÁMARA:");
            Console.WriteLine("  WASD: Mover | QE: Subir/Bajar | Flechas: Rotar");
            Console.WriteLine("");
            Console.WriteLine("MODO OBJETOS:");
            Console.WriteLine("  WASD: Translation | RF: Rotar Y | TG: Rotar X");
            Console.WriteLine("  +/-: Scale | XYZ: Reflection | Space: Reset");
            Console.WriteLine("===============================");
        }

        private void CrearEscenaSimple()
        {
            escenario = new Escenario("Setup de Computadora");
            renderObjects = new Dictionary<string, ObjectRenderData>();

            var escritorio = CrearObjeto("Escritorio",
                new Vector3(0f, -1.5f, 0f),
                new Vector3(0.4f, 0.3f, 0.2f),
                new ParteInfo("Superficie", Vector3.Zero, new Vector3(5.0f, 0.1f, 3.0f), new Vector3(0.4f, 0.3f, 0.2f))
            );

            var monitor = CrearObjeto("Monitor",
                new Vector3(0f, -0.3f, -0.3f),
                new Vector3(0.15f, 0.15f, 0.15f),
                new ParteInfo("Pantalla", Vector3.Zero, new Vector3(2.4f, 1.5f, 0.06f), new Vector3(0.02f, 0.02f, 0.05f)),
                new ParteInfo("Base", new Vector3(0f, -0.8f, 0f), new Vector3(0.6f, 0.2f, 0.4f), new Vector3(0.2f, 0.2f, 0.2f))
            );

            var cpu = CrearObjeto("CPU",
                new Vector3(1.8f, -0.8f, -0.2f),
                new Vector3(0.25f, 0.25f, 0.3f),
                new ParteInfo("Carcasa", Vector3.Zero, new Vector3(0.5f, 1.6f, 0.8f), new Vector3(0.2f, 0.2f, 0.25f)),
                new ParteInfo("BotonEncendido", new Vector3(-0.15f, -0.5f, 0.43f), new Vector3(0.05f, 0.05f, 0.01f), new Vector3(0.8f, 0.2f, 0.2f))
            );

            var teclado = CrearObjeto("Teclado",
                new Vector3(0f, -1.4f, 0.8f),
                new Vector3(0.05f, 0.05f, 0.05f),
                new ParteInfo("BaseTeclado", Vector3.Zero, new Vector3(2.8f, 0.08f, 0.8f), new Vector3(0.05f, 0.05f, 0.05f)),
                new ParteInfo("BarraEspaciadora", new Vector3(0f, 0.06f, 0.2f), new Vector3(1.2f, 0.025f, 0.1f), new Vector3(0.15f, 0.15f, 0.15f))
            );

            escenario.AgregarObjetos(escritorio, monitor, cpu, teclado);
        }

        private Objeto CrearObjeto(string nombre, Vector3 posicion, Vector3 colorBase, params ParteInfo[] partes)
        {
            var objeto = new Objeto(nombre);
            objeto.Posicion = posicion;
            objeto.ColorBase = colorBase;

            foreach (var info in partes)
            {
                var parte = Parte.CrearCubo(info.Nombre, info.Posicion, info.Tamaño, info.Color);
                objeto.AgregarParte(parte);
            }

            return objeto;
        }

        private void InicializarShaders()
        {
            string vertexShader = @"#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat3 normalMatrix;

out vec3 FragPos;
out vec3 Normal;

void main()
{
    FragPos = vec3(model * vec4(aPosition, 1.0));
    Normal = normalMatrix * aNormal;
    gl_Position = projection * view * vec4(FragPos, 1.0);
}";

            string fragmentShader = @"#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;

uniform vec3 objectColor;
uniform vec3 lightPos;
uniform vec3 lightColor;
uniform vec3 viewPos;

void main()
{
    vec3 ambient = 0.4 * vec3(1.0, 1.0, 1.0);
    
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * vec3(1.0, 1.0, 1.0);
    
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 64.0);
    vec3 specular = 0.6 * spec * vec3(1.0, 1.0, 1.0);
    
    vec3 result = (ambient + diffuse + specular) * objectColor;
    FragColor = vec4(result, 1.0);
}";

            shaderProgram = CrearProgramaShader(vertexShader, fragmentShader);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            rotationTime += (float)e.Time * 0.3f;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(shaderProgram);

            Matrix4 view = Matrix4.LookAt(cameraPos, cameraTarget, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f), (float)Width / Height, 0.1f, 100f);

            SetUniform("view", view);
            SetUniform("projection", projection);
            SetUniform("lightPos", lightPos);
            SetUniform("lightColor", Vector3.One);
            SetUniform("viewPos", cameraPos);

            int objetoIndex = 0;
            foreach (var kvp in renderObjects)
            {
                objetoIndex++;
                var objData = kvp.Value;

                bool aplicarTransformaciones = (objetoSeleccionado == 0 || objetoSeleccionado == objetoIndex);

                Matrix4 model = Matrix4.Identity;

                if (aplicarTransformaciones)
                {
                    // REFLECTION
                    Matrix4 reflection = Matrix4.Identity;
                    if (reflectionX) reflection *= Matrix4.CreateScale(-1, 1, 1);
                    if (reflectionY) reflection *= Matrix4.CreateScale(1, -1, 1);
                    if (reflectionZ) reflection *= Matrix4.CreateScale(1, 1, -1);

                    // SCALE
                    Matrix4 scale = Matrix4.CreateScale(escalaManual);

                    // ROTATION
                    Matrix4 rotationX = Matrix4.CreateRotationX(rotacionManual.X);
                    Matrix4 rotationY = Matrix4.CreateRotationY(rotacionManual.Y + rotationTime * 0.2f);
                    Matrix4 rotationZ = Matrix4.CreateRotationZ(rotacionManual.Z);

                    // TRANSLATION
                    Matrix4 translation = Matrix4.CreateTranslation(objData.Position + translacionManual);

                    model = reflection * scale * rotationZ * rotationY * rotationX * translation;
                }
                else
                {
                    model = Matrix4.CreateRotationY(rotationTime * 0.2f) * Matrix4.CreateTranslation(objData.Position);
                }

                Matrix3 normalMatrix = new Matrix3(Matrix4.Transpose(Matrix4.Invert(model)));

                SetUniform("model", model);
                SetUniform("normalMatrix", normalMatrix);

                Vector3 color = objData.BaseColor;
                if (aplicarTransformaciones && objetoSeleccionado != 0)
                {
                    color = color * 1.2f;
                }
                SetUniform("objectColor", color);

                GL.BindVertexArray(objData.VAO);
                GL.DrawElements(PrimitiveType.Triangles, objData.IndexCount, DrawElementsType.UnsignedInt, 0);
            }

            SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var keyboard = Keyboard.GetState();
            float deltaTime = (float)e.Time;
            float speed = velocidadTransformacion * deltaTime;

            // CAMBIAR MODO con control de press
            if (keyboard.IsKeyDown(Key.Tab) && !tabPressed)
            {
                modoTransformacion = !modoTransformacion;
                Console.WriteLine($"Modo: {(modoTransformacion ? "OBJETOS" : "CÁMARA")}");
                tabPressed = true;
            }
            else if (!keyboard.IsKeyDown(Key.Tab))
            {
                tabPressed = false;
            }

            // SELECCIÓN DE OBJETOS con control de press
            for (int i = 0; i <= 4; i++)
            {
                Key numKey = (Key)((int)Key.Number0 + i);
                if (keyboard.IsKeyDown(numKey) && !numberPressed[i])
                {
                    objetoSeleccionado = i;
                    string[] nombres = { "Todos los objetos", "Escritorio", "Monitor", "CPU", "Teclado" };
                    Console.WriteLine($"{nombres[i]} seleccionado");
                    numberPressed[i] = true;
                }
                else if (!keyboard.IsKeyDown(numKey))
                {
                    numberPressed[i] = false;
                }
            }

            // SERIALIZACIÓN - GUARDAR ESCENA (F5)
            if (keyboard.IsKeyDown(Key.F5) && !f5Pressed)
            {
                GuardarEscena();
                f5Pressed = true;
            }
            else if (!keyboard.IsKeyDown(Key.F5))
            {
                f5Pressed = false;
            }

            // SERIALIZACIÓN - CARGAR ESCENA (F9)
            if (keyboard.IsKeyDown(Key.F9) && !f9Pressed)
            {
                CargarEscena();
                f9Pressed = true;
            }
            else if (!keyboard.IsKeyDown(Key.F9))
            {
                f9Pressed = false;
            }

            if (modoTransformacion)
            {
                ProcesarTransformacionObjetos(keyboard, speed);
            }
            else
            {
                ProcesarInputCamara(keyboard, deltaTime);
            }

            if (keyboard.IsKeyDown(Key.Escape)) Exit();

            base.OnUpdateFrame(e);
        }

        private void ProcesarTransformacionObjetos(KeyboardState keyboard, float speed)
        {
            // TRANSLATION
            if (keyboard.IsKeyDown(Key.W)) translacionManual += Vector3.UnitZ * -speed;
            if (keyboard.IsKeyDown(Key.S)) translacionManual += Vector3.UnitZ * speed;
            if (keyboard.IsKeyDown(Key.A)) translacionManual += Vector3.UnitX * -speed;
            if (keyboard.IsKeyDown(Key.D)) translacionManual += Vector3.UnitX * speed;
            if (keyboard.IsKeyDown(Key.Q)) translacionManual += Vector3.UnitY * speed;
            if (keyboard.IsKeyDown(Key.E)) translacionManual += Vector3.UnitY * -speed;

            // ROTATION
            if (keyboard.IsKeyDown(Key.R)) rotacionManual.Y += speed;
            if (keyboard.IsKeyDown(Key.F)) rotacionManual.Y -= speed;
            if (keyboard.IsKeyDown(Key.T)) rotacionManual.X += speed;
            if (keyboard.IsKeyDown(Key.G)) rotacionManual.X -= speed;

            // SCALE
            if (keyboard.IsKeyDown(Key.Plus)) escalaManual *= 1.02f;
            if (keyboard.IsKeyDown(Key.Minus)) escalaManual *= 0.98f;

            // REFLECTION con control de press
            if (keyboard.IsKeyDown(Key.X) && !xPressed)
            {
                reflectionX = !reflectionX;
                Console.WriteLine($"Reflection X: {(reflectionX ? "ON" : "OFF")}");
                xPressed = true;
            }
            else if (!keyboard.IsKeyDown(Key.X))
            {
                xPressed = false;
            }

            if (keyboard.IsKeyDown(Key.Y) && !yPressed)
            {
                reflectionY = !reflectionY;
                Console.WriteLine($"Reflection Y: {(reflectionY ? "ON" : "OFF")}");
                yPressed = true;
            }
            else if (!keyboard.IsKeyDown(Key.Y))
            {
                yPressed = false;
            }

            if (keyboard.IsKeyDown(Key.Z) && !zPressed)
            {
                reflectionZ = !reflectionZ;
                Console.WriteLine($"Reflection Z: {(reflectionZ ? "ON" : "OFF")}");
                zPressed = true;
            }
            else if (!keyboard.IsKeyDown(Key.Z))
            {
                zPressed = false;
            }

            // RESET
            if (keyboard.IsKeyDown(Key.Space) && !spacePressed)
            {
                rotacionManual = Vector3.Zero;
                translacionManual = Vector3.Zero;
                escalaManual = Vector3.One;
                reflectionX = reflectionY = reflectionZ = false;
                Console.WriteLine("Transformaciones reseteadas");
                spacePressed = true;
            }
            else if (!keyboard.IsKeyDown(Key.Space))
            {
                spacePressed = false;
            }
        }

        private void ProcesarInputCamara(KeyboardState keyboard, float deltaTime)
        {
            float speed = 3.0f * deltaTime;
            Vector3 forward = Vector3.Normalize(cameraTarget - cameraPos);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));

            if (keyboard.IsKeyDown(Key.W)) cameraPos += forward * speed;
            if (keyboard.IsKeyDown(Key.S)) cameraPos -= forward * speed;
            if (keyboard.IsKeyDown(Key.A)) cameraPos -= right * speed;
            if (keyboard.IsKeyDown(Key.D)) cameraPos += right * speed;
            if (keyboard.IsKeyDown(Key.Q)) cameraPos += Vector3.UnitY * speed;
            if (keyboard.IsKeyDown(Key.E)) cameraPos -= Vector3.UnitY * speed;

            if (keyboard.IsKeyDown(Key.Left))
            {
                float angleY = 1.5f * deltaTime;
                Vector3 toCamera = cameraPos - cameraTarget;

                float cosY = (float)Math.Cos(angleY);
                float sinY = (float)Math.Sin(angleY);

                Vector3 newToCamera = new Vector3(
                    toCamera.X * cosY + toCamera.Z * sinY,
                    toCamera.Y,
                    -toCamera.X * sinY + toCamera.Z * cosY
                );

                cameraPos = cameraTarget + newToCamera;
            }

            if (keyboard.IsKeyDown(Key.Right))
            {
                float angleY = -1.5f * deltaTime;
                Vector3 toCamera = cameraPos - cameraTarget;

                float cosY = (float)Math.Cos(angleY);
                float sinY = (float)Math.Sin(angleY);

                Vector3 newToCamera = new Vector3(
                    toCamera.X * cosY + toCamera.Z * sinY,
                    toCamera.Y,
                    -toCamera.X * sinY + toCamera.Z * cosY
                );

                cameraPos = cameraTarget + newToCamera;
            }
        }

        // MÉTODOS DE SERIALIZACIÓN
        private void GuardarEscena()
        {
            try
            {
                Console.WriteLine("=== GUARDANDO ESCENA ===");

                var escenaData = new EscenaData(escenario, cameraPos, cameraTarget, lightPos);

                escenaData.TransformacionActual = new TransformacionData
                {
                    ObjetoSeleccionado = objetoSeleccionado,
                    RotacionManual = rotacionManual,
                    TranslacionManual = translacionManual,
                    EscalaManual = escalaManual,
                    ReflectionX = reflectionX,
                    ReflectionY = reflectionY,
                    ReflectionZ = reflectionZ
                };

                string nombreArchivo = $"escena_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                Serializar.GuardarComoJson(escenaData, nombreArchivo);

                Console.WriteLine($"Escena guardada: {nombreArchivo}");
                Console.WriteLine($"Carpeta: EscenasGuardadas/");
                Console.WriteLine("===========================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando escena: {ex.Message}");
            }
        }

        private void CargarEscena()
        {
            try
            {
                Console.WriteLine("=== CARGANDO ESCENA ===");

                var archivos = Serializar.ListarEscenasGuardadas();
                if (archivos.Count == 0)
                {
                    Console.WriteLine("No hay escenas guardadas");
                    Console.WriteLine("Presiona F5 para guardar la escena actual");
                    return;
                }

                Console.WriteLine($"Encontradas {archivos.Count} escenas:");
                for (int i = 0; i < archivos.Count && i < 5; i++)
                {
                    Console.WriteLine($"  {i + 1}. {archivos[i]}");
                }

                string ultimaEscena = archivos.OrderByDescending(f => f).First();
                var escenaData = Serializar.CargarDesdeJson<EscenaData>(ultimaEscena);

                if (escenaData != null)
                {
                    cameraPos = escenaData.PosicionCamara;
                    cameraTarget = escenaData.ObjetivoCamara;
                    lightPos = escenaData.PosicionLuz;

                    if (escenaData.TransformacionActual != null)
                    {
                        objetoSeleccionado = escenaData.TransformacionActual.ObjetoSeleccionado;
                        rotacionManual = escenaData.TransformacionActual.RotacionManual;
                        translacionManual = escenaData.TransformacionActual.TranslacionManual;
                        escalaManual = escenaData.TransformacionActual.EscalaManual;
                        reflectionX = escenaData.TransformacionActual.ReflectionX;
                        reflectionY = escenaData.TransformacionActual.ReflectionY;
                        reflectionZ = escenaData.TransformacionActual.ReflectionZ;
                    }

                    Console.WriteLine($"Escena cargada: {ultimaEscena}");
                    Console.WriteLine($"Cámara restaurada a: {cameraPos}");
                    Console.WriteLine($"Luz restaurada a: {lightPos}");
                    Console.WriteLine("========================");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando escena: {ex.Message}");
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnUnload(EventArgs e)
        {
            foreach (var objData in renderObjects.Values)
            {
                GL.DeleteVertexArray(objData.VAO);
                GL.DeleteBuffer(objData.VBO);
                GL.DeleteBuffer(objData.EBO);
            }

            GL.DeleteProgram(shaderProgram);
            base.OnUnload(e);
        }

        private void PrepararBuffersRenderizado()
        {
            foreach (var objeto in escenario.Objetos)
            {
                var vertices = objeto.ObtenerVerticesParaRender();
                var indices = objeto.ObtenerIndicesParaRender();

                int vao = GL.GenVertexArray();
                int vbo = GL.GenBuffer();
                int ebo = GL.GenBuffer();

                GL.BindVertexArray(vao);

                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float),
                             vertices.ToArray(), BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint),
                             indices.ToArray(), BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);

                GL.BindVertexArray(0);

                renderObjects[objeto.Nombre] = new ObjectRenderData
                {
                    VAO = vao,
                    VBO = vbo,
                    EBO = ebo,
                    IndexCount = indices.Count,
                    BaseColor = objeto.ColorBase,
                    Position = objeto.Posicion,
                    Rotation = objeto.Rotacion,
                    Scale = objeto.Escala
                };
            }
        }

        private int CrearProgramaShader(string vertexSource, string fragmentSource)
        {
            int vertexShader = CompileShader(vertexSource, ShaderType.VertexShader);
            int fragmentShader = CompileShader(fragmentSource, ShaderType.FragmentShader);

            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                throw new Exception($"Error linking shader program: {infoLog}");
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return program;
        }

        private int CompileShader(string source, ShaderType type)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error compiling {type} shader: {infoLog}");
            }

            return shader;
        }

        private void SetUniform(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(shaderProgram, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        private void SetUniform(string name, Matrix3 matrix)
        {
            int location = GL.GetUniformLocation(shaderProgram, name);
            GL.UniformMatrix3(location, false, ref matrix);
        }

        private void SetUniform(string name, Vector3 vector)
        {
            int location = GL.GetUniformLocation(shaderProgram, name);
            GL.Uniform3(location, vector);
        }

        public struct ObjectRenderData
        {
            public int VAO;
            public int VBO;
            public int EBO;
            public int IndexCount;
            public Vector3 BaseColor;
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;
        }
    }
}