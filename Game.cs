// ===================== Game.cs CON TRANSFORMACIONES DE PARTES - CÓDIGO LIMPIO =====================
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Opentk_2222.Clases;
namespace Opentk_2222
{
    internal class Game : GameWindow
    {
        #region Constantes
        private const float VELOCIDAD_CAMARA = 3.0f;
        private const float VELOCIDAD_TRANSFORMACION = 2.0f;
        private const float VELOCIDAD_ROTACION_AUTO = 0.2f;
        private const int VERTICES_POR_NORMAL = 6;
        private const float FOV_GRADOS = 45f;
        private const float NEAR_PLANE = 0.1f;
        private const float FAR_PLANE = 100f;
        #endregion

        #region Variables del Sistema
        private Escenario escenario;
        private Dictionary<string, List<ParteRenderData>> renderObjects;
        private int shaderProgram;
        #endregion

        #region Variables de Cámara y Luz
        private Vector3 cameraPos = new Vector3(3f, 2f, 4f);
        private Vector3 cameraTarget = new Vector3(0f, -0.5f, 0f);
        private Vector3 lightPos = new Vector3(2f, 4f, 2f);
        private float rotationTime;
        #endregion

        #region Variables de Transformaciones - MEJORADAS
        private NivelSeleccion nivelActual = NivelSeleccion.Escenario;
        private int objetoSeleccionado = 0;
        private int parteSeleccionada = 0;

        private Vector3 rotacionManual = Vector3.Zero;
        private Vector3 translacionManual = Vector3.Zero;
        private Vector3 escalaManual = Vector3.One;
        private bool reflectionX = false, reflectionY = false, reflectionZ = false;

        private bool modoTransformacion = false;
        #endregion

        #region Control de Input
        private Dictionary<Key, bool> keyStates = new Dictionary<Key, bool>();
        #endregion

        #region Enums y Structs
        public enum NivelSeleccion
        {
            Escenario = 0,    // Todos los objetos
            Objeto = 1,       // Objeto específico
            Parte = 2         // Parte específica
        }

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

        public struct ParteRenderData
        {
            public int VAO;
            public int VBO;
            public int EBO;
            public int IndexCount;
            public Vector3 BaseColor;
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;
            public string NombreParte;
            public string NombreObjeto;
        }
        #endregion

        // Constructor
        public Game() : base(1024, 768, GraphicsMode.Default, "Setup de Computadora 3D - OpenTK 3.1.0 + Partes")
        {
        }

        #region Inicialización
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InicializarSistema();
        }

        private void InicializarSistema()
        {
            ConfigurarOpenGL();
            InicializarShaders();
            CrearEscenaSimple();
            PrepararBuffersRenderizado();
            MostrarControles();
        }

        private void ConfigurarOpenGL()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.ClearColor(0.95f, 0.95f, 0.95f, 1.0f);
        }

        private void MostrarControles()
        {
            var controles = new[]
            {
                "=== CONTROLES OPENTK 3.1.0 + PARTES ===",
                "Tab: Cambiar modo (Cámara/Objetos)",
                "C: Cambiar nivel (Escenario → Objeto → Parte)",
                "1-4: Seleccionar objeto | Q/E: Cambiar parte",
                "F5: Guardar escena | F9: Cargar última escena",
                "",
                "MODO CÁMARA:",
                "  WASD: Mover | QE: Subir/Bajar | Flechas: Rotar",
                "",
                "MODO OBJETOS/PARTES:",
                "  WASD: Translation | RF: Rotar Y | TG: Rotar X",
                "  +/-: Scale | XYZ: Reflection | Space: Reset",
                "============================================="
            };

            foreach (var linea in controles)
                Console.WriteLine(linea);
        }
        #endregion

        #region Creación de Escena
        private void CrearEscenaSimple()
        {
            escenario = new Escenario("Setup de Computadora");
            renderObjects = new Dictionary<string, List<ParteRenderData>>();

            AgregarEscritorio();
            AgregarMonitor();
            AgregarCPU();
            AgregarTeclado();
        }

        private void AgregarEscritorio()
        {
            var escritorio = CrearObjeto("Escritorio",
                new Vector3(0f, -1.5f, 0f),
                new Vector3(0.4f, 0.3f, 0.2f),
                new ParteInfo("Superficie", Vector3.Zero, new Vector3(5.0f, 0.1f, 3.0f), new Vector3(0.4f, 0.3f, 0.2f))
            );
            escenario.AgregarObjeto(escritorio);
        }

        private void AgregarMonitor()
        {
            var monitor = CrearObjeto("Monitor",
                new Vector3(0f, -0.3f, -0.3f),
                new Vector3(0.15f, 0.15f, 0.15f),
                new ParteInfo("Pantalla", Vector3.Zero, new Vector3(2.4f, 1.5f, 0.06f), new Vector3(0.02f, 0.02f, 0.05f)),
                new ParteInfo("Base", new Vector3(0f, -0.8f, 0f), new Vector3(0.6f, 0.2f, 0.4f), new Vector3(0.2f, 0.2f, 0.2f))
            );
            escenario.AgregarObjeto(monitor);
        }

        private void AgregarCPU()
        {
            var cpu = CrearObjeto("CPU",
                new Vector3(1.8f, -0.8f, -0.2f),
                new Vector3(0.25f, 0.25f, 0.3f),
                new ParteInfo("Carcasa", Vector3.Zero, new Vector3(0.5f, 1.6f, 0.8f), new Vector3(0.2f, 0.2f, 0.25f)),
                new ParteInfo("BotonEncendido", new Vector3(-0.15f, -0.5f, 0.43f), new Vector3(0.05f, 0.05f, 0.01f), new Vector3(0.8f, 0.2f, 0.2f))
            );
            escenario.AgregarObjeto(cpu);
        }

        private void AgregarTeclado()
        {
            var teclado = CrearObjeto("Teclado",
                new Vector3(0f, -1.4f, 0.8f),
                new Vector3(0.05f, 0.05f, 0.05f),
                new ParteInfo("BaseTeclado", Vector3.Zero, new Vector3(2.8f, 0.08f, 0.8f), new Vector3(0.05f, 0.05f, 0.05f)),
                new ParteInfo("BarraEspaciadora", new Vector3(0f, 0.06f, 0.2f), new Vector3(1.2f, 0.025f, 0.1f), new Vector3(0.15f, 0.15f, 0.15f))
            );
            escenario.AgregarObjeto(teclado);
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
        #endregion

        #region Renderizado
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            rotationTime += (float)e.Time * VELOCIDAD_ROTACION_AUTO;

            PrepararRender();
            ConfigurarMatricesVista();
            RenderizarObjetos();
            FinalizarRender();
        }

        private void PrepararRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(shaderProgram);
        }

        private void ConfigurarMatricesVista()
        {
            Matrix4 view = Matrix4.LookAt(cameraPos, cameraTarget, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(FOV_GRADOS), (float)Width / Height, NEAR_PLANE, FAR_PLANE);

            SetUniform("view", view);
            SetUniform("projection", projection);
            SetUniform("lightPos", lightPos);
            SetUniform("lightColor", Vector3.One);
            SetUniform("viewPos", cameraPos);
        }

        private void RenderizarObjetos()
        {
            int objetoIndex = 0;

            foreach (var kvp in renderObjects)
            {
                objetoIndex++;
                string nombreObjeto = kvp.Key;
                var partesObjeto = kvp.Value;

                int parteIndex = 0;
                foreach (var parteData in partesObjeto)
                {
                    parteIndex++;

                    bool aplicarTransformaciones = DebeAplicarTransformaciones(objetoIndex, parteIndex);
                    Matrix4 model = CalcularMatrizTransformacion(parteData, aplicarTransformaciones);

                    ConfigurarUniformsParte(model, parteData, aplicarTransformaciones);
                    DibujarParte(parteData);
                }
            }
        }

        private bool DebeAplicarTransformaciones(int objIndex, int parteIndex)
        {
            switch (nivelActual)
            {
                case NivelSeleccion.Escenario:
                    return objetoSeleccionado == 0; // Todos los objetos

                case NivelSeleccion.Objeto:
                    return objetoSeleccionado == 0 || objetoSeleccionado == objIndex;

                case NivelSeleccion.Parte:
                    if (objetoSeleccionado == 0) return true; // Todas las partes de todos los objetos
                    if (objetoSeleccionado != objIndex) return false; // Objeto no seleccionado
                    return parteSeleccionada == 0 || parteSeleccionada == parteIndex; // Parte específica

                default:
                    return false;
            }
        }

        private Matrix4 CalcularMatrizTransformacion(ParteRenderData parteData, bool aplicarTransformaciones)
        {
            Matrix4 model = Matrix4.Identity;

            if (aplicarTransformaciones)
            {
                // REFLECTION
                Matrix4 reflection = Matrix4.Identity;
                if (reflectionX) reflection *= Matrix4.CreateScale(-1, 1, 1);
                if (reflectionY) reflection *= Matrix4.CreateScale(1, -1, 1);
                if (reflectionZ) reflection *= Matrix4.CreateScale(1, 1, -1);

                // SCALE, ROTATION, TRANSLATION
                Matrix4 scale = Matrix4.CreateScale(escalaManual);
                Matrix4 rotationX = Matrix4.CreateRotationX(rotacionManual.X);
                Matrix4 rotationY = Matrix4.CreateRotationY(rotacionManual.Y + rotationTime);
                Matrix4 rotationZ = Matrix4.CreateRotationZ(rotacionManual.Z);
                Matrix4 translation = Matrix4.CreateTranslation(parteData.Position + translacionManual);

                model = reflection * scale * rotationZ * rotationY * rotationX * translation;
            }
            else
            {
                model = Matrix4.CreateRotationY(rotationTime) * Matrix4.CreateTranslation(parteData.Position);
            }

            return model;
        }

        private void ConfigurarUniformsParte(Matrix4 model, ParteRenderData parteData, bool aplicarTransformaciones)
        {
            Matrix3 normalMatrix = new Matrix3(Matrix4.Transpose(Matrix4.Invert(model)));

            SetUniform("model", model);
            SetUniform("normalMatrix", normalMatrix);

            Vector3 color = parteData.BaseColor;
            if (aplicarTransformaciones && (objetoSeleccionado != 0 || parteSeleccionada != 0))
                color = color * 1.3f; // Resaltar parte/objeto seleccionado

            SetUniform("objectColor", color);
        }

        private void DibujarParte(ParteRenderData parteData)
        {
            GL.BindVertexArray(parteData.VAO);
            GL.DrawElements(PrimitiveType.Triangles, parteData.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        private void FinalizarRender()
        {
            SwapBuffers();
            base.OnRenderFrame(null);
        }
        #endregion

        #region Manejo de Input - MEJORADO
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var keyboard = Keyboard.GetState();
            float deltaTime = (float)e.Time;

            ProcesarTeclasEspeciales(keyboard);
            ProcesarCambioNivel(keyboard);
            ProcesarSeleccionObjetos(keyboard);
            ProcesarSeleccionPartes(keyboard);
            ProcesarSerializacion(keyboard);

            if (modoTransformacion)
                ProcesarTransformacionObjetos(keyboard, deltaTime);
            else
                ProcesarInputCamara(keyboard, deltaTime);

            if (keyboard.IsKeyDown(Key.Escape)) Exit();
            base.OnUpdateFrame(e);
        }

        private bool IsKeyJustPressed(Key key)
        {
            var current = Keyboard.GetState().IsKeyDown(key);
            var previous = keyStates.GetValueOrDefault(key, false);
            keyStates[key] = current;
            return current && !previous;
        }

        private void ProcesarTeclasEspeciales(KeyboardState keyboard)
        {
            if (IsKeyJustPressed(Key.Tab))
            {
                modoTransformacion = !modoTransformacion;
                Console.WriteLine($"Modo: {(modoTransformacion ? "TRANSFORMACIONES" : "CÁMARA")}");
            }
        }

        private void ProcesarCambioNivel(KeyboardState keyboard)
        {
            if (IsKeyJustPressed(Key.C))
            {
                nivelActual = (NivelSeleccion)(((int)nivelActual + 1) % 3);
                ResetearTransformaciones();

                string[] niveles = { "ESCENARIO", "OBJETO", "PARTE" };
                Console.WriteLine($"Nivel: {niveles[(int)nivelActual]}");
                MostrarSeleccionActual();
            }
        }

        private void ProcesarSeleccionObjetos(KeyboardState keyboard)
        {
            string[] nombres = { "Todos", "Escritorio", "Monitor", "CPU", "Teclado" };

            for (int i = 0; i <= 4; i++)
            {
                Key numKey = (Key)((int)Key.Number0 + i);
                if (IsKeyJustPressed(numKey))
                {
                    objetoSeleccionado = i;
                    parteSeleccionada = 0; // Reset parte al cambiar objeto
                    Console.WriteLine($"Objeto: {nombres[i]}");
                    MostrarSeleccionActual();
                }
            }
        }

        private void ProcesarSeleccionPartes(KeyboardState keyboard)
        {
            if (nivelActual != NivelSeleccion.Parte) return;

            if (IsKeyJustPressed(Key.Q))
            {
                var partes = ObtenerPartesObjetoSeleccionado();
                if (partes.Count > 0)
                {
                    parteSeleccionada = parteSeleccionada > 0 ? parteSeleccionada - 1 : partes.Count;
                    MostrarSeleccionActual();
                }
            }

            if (IsKeyJustPressed(Key.E))
            {
                var partes = ObtenerPartesObjetoSeleccionado();
                if (partes.Count > 0)
                {
                    parteSeleccionada = parteSeleccionada < partes.Count ? parteSeleccionada + 1 : 0;
                    MostrarSeleccionActual();
                }
            }
        }

        private List<ParteRenderData> ObtenerPartesObjetoSeleccionado()
        {
            if (objetoSeleccionado == 0) return new List<ParteRenderData>();

            string[] nombresObjetos = { "", "Escritorio", "Monitor", "CPU", "Teclado" };
            if (objetoSeleccionado >= nombresObjetos.Length) return new List<ParteRenderData>();

            string nombreObjeto = nombresObjetos[objetoSeleccionado];
            return renderObjects.GetValueOrDefault(nombreObjeto, new List<ParteRenderData>());
        }

        private void MostrarSeleccionActual()
        {
            string[] niveles = { "ESCENARIO", "OBJETO", "PARTE" };
            string[] objetos = { "Todos", "Escritorio", "Monitor", "CPU", "Teclado" };

            Console.WriteLine($"[{niveles[(int)nivelActual]}] {objetos[objetoSeleccionado]}");

            if (nivelActual == NivelSeleccion.Parte && objetoSeleccionado > 0)
            {
                var partes = ObtenerPartesObjetoSeleccionado();
                if (partes.Count > 0 && parteSeleccionada > 0 && parteSeleccionada <= partes.Count)
                {
                    Console.WriteLine($"  └─ Parte: {partes[parteSeleccionada - 1].NombreParte}");
                }
                else if (parteSeleccionada == 0)
                {
                    Console.WriteLine($"  └─ Todas las partes ({partes.Count})");
                }
            }
        }

        private void ProcesarSerializacion(KeyboardState keyboard)
        {
            if (IsKeyJustPressed(Key.F5))
                GuardarEscena();

            if (IsKeyJustPressed(Key.F9))
                CargarEscena();
        }

        private void ProcesarTransformacionObjetos(KeyboardState keyboard, float deltaTime)
        {
            float speed = VELOCIDAD_TRANSFORMACION * deltaTime;

            // TRANSLATION
            if (keyboard.IsKeyDown(Key.W)) translacionManual += Vector3.UnitZ * -speed;
            if (keyboard.IsKeyDown(Key.S)) translacionManual += Vector3.UnitZ * speed;
            if (keyboard.IsKeyDown(Key.A)) translacionManual += Vector3.UnitX * -speed;
            if (keyboard.IsKeyDown(Key.D)) translacionManual += Vector3.UnitX * speed;

            // ROTATION
            if (keyboard.IsKeyDown(Key.R)) rotacionManual.Y += speed;
            if (keyboard.IsKeyDown(Key.F)) rotacionManual.Y -= speed;
            if (keyboard.IsKeyDown(Key.T)) rotacionManual.X += speed;
            if (keyboard.IsKeyDown(Key.G)) rotacionManual.X -= speed;

            // SCALE
            if (keyboard.IsKeyDown(Key.Plus)) escalaManual *= 1.02f;
            if (keyboard.IsKeyDown(Key.Minus)) escalaManual *= 0.98f;

            // REFLECTION
            if (IsKeyJustPressed(Key.X))
            {
                reflectionX = !reflectionX;
                Console.WriteLine($"Reflection X: {(reflectionX ? "ON" : "OFF")}");
            }
            if (IsKeyJustPressed(Key.Y))
            {
                reflectionY = !reflectionY;
                Console.WriteLine($"Reflection Y: {(reflectionY ? "ON" : "OFF")}");
            }
            if (IsKeyJustPressed(Key.Z))
            {
                reflectionZ = !reflectionZ;
                Console.WriteLine($"Reflection Z: {(reflectionZ ? "ON" : "OFF")}");
            }

            // RESET
            if (IsKeyJustPressed(Key.Space))
            {
                ResetearTransformaciones();
            }
        }

        private void ResetearTransformaciones()
        {
            rotacionManual = Vector3.Zero;
            translacionManual = Vector3.Zero;
            escalaManual = Vector3.One;
            reflectionX = reflectionY = reflectionZ = false;
            Console.WriteLine("Transformaciones reseteadas");
        }

        private void ProcesarInputCamara(KeyboardState keyboard, float deltaTime)
        {
            float speed = VELOCIDAD_CAMARA * deltaTime;
            Vector3 forward = Vector3.Normalize(cameraTarget - cameraPos);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));

            // Movimiento básico
            if (keyboard.IsKeyDown(Key.W)) cameraPos += forward * speed;
            if (keyboard.IsKeyDown(Key.S)) cameraPos -= forward * speed;
            if (keyboard.IsKeyDown(Key.A)) cameraPos -= right * speed;
            if (keyboard.IsKeyDown(Key.D)) cameraPos += right * speed;

            // Rotación de cámara
            if (keyboard.IsKeyDown(Key.Left)) RotarCamara(1.5f * deltaTime);
            if (keyboard.IsKeyDown(Key.Right)) RotarCamara(-1.5f * deltaTime);
        }

        private void RotarCamara(float angulo)
        {
            Vector3 toCamera = cameraPos - cameraTarget;
            float cosY = (float)Math.Cos(angulo);
            float sinY = (float)Math.Sin(angulo);

            Vector3 newToCamera = new Vector3(
                toCamera.X * cosY + toCamera.Z * sinY,
                toCamera.Y,
                -toCamera.X * sinY + toCamera.Z * cosY
            );

            cameraPos = cameraTarget + newToCamera;
        }
        #endregion

        #region Serialización
        private void GuardarEscena()
        {
            try
            {
                Console.WriteLine("=== GUARDANDO ESCENA ===");

                var escenaData = new EscenaData(escenario, cameraPos, cameraTarget, lightPos)
                {
                    TransformacionActual = new TransformacionData
                    {
                        ObjetoSeleccionado = objetoSeleccionado,
                        ParteSeleccionada = parteSeleccionada, // NUEVO
                        NivelSeleccion = (int)nivelActual, // NUEVO
                        RotacionManual = rotacionManual,
                        TranslacionManual = translacionManual,
                        EscalaManual = escalaManual,
                        ReflectionX = reflectionX,
                        ReflectionY = reflectionY,
                        ReflectionZ = reflectionZ
                    }
                };

                string nombreArchivo = $"escena_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                Serializar.GuardarComoJson(escenaData, nombreArchivo);

                Console.WriteLine($"Escena guardada: {nombreArchivo}");
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
                    Console.WriteLine("No hay escenas guardadas. Presiona F5 para guardar la escena actual");
                    return;
                }

                string ultimaEscena = archivos.OrderByDescending(f => f).First();
                var escenaData = Serializar.CargarDesdeJson<EscenaData>(ultimaEscena);

                if (escenaData != null)
                {
                    RestaurarEscena(escenaData);
                    Console.WriteLine($"Escena cargada: {ultimaEscena}");
                    Console.WriteLine("========================");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando escena: {ex.Message}");
            }
        }

        private void RestaurarEscena(EscenaData escenaData)
        {
            cameraPos = escenaData.PosicionCamara;
            cameraTarget = escenaData.ObjetivoCamara;
            lightPos = escenaData.PosicionLuz;

            if (escenaData.TransformacionActual != null)
            {
                var transform = escenaData.TransformacionActual;
                objetoSeleccionado = transform.ObjetoSeleccionado;
                parteSeleccionada = transform.ParteSeleccionada; // NUEVO
                nivelActual = (NivelSeleccion)transform.NivelSeleccion; // NUEVO
                rotacionManual = transform.RotacionManual;
                translacionManual = transform.TranslacionManual;
                escalaManual = transform.EscalaManual;
                reflectionX = transform.ReflectionX;
                reflectionY = transform.ReflectionY;
                reflectionZ = transform.ReflectionZ;
            }
        }
        #endregion

        #region Shaders y OpenGL
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

        private void PrepararBuffersRenderizado()
        {
            foreach (var objeto in escenario.Objetos)
            {
                var partesRenderData = new List<ParteRenderData>();

                foreach (var parte in objeto.Partes)
                {
                    var vertices = ObtenerVerticesParaParte(parte);
                    var indices = ObtenerIndicesParaParte(parte);

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

                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VERTICES_POR_NORMAL * sizeof(float), 0);
                    GL.EnableVertexAttribArray(0);

                    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, VERTICES_POR_NORMAL * sizeof(float), 3 * sizeof(float));
                    GL.EnableVertexAttribArray(1);

                    GL.BindVertexArray(0);

                    partesRenderData.Add(new ParteRenderData
                    {
                        VAO = vao,
                        VBO = vbo,
                        EBO = ebo,
                        IndexCount = indices.Count,
                        BaseColor = parte.Color != Vector3.One ? parte.Color : objeto.ColorBase,
                        Position = objeto.Posicion + parte.Posicion,
                        Rotation = parte.Rotacion,
                        Scale = parte.Escala,
                        NombreParte = parte.Nombre,
                        NombreObjeto = objeto.Nombre
                    });
                }

                renderObjects[objeto.Nombre] = partesRenderData;
            }
        }

        private List<float> ObtenerVerticesParaParte(Parte parte)
        {
            var vertices = new List<float>();

            foreach (var cara in parte.Caras)
            {
                foreach (var vertice in cara.Vertices)
                {
                    vertices.Add(vertice.X);
                    vertices.Add(vertice.Y);
                    vertices.Add(vertice.Z);

                    var normal = cara.CalcularNormal();
                    vertices.Add(normal.X);
                    vertices.Add(normal.Y);
                    vertices.Add(normal.Z);
                }
            }

            return vertices;
        }

        private List<uint> ObtenerIndicesParaParte(Parte parte)
        {
            var indices = new List<uint>();
            uint baseIndex = 0;

            foreach (var cara in parte.Caras)
            {
                foreach (var indice in cara.Indices)
                {
                    indices.Add(baseIndex + indice);
                }
                baseIndex += (uint)cara.Vertices.Count;
            }

            return indices;
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
        #endregion

        #region Cleanup
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnUnload(EventArgs e)
        {
            foreach (var partesObjeto in renderObjects.Values)
            {
                foreach (var parteData in partesObjeto)
                {
                    GL.DeleteVertexArray(parteData.VAO);
                    GL.DeleteBuffer(parteData.VBO);
                    GL.DeleteBuffer(parteData.EBO);
                }
            }

            GL.DeleteProgram(shaderProgram);
            base.OnUnload(e);
        }
        #endregion
    }
}