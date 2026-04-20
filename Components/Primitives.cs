using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public static class Primitives
{
    
    
    public static ModelObject CreateCylinder(Vector3 position, int segments, float height, float radius, string texturePath)
    {
        List<Vector3> vertices = new();
        List<Vector2> texCoords = new();
        List<uint> indices = new();

        // Horní střed
        vertices.Add(new Vector3(0, height / 2f, 0));
        texCoords.Add(new Vector2(0.5f, 1.0f));
        uint topCenterIndex = 0;

        // Spodní střed
        vertices.Add(new Vector3(0, -height / 2f, 0));
        texCoords.Add(new Vector2(0.5f, 0.0f));
        uint bottomCenterIndex = 1;

        int offset = 2; // první boční vrchol bude mít index 2

        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * MathF.PI * 2f;
            float x = MathF.Cos(angle) * radius;
            float z = MathF.Sin(angle) * radius;

            // horní kružnice
            vertices.Add(new Vector3(x, height / 2f, z));
            texCoords.Add(new Vector2((float)i / segments, 1.0f));

            // spodní kružnice
            vertices.Add(new Vector3(x, -height / 2f, z));
            texCoords.Add(new Vector2((float)i / segments, 0.0f));

            if (i < segments)
            {
                int ti = offset + i * 2;
                int bi = ti + 1;
                int tnext = ti + 2;
                int bnext = bi + 2;

                // strany válce (2 trojúhelníky)
                indices.Add((uint)ti);
                indices.Add((uint)bi);
                indices.Add((uint)tnext);

                indices.Add((uint)tnext);
                indices.Add((uint)bi);
                indices.Add((uint)bnext);

                // horní podstava
                indices.Add(topCenterIndex);
                indices.Add((uint)tnext);
                indices.Add((uint)ti);

                // spodní podstava
                indices.Add(bottomCenterIndex);
                indices.Add((uint)bi);
                indices.Add((uint)bnext);
            }
        }

        return new ModelObject(vertices, texCoords, indices, texturePath, position);
    }

    public static ModelObject CreateBox(Vector3 position, string texturePath)
    {
        
        
        List<Vector3> vertices = new();
        List<Vector2> texCoords = new();
        List<uint> indices = new();

        // Definice vrcholů krychle
        vertices.Add(new Vector3(-1, -1, -1)); // 0
        vertices.Add(new Vector3(1, -1, -1));  // 1
        vertices.Add(new Vector3(1, 1, -1));   // 2
        vertices.Add(new Vector3(-1, 1, -1));  // 3
        
        vertices.Add(new Vector3(-1, -1, 1));  // 4
        vertices.Add(new Vector3(1, -1, 1));   // 5
        vertices.Add(new Vector3(1, 1, 1));    // 6
        vertices.Add(new Vector3(-1, 1, 1));   // 7
        
  

        // Definice texturových souřadnic
        texCoords.AddRange(new List<Vector2>
        {
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)
        });

        // Definice indexů pro trojúhelníky krychle
        indices.AddRange(new List<uint>
        {
            0, 1, 2,
            2, 3, 0,
            
            4, 5, 6,
            6, 7, 4,
            
            0, 1, 5,
            5, 4, 0,
            
            2, 3, 7,
            7, 6, 2,
            0, 3, 7,
            7, 4, 0,
            1, 2, 6,
            6, 5, 1
            
            
            
        });

        return new ModelObject(vertices, texCoords, indices, texturePath, position);
    }
    
    public static ModelObject CreateRay(Vector3 start, Vector3 end, float thickness = 0.1f)
    {
        Vector3 direction = end - start;
        float length = direction.Length;
        
        if (length < 0.001f) return null!;

        // Create base cylinder aligned with Y-axis
        ModelObject ray = CreateCylinder(
            position: Vector3.Zero,
            segments: 8,
            height: 1.0f,
            radius: thickness,
            texturePath: "grassTex.jpg"
        );

        UpdateRayTransform(ray, start, end);
        return ray;
    }

    public static void UpdateRayTransform(ModelObject ray, Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float length = direction.Length;
        
        if (length < 0.001f) return;

        direction.Normalize();
        Vector3 midpoint = (start + end) / 2;

        Matrix4 transform = Matrix4.CreateScale(1, length, 1) *
                            GetRotationToDirection(direction) *
                            Matrix4.CreateTranslation(midpoint);

        ray.Transform = transform;
    }

    private static Matrix4 GetRotationToDirection(Vector3 direction)
    {
        direction.Normalize();
        Vector3 axis = Vector3.Cross(Vector3.UnitY, direction);
        
        // Handle parallel/anti-parallel cases
        if (axis.LengthSquared < 0.0001f)
            return direction.Y > 0 ? Matrix4.Identity 
                : Matrix4.CreateRotationX(MathHelper.Pi);

        axis.Normalize();
        float angle = MathF.Acos(Vector3.Dot(Vector3.UnitY, direction));
        
        return Matrix4.CreateFromAxisAngle(axis, angle);
    }
    
    
    
    public static ModelObject CreateEnemy(Vector3 position, string texturePathHead, string texturePathBody)
{
    // Hlava (Koule)
    var head = CreateSphere(Vector3.Zero, 0.5f, 16, texturePathHead);
    
    // Tělo (Válec)
    var body = CreateCylinder(
        position: Vector3.Zero, 
        segments: 8,
        height: 1.5f,
        radius: 0.4f,
        texturePath: texturePathBody
    );
    head.Transform = Matrix4.CreateTranslation(new Vector3(0, 0.4f, 0));
    body.Transform = Matrix4.CreateTranslation(new Vector3(0, -0.7f, 0));
    
    ModelObject enemy = CombineModelsWithTransforms(head, body, texturePathHead);
    enemy.Position = position;
    // Spojení modelů
    return enemy;
}

    
    private static ModelObject CreateSphere(Vector3 position, float radius, int segments, string texturePath)
    {
        List<Vector3> vertices = new();
        List<Vector2> texCoords = new();
        List<uint> indices = new();

        for (int i = 0; i <= segments; i++)
        {
            float theta = MathF.PI * i / segments;
            for (int j = 0; j <= segments; j++)
            {
                float phi = 2 * MathF.PI * j / segments;
            
                Vector3 pos = new(
                    radius * MathF.Sin(theta) * MathF.Cos(phi),
                    radius * MathF.Cos(theta),  // Bez posunu - nechte transformaci matici
                    radius * MathF.Sin(theta) * MathF.Sin(phi)
                );
            
                vertices.Add(pos);
                texCoords.Add(new Vector2((float)j / segments, (float)i / segments));
            
                if (i < segments && j < segments)
                {
                    uint current = (uint)(i * (segments + 1) + j);
                    uint next = current + (uint)segments + 1;

                    indices.Add(current);
                    indices.Add(next);
                    indices.Add(current + 1);

                    indices.Add(next);
                    indices.Add(next + 1);
                    indices.Add(current + 1);
                }
            }
        }
    
        return new ModelObject(vertices, texCoords, indices, texturePath, position);
    }
    
    private static ModelObject CombineModelsWithTransforms(ModelObject a, ModelObject b, string texturePath)
    {
        // Aplikujeme transformace na vrcholy
        var aVertices = a.vertices.Select(v => Vector3.TransformPosition(v, a.Transform)).ToList();
        var bVertices = b.vertices.Select(v => Vector3.TransformPosition(v, b.Transform)).ToList();
    
        uint offset = (uint)aVertices.Count;
    
        return new ModelObject(
            vertices: aVertices.Concat(bVertices).ToList(),
            texCoords: a.texCoords.Concat(b.texCoords).ToList(),
            indices: a.indices.Concat(b.indices.Select(i => i + offset)).ToList(),
            texturePath: texturePath,
            position: Vector3.Zero  // Pozice se nastaví až na finálním objektu
        );
    }

    
    
    
    

}