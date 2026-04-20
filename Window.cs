
using OpenGLAsi;
using OpenGLAsi.Graphics;
using OpenGLAsi.Components.World;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class Window : GameWindow
{
    // World
    private Camera camera = null!;
    public Chunk chunk = null!;
    private Sky sky = null!;
    private ModelObject rayModelObject = null!;

    // Models in the world
    private List<ModelObject> modelObjects = null!;
    private int numberOfEnemies = 10;
    private int shootedEnemyIndex = -1; // Index of the last hit enemy, -1 if none hit
    private int numberOfShootedEnemies = 0;
    
    
    // Shader programs
    private ShaderProgram _shaderProgram = null!;
    private ShaderProgram _rayProgram = null!;
    private ShaderProgram _crosshairShader = null!;


    // Rendering screen _width and height
    private int _width, _height;
    
    public Window(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        _width = width;
        _height = height; 
        modelObjects = new List<ModelObject>();
        
        // Center the window on the screen
        CenterWindow(new Vector2i(width, height));
        Console.WriteLine("OpenGL version: " + GL.GetString(StringName.Version));

    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0,0, e.Width,e.Height);
        _height = e.Height;
        _width = e.Width;
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        // Enable blending for transparency
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // Make world
        chunk = new Chunk(new Vector3(-40, 1, -40));
        sky = new Sky(new Vector3(0,-10,0), "skyTex.jpeg");
        
        modelObjects.Add(Primitives.CreateEnemy(new Vector3(5, chunk.GetTerrainHeight(5, 5) + 3, 5), "textureCat.png", "grassTex.jpg"));
        
        _shaderProgram = new ShaderProgram("Default.vert", "Default.frag");
        _rayProgram = new ShaderProgram("Ray.vert", "Ray.frag");
        
        GL.Enable(EnableCap.DepthTest);
        
        // lock frame rate :P
        this.VSync = VSyncMode.On;
        
        
        camera = new Camera(_width, _height, Vector3.Zero);
        camera.chunk = chunk;
        
        CursorState = CursorState.Grabbed;
        SetupCrosshair();
        _crosshairShader = new ShaderProgram("crosshair.vert", "crosshair.frag");
    }
    protected override void OnUnload()
    {
        base.OnUnload();
        chunk.Delete();
        sky.Delete();
        modelObjects.ForEach(obj => obj.Delete());
        _shaderProgram.Delete();
        _rayProgram.Delete();
        _crosshairShader.Delete();

    }
    
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        MouseState mouse = MouseState;
        KeyboardState input = KeyboardState;
        
        base.OnUpdateFrame(args);
        camera.Update(input, mouse, args);
        
        // Enemy management
        if (shootedEnemyIndex >= 0 && shootedEnemyIndex < modelObjects.Count)
        {
            modelObjects.RemoveAt(shootedEnemyIndex);
            shootedEnemyIndex = -1;
            numberOfShootedEnemies++;
        }
        if (modelObjects.Count < numberOfEnemies)
        {
            // Add new enemies if there are less than the desired number
            for (int i = modelObjects.Count; i < numberOfEnemies; i++)
            {
                // Limit to map size (location)
                Vector2 chunkPositionX = chunk.GetChunkPosition().X;
                Vector2 chunkPositionZ = chunk.GetChunkPosition().Z;
                int X = Random.Shared.Next((int)chunkPositionX.X, (int)chunkPositionX.Y);
                int Z = Random.Shared.Next((int)chunkPositionZ.X, (int)chunkPositionZ.Y);
                
                // Ensure the position is above the terrain
                float Y = chunk.GetTerrainHeight(X, Z);
                Vector3 position = new Vector3(X, Y+1, Z);
                modelObjects.Add(Primitives.CreateEnemy(position, "textureCat.png", "grassTex.jpg"));
            }
        }
    }
    
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.ClearColor(0.5f, 0.7f, 1.0f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        
        Matrix4 viewMatrix = camera.GetViewMatrix();
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram.ID, "view"), false, ref viewMatrix);
        Matrix4 projectionMatrix = camera.GetProjectionMatrix();
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram.ID, "projection"), false, ref projectionMatrix);

        
        chunk.Render(_shaderProgram);
        sky.Render(_shaderProgram);
        modelObjects.ForEach(obj => obj.Render(_shaderProgram));

        if (rayModelObject != null)
        {
            _rayProgram.Use();
            _rayProgram.SetMatrix4("view", viewMatrix);
            _rayProgram.SetMatrix4("projection", projectionMatrix);
            rayModelObject.Render(_rayProgram);
        }
        
        
        GL.Disable(EnableCap.DepthTest);
        
        _crosshairShader.Use();
        
        Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, _width, _height, 0, -1, 1);
        _crosshairShader.SetMatrix4("projection", ortho);
        _crosshairShader.SetVector2("offset", new Vector2(_width / 2f, _height / 2f));

        GL.BindVertexArray(crosshairVao);
        GL.LineWidth(6.0f);
        GL.DrawArrays(PrimitiveType.Lines, 0, 4);
        GL.BindVertexArray(0);

        GL.Enable(EnableCap.DepthTest);
        
        
        Context.SwapBuffers();
        this.Title = "FPS: " + (1f / args.Time).ToString("0.00") + " | OpenGL version: " + GL.GetString(StringName.Version) + " | Enemies: " + modelObjects.Count + " | Shooted: " + numberOfShootedEnemies;
        _shaderProgram.Use();

        base.OnRenderFrame(args);
    }
    
    private int crosshairVao;
    private int crosshairVbo;

    private void SetupCrosshair()
    {
        float size = 20f;

        float[] vertices = new float[]
        {
            -size, 0f,
            size, 0f,
            0f, -size,
            0f,  size
        };

        crosshairVao = GL.GenVertexArray();
        GL.BindVertexArray(crosshairVao);

        crosshairVbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, crosshairVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

        GL.BindVertexArray(0);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (e.Button == MouseButton.Left)
        {
            //Console.WriteLine(camera.GetRayLine());
            Vector3 rayOrigin = camera.GetRayLine().origin;
            Vector3 rayDirection = camera.GetRayLine().direction;
            
            rayOrigin.Y -= 0.2f;
            rayModelObject = Primitives.CreateRay(
                rayOrigin,
                rayOrigin + rayDirection * 10f, // Length of the ray
                0.10f // Thickness of the ray
            );
            rayModelObject.Render(_rayProgram);

            for (int i = 0; i < modelObjects.Count; i++)
            {
                ModelObject modelObject = modelObjects[i];
                AABB aabb = modelObject.GetHitbox();
                if (RayIntersectsAABB(rayOrigin, rayDirection, aabb.Min, aabb.Max, out float t))
                {
                    Console.WriteLine("Ray hit model object at distance: " + t);
                    shootedEnemyIndex = i;
                }
            }
        }
    }    
    
    
    public bool RayIntersectsAABB(Vector3 rayOrigin, Vector3 rayDirection, Vector3 aabbMin, Vector3 aabbMax, out float t)
    {
        t = 0f;
        float tmin = float.MinValue;
        float tmax = float.MaxValue;

        // X-axis
        if (MathF.Abs(rayDirection.X) > 0.0001f)
        {
            float invDir = 1.0f / rayDirection.X;
            float t1 = (aabbMin.X - rayOrigin.X) * invDir;
            float t2 = (aabbMax.X - rayOrigin.X) * invDir;
            tmin = MathF.Max(tmin, MathF.Min(t1, t2));
            tmax = MathF.Min(tmax, MathF.Max(t1, t2));
        }
        else if (rayOrigin.X < aabbMin.X || rayOrigin.X > aabbMax.X)
        {
            return false; // Ray is parallel and outside the AABB
        }

        // Y-axis
        if (MathF.Abs(rayDirection.Y) > 0.0001f)
        {
            float invDir = 1.0f / rayDirection.Y;
            float t1 = (aabbMin.Y - rayOrigin.Y) * invDir;
            float t2 = (aabbMax.Y - rayOrigin.Y) * invDir;
            tmin = MathF.Max(tmin, MathF.Min(t1, t2));
            tmax = MathF.Min(tmax, MathF.Max(t1, t2));
        }
        else if (rayOrigin.Y < aabbMin.Y || rayOrigin.Y > aabbMax.Y)
        {
            return false;
        }

        // Z-axis
        if (MathF.Abs(rayDirection.Z) > 0.0001f)
        {
            float invDir = 1.0f / rayDirection.Z;
            float t1 = (aabbMin.Z - rayOrigin.Z) * invDir;
            float t2 = (aabbMax.Z - rayOrigin.Z) * invDir;
            tmin = MathF.Max(tmin, MathF.Min(t1, t2));
            tmax = MathF.Min(tmax, MathF.Max(t1, t2));
        }
        else if (rayOrigin.Z < aabbMin.Z || rayOrigin.Z > aabbMax.Z)
        {
            return false;
        }
        
        if (tmax >= MathF.Max(tmin, 0.0f))
        {
            t = tmin > 0 ? tmin : tmax; 
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Keys.Down)
        {
            if (numberOfEnemies > 0)
            {
                numberOfEnemies--;
            }
        }
        if (e.Key == Keys.Up)
        {
            numberOfEnemies++;
        }

        if (e.Key == Keys.Escape)
        {
            Close();
        }
        
    }
}