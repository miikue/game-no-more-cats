using OpenGLAsi.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenGLAsi.Components.World;

public class Chunk
{
    private List<Vector3> chunkVertices;
    private List<Vector2> chunkTexCoords;
    private List<uint> chunkIndices;
    private Vector3 position;
    private uint indexCount = 0;

    public const int SIZE = 100;
    const int HEIGHT = 32; // Height of the chunk
    
    VAO chunkVAO;
    VBO chunkVBO;
    VBO chunkTexVBO;
    IBO chunkIBO;
    
    Texture chunkTexture;
    float[,] heightMap;
    
    public Chunk(Vector3 position)
    {
        this.position = position;
        
        chunkVertices = new List<Vector3>();
        chunkTexCoords = new List<Vector2>();
        chunkIndices = new List<uint>();
        
        heightMap = GenChunk();
        
        GenBlocks(heightMap);
        BuildChunk();

    }

    public float[,] GenChunk()
    {
        float[,] ret = new float[SIZE, SIZE];

        Vector2 peak1 = new Vector2(SIZE * 0.25f, SIZE * 0.25f);
        Vector2 peak2 = new Vector2(SIZE * 0.75f, SIZE * 0.75f);

        for (int x = 0; x < SIZE; x++)
        {
            for (int z = 0; z < SIZE; z++)
            {
                Vector2 pos = new Vector2(x, z);
                float d1 = (pos - peak1).Length;
                float d2 = (pos - peak2).Length;

                float height1 = MathF.Max(0, 1f - (d1 / (SIZE * 0.4f)));
                float height2 = MathF.Max(0, 1f - (d2 / (SIZE * 0.4f)));

                float combined = MathF.Pow(height1, 2f) + MathF.Pow(height2, 2f); // dvě hory
                float finalHeight = combined * 0.5f * HEIGHT;

                ret[x, z] = MathF.Max(1f, finalHeight); // zajištění minimální výšky
            }
        }

        return ret;
      
    }

    public (Vector2 X, Vector2 Z) GetChunkPosition()
    {
        // position limit X
        Vector2 x = new Vector2(position.X, position.X + SIZE);
        Vector2 z = new Vector2(position.Z, position.Z + SIZE);
        return (x, z);
    }

    public void GenBlocks(float[,] heightMap)
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int z = 0; z < SIZE; z++)
            {
                int height = (int)(heightMap[x, z]); // Scale height to the chunk height
                for (int y = 0; y < height; y++)
                {
                    Blok blok = new Blok(new Vector3(x, y, z));
                    IntegrateFace(blok, Faces.Front);
                    IntegrateFace(blok, Faces.Back);
                    IntegrateFace(blok, Faces.Right);
                    IntegrateFace(blok, Faces.Left);
                    IntegrateFace(blok, Faces.Top);
                    IntegrateFace(blok, Faces.Bottom);
                }
            }
        }
     
    }
    
    public void IntegrateFace(Blok blok, Faces face)
    {
        var faceData = blok.GetFace(face);
        chunkVertices.AddRange(faceData.Vertices);
        chunkTexCoords.AddRange(faceData.TexCoords);
        
        int numFaces = 1; // Each call to this method adds one face
        AddIndices(numFaces);
    }

    public void AddIndices(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            chunkIndices.Add(0 + indexCount);
            chunkIndices.Add(1 + indexCount);
            chunkIndices.Add(2 + indexCount);
            chunkIndices.Add(2 + indexCount);
            chunkIndices.Add(3 + indexCount);
            chunkIndices.Add(0 + indexCount);
            indexCount += 4; // Each face has 4 vertices
        }
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
        
        chunkTexture = new Texture("concrete.jpg");
        
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
    
    public float GetTerrainHeight(float x, float z)
    {
        int mapX = (int)MathF.Floor(x - position.X);
        int mapZ = (int)MathF.Floor(z - position.Z);

    
        // Zkontroluj, že jsi uvnitř pole
        if (mapX >= 0 && mapZ >= 0 && mapX < heightMap.GetLength(0) && mapZ < heightMap.GetLength(1))
        {
            return heightMap[mapX, mapZ];
        }
    
        return 0f; // mimo mapu = výška 0
    }
}