using OpenTK.Mathematics;

namespace OpenGLAsi.Components.World;

public class Blok
{
    public Vector3 position;

    private Dictionary<Faces, FaceData> faces;

    private List<Vector2> cartUVs = new List<Vector2>
    {
        new Vector2(0f, 1f), // Top left
        new Vector2(1f, 1f), // Top right
        new Vector2(1f, 0f), // Bottom right
        new Vector2(0f, 0f), // Bottom left
    };
    
    public Blok(Vector3 position)
    {
        this.position = position;

        faces = new Dictionary<Faces, FaceData>
        {
            { Faces.Front, new FaceData
            {
                Vertices = AddTransformedVertices(RawFaceData.rawVertexData[Faces.Front]),
                TexCoords = cartUVs
            }
            },
            { Faces.Back , new FaceData
            {
                Vertices = AddTransformedVertices(RawFaceData.rawVertexData[Faces.Back]),
                TexCoords = cartUVs
            }},
            { Faces.Right , new FaceData
            {
                Vertices = AddTransformedVertices(RawFaceData.rawVertexData[Faces.Right]),
                TexCoords = cartUVs
            }},
            { Faces.Left , new FaceData
            {
                Vertices = AddTransformedVertices(RawFaceData.rawVertexData[Faces.Left]),
                TexCoords = cartUVs
            }},
            { Faces.Top , new FaceData
            {
                Vertices = AddTransformedVertices(RawFaceData.rawVertexData[Faces.Top]),
                TexCoords = cartUVs
            }},
            { Faces.Bottom , new FaceData
            {
                Vertices = AddTransformedVertices(RawFaceData.rawVertexData[Faces.Bottom]),
                TexCoords = cartUVs
            }}
        };
    }
    public List<Vector3> AddTransformedVertices(List<Vector3> vertices)
    {
        List<Vector3> transformedVertices = new List<Vector3>();
        foreach (var vert in vertices)
        {
            transformedVertices.Add(vert+ position);
        }
        return transformedVertices;
    }

    public FaceData GetFace(Faces face)
    {
        return faces[face];
    }
}