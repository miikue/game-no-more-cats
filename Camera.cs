using OpenGLAsi.Components.World;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenGLAsi;

public class Camera
{
    // CONSTANTS
    private float SPEED = 8f;
    private float SENSITIVITY = 180f;
    private float SCREEN_WIDTH;
    private float SCREEN_HEIGHT;
    
    // POSITION AND ORIENTATION
    public Vector3 position;
    
    Vector3 up = Vector3.UnitY;
    Vector3 front = -Vector3.UnitZ; // - because otherwise it look behind the camera
    Vector3 right = Vector3.UnitX;
    
    
    // View rotation
    private float yaw = -90f; // Yaw is the rotation around the Y axis
    private float pitch; // Pitch is the rotation around the X axis

    private bool firstMove = true;
    private Vector2 lastPos;
    
    
    // jump 
    private float verticalVelocity = 0f;
    private float gravity = -20f;
    private float jumpForce = 7f;
    public Chunk chunk;

    
    public Camera(float width, float height, Vector3 position)
    {
        SCREEN_WIDTH = width;
        SCREEN_HEIGHT = height;
        this.position = position;
    }

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt( position, position + front, up);
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45.0f),
            SCREEN_WIDTH / SCREEN_HEIGHT,
            0.1f, 1000.0f);
    }
    
    public (Vector3 origin, Vector3 direction) GetRayLine()
    {
        Vector2 screenPosition = new Vector2(SCREEN_WIDTH / 2f, SCREEN_HEIGHT / 2f);
    
        // Convert to NDC
        float x = (2.0f * screenPosition.X) / SCREEN_WIDTH - 1.0f;
        float y = 1.0f - (2.0f * screenPosition.Y) / SCREEN_HEIGHT;

        Vector4 rayClip = new Vector4(x, y, -1.0f, 1.0f);

        Matrix4 invProjection = Matrix4.Invert(GetProjectionMatrix());
        Vector4 rayEye = invProjection * rayClip;

        rayEye /= rayEye.W;
        rayEye.W = 0.0f;

        Matrix4 invView = Matrix4.Invert(GetViewMatrix());
        Vector3 rayWorld = Vector3.TransformVector(rayEye.Xyz, invView).Normalized();

        return (position, rayWorld);
    }
    private void UpdateVectors()
    {
        // Clamp pitch to avoid flipping
        pitch = MathHelper.Clamp(pitch, -89.0f, 89.0f);

        // Calculate new front vector
        front.X = MathF.Cos(MathHelper.DegreesToRadians(pitch)) 
                  * MathF.Cos(MathHelper.DegreesToRadians(yaw));
        front.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
        front.Z = MathF.Cos(MathHelper.DegreesToRadians(pitch)) 
                  * MathF.Sin(MathHelper.DegreesToRadians(yaw));

        front = Vector3.Normalize(front);
        right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
        up = Vector3.Normalize(Vector3.Cross(right, front));
    }
    
    
    public void InputController(KeyboardState input, MouseState mouse, FrameEventArgs e)
    {
        if (input.IsKeyDown(Keys.W))
        {
            position += front * SPEED * (float)e.Time;
        }
        if (input.IsKeyDown(Keys.S))
        {
            position -= front * SPEED * (float)e.Time;
        }
        if (input.IsKeyDown(Keys.A))
        {
            position -= right * SPEED * (float)e.Time;
        }
        if (input.IsKeyDown(Keys.D))
        {
            position += right * SPEED * (float)e.Time;
        }
        if (input.IsKeyDown(Keys.Space))
        {
            //position += up * SPEED * (float)e.Time;
            verticalVelocity = jumpForce;
        }
        if (input.IsKeyDown(Keys.LeftShift))
        {
            position -= up * SPEED * (float)e.Time;
        }

        if (firstMove)
        {
            lastPos = new Vector2(mouse.X, mouse.Y);
            firstMove = false;
        }
        else
        {
            var deltaX = mouse.X - lastPos.X;
            var deltaY = mouse.Y - lastPos.Y;
            lastPos = new Vector2(mouse.X, mouse.Y);
            
            yaw += deltaX * SENSITIVITY * (float)e.Time;
            pitch -= deltaY * SENSITIVITY * (float)e.Time;
        }
        UpdateVectors();
    }
    
    

    public void Update(KeyboardState input, MouseState mouse, FrameEventArgs e)
    {
        InputController(input, mouse, e);
        
        // Apply gravity
        verticalVelocity += gravity * (float)e.Time;
        position.Y += verticalVelocity * (float)e.Time;

        float groundHeight = 2f + chunk.GetTerrainHeight(position.X, position.Z);

        if (position.Y <= groundHeight)
        {
            position.Y = groundHeight;
            verticalVelocity = 0;
        }
    }
}