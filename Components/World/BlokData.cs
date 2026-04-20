using OpenTK.Mathematics;

namespace OpenGLAsi.Components.World;

public enum Faces
{
    Top,
    Bottom,
    Left,
    Right,
    Front,
    Back
}

public struct FaceData
{
    public List<Vector3> Vertices;
    public List<Vector2> TexCoords; //uv
}

public struct RawFaceData
{
    public static readonly Dictionary<Faces, List<Vector3>> rawVertexData = new Dictionary<Faces, List<Vector3>>
    {
        {
            Faces.Front, new List<Vector3>()
            {
                // Front face
                new Vector3(-0.5f, 0.5f, 0.5f), // Top left
                new Vector3(0.5f, 0.5f, 0.5f), // Top right
                new Vector3(0.5f, -0.5f, 0.5f), // Bottom right
                new Vector3(-0.5f, -0.5f, 0.5f), // Bottom left
            }
        },
        {
            Faces.Back, new List<Vector3>()
            {
                // back face
                new Vector3(-0.5f, 0.5f, -0.5f), // Top left
                new Vector3(0.5f, 0.5f, -0.5f), // Top right
                new Vector3(0.5f, -0.5f, -0.5f), // Bottom right
                new Vector3(-0.5f, -0.5f, -0.5f), // Bottom left
            }
        },
        {
            Faces.Right, new List<Vector3>()
            {
                // Right face
                new Vector3(0.5f, 0.5f, -0.5f), // Top left
                new Vector3(0.5f, 0.5f, 0.5f), // Top right
                new Vector3(0.5f, -0.5f, 0.5f), // Bottom right
                new Vector3(0.5f, -0.5f, -0.5f), // Bottom left
            }
        },
        {
            Faces.Left, new List<Vector3>()
            {
                // Left face
                new Vector3(-0.5f, 0.5f, -0.5f), // Top left
                new Vector3(-0.5f, 0.5f, 0.5f), // Top right
                new Vector3(-0.5f, -0.5f, 0.5f), // Bottom right
                new Vector3(-0.5f, -0.5f, -0.5f), // Bottom left
            }
        },
        {
            Faces.Top, new List<Vector3>()
            {
                // Top face
                new Vector3(-0.5f, 0.5f, 0.5f), // Top left
                new Vector3(0.5f, 0.5f, 0.5f), // Top right
                new Vector3(0.5f, 0.5f, -0.5f), // Bottom right
                new Vector3(-0.5f, 0.5f, -0.5f), // Bottom left
            }
        },
        {
            Faces.Bottom, new List<Vector3>()
            {
                // Bottom face
                new Vector3(-0.5f, -0.5f, 0.5f), // Top left
                new Vector3(0.5f, -0.5f, 0.5f), // Top right
                new Vector3(0.5f, -0.5f, -0.5f), // Bottom right
                new Vector3(-0.5f, -0.5f, -0.5f), // Bottom left
            }
        },
    };
}
