using OpenGLAsi.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenGLAsi.Components.World;

public class Sky
{
    private List<Vector3> chunkVertices;
    private List<Vector2> chunkTexCoords;
    private List<uint> chunkIndices = null!;
    private Vector3 position;
    private const int SIZE = 100;
    
    VAO chunkVAO = null!;
    VBO chunkVBO = null!;
    VBO chunkTexVBO = null!;
    IBO chunkIBO = null!;
    
    Texture chunkTexture = null!;
    private readonly string texturePath = null!;
    
    public Sky(Vector3 position, string texturePath)
    {
        this.position = position;
        this.texturePath = texturePath;
        
        chunkVertices = new List<Vector3>
        {
            new Vector3(-SIZE, SIZE, -SIZE) + position,
            new Vector3(SIZE, SIZE, -SIZE) + position,
            new Vector3(SIZE, SIZE, SIZE) + position,
            new Vector3(-SIZE, SIZE, SIZE) + position,
            
            new Vector3(-SIZE, -SIZE, -SIZE) + position,
            new Vector3(SIZE, -SIZE, -SIZE) + position,
            new Vector3(SIZE, -SIZE, SIZE) + position,
            new Vector3(-SIZE, -SIZE, SIZE) + position,
            
            new Vector3(-SIZE, SIZE, -SIZE) + position,
            new Vector3(SIZE, SIZE, -SIZE) + position,
            new Vector3(SIZE, -SIZE, -SIZE) + position,
            new Vector3(-SIZE, -SIZE, -SIZE) + position,
            
            new Vector3(SIZE, SIZE, SIZE) + position,
            new Vector3(-SIZE, SIZE, SIZE) + position,
            new Vector3(-SIZE, -SIZE, SIZE) + position,
            new Vector3(SIZE, -SIZE, SIZE) + position,
            
            new Vector3(-SIZE, SIZE, -SIZE) + position,
            new Vector3(-SIZE, SIZE, SIZE) + position,
            new Vector3(-SIZE, -SIZE, SIZE) + position,
            new Vector3(-SIZE, -SIZE, -SIZE) + position,
            
            new Vector3(SIZE, SIZE, -SIZE) + position,
            new Vector3(SIZE, SIZE, SIZE) + position,
            new Vector3(SIZE, -SIZE, SIZE) + position,
            new Vector3(SIZE, -SIZE, -SIZE) + position
            
        };
        chunkTexCoords = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),

            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),

            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),

            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)

        };
        chunkIndices = new List<uint>
        {
            0, 1, 2,
            2, 3, 0,

            4, 5, 6,
            6, 7, 4,

            8, 9, 10,
            10, 11, 8,

            12, 13, 14,
            14, 15, 12,
            
            16, 17, 18,
            18, 19, 16,
            
            20, 21, 22,
            22, 23, 20
        };

        BuildChunk();

    }


    public void BuildChunk()
    {
        chunkVAO = new VAO();
        chunkVAO.Bind();

        
        chunkVBO = new VBO(chunkVertices);
        chunkVBO.Bind();
        chunkVAO.LinkToVBO(0, 3 , chunkVBO);
        
        chunkTexVBO = new VBO(chunkTexCoords);
        chunkTexVBO.Bind();
        chunkVAO.LinkToVBO(1, 2, chunkTexVBO);
        
        chunkIBO = new IBO(chunkIndices);
        chunkIBO.Bind();
        
        chunkTexture = new Texture(texturePath);
        
    }

    public void Render(ShaderProgram shaderProgram)
    {
        shaderProgram.Use();
        chunkVAO.Bind();
        chunkIBO.Bind();
        chunkTexture.Bind();
        
        Matrix4 model = Matrix4.CreateTranslation(position);
        shaderProgram.SetMatrix4("model", model);
        
        GL.DrawElements(PrimitiveType.Triangles, chunkIndices.Count, DrawElementsType.UnsignedInt, 0);
    }
    
    public void Delete()
    {
        chunkVAO.Delete();
        chunkVBO.Delete();
        chunkTexVBO.Delete();
        chunkIBO.Delete();
        chunkTexture.Delete();
    }
}