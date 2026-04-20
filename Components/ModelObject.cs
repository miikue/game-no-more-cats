using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenGLAsi.Graphics;

public class ModelObject
{
    private VAO vao;
    private VBO vbo;
    private VBO texVbo;
    private IBO ibo;
    private Texture texture;
    private int indexCount;
    public List<Vector2> texCoords;
    
    public List<Vector3> vertices;
    public List<uint> indices;

    public Matrix4 Transform = Matrix4.Identity;

    public Vector3 Position
    {
        get => Transform.ExtractTranslation();
        set => Transform = Matrix4.CreateTranslation(value) * Transform.ClearTranslation();
    }
    public ModelObject(List<Vector3> vertices, List<Vector2> texCoords, List<uint> indices, string texturePath, Vector3 position)
    {
        Position = position;
        this.vertices = vertices;
        this.texCoords = texCoords;
        this.indices = indices;
        
        

        vao = new VAO();
        vao.Bind();

        vbo = new VBO(this.vertices);
        vbo.Bind();
        vao.LinkToVBO(0, 3, vbo);

        texVbo = new VBO(texCoords);
        texVbo.Bind();
        vao.LinkToVBO(1, 2, texVbo);

        ibo = new IBO(indices);
        ibo.Bind();

        texture = new Texture(texturePath);
        indexCount = indices.Count;
    }

    public void Render(ShaderProgram shader)
    {
        if (vertices.Count != texCoords.Count)
        {
            throw new InvalidOperationException("Vertex count does not match texture coordinate count. (" + vertices.Count + " vs " + texCoords.Count + ")");
        }
        
        shader.Use();
        vao.Bind();
        ibo.Bind();
        texture.Bind();
        shader.SetMatrix4("model", Transform);
        

        GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
    }
    
    // get hitbox
    public AABB GetHitbox()
    {
        Vector3 min = new Vector3(float.MaxValue);
        Vector3 max = new Vector3(float.MinValue);

        foreach (var vertex in vertices)
        {
            // Transform model-space vertex to world space
            Vector3 worldVertex = vertex + Position;
            min = Vector3.ComponentMin(min, worldVertex);
            max = Vector3.ComponentMax(max, worldVertex);
        }

        
        return new AABB(min, max);
    }


    public void Delete()
    {
        vao.Delete();
        vbo.Delete();
        texVbo.Delete();
        ibo.Delete();
        texture.Delete();
    }
    
}

public class AABB
{
    public Vector3 Min { get; }
    public Vector3 Max { get; }

    public AABB(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
        
        
    }

}