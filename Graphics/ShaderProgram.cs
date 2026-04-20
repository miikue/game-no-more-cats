using OpenTK.Mathematics;

namespace OpenGLAsi.Graphics;
using OpenTK.Graphics.OpenGL4;


public class ShaderProgram
{
    public int ID;
    
    public void Use()
    {
        GL.UseProgram(ID);
    }

    public void Delete()
    {
        GL.DeleteProgram(ID);
    }
    
    

    public ShaderProgram(string vertPath, string fragPath)
    {
        // Create program
        ID = GL.CreateProgram();
        
        // Create and compile shaders
        string shaderBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shaders");
        int vertShader = CompileShader(Path.Combine(shaderBase, vertPath), ShaderType.VertexShader);
        int fragShader = CompileShader(Path.Combine(shaderBase, fragPath), ShaderType.FragmentShader);

        // Attach and link
        GL.AttachShader(ID, vertShader);
        GL.AttachShader(ID, fragShader);
        GL.LinkProgram(ID);

        // Check linking errors
        GL.GetProgram(ID, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(ID);
            throw new Exception($"Program linking failed: {infoLog}");
        }

        // Cleanup shaders
        GL.DetachShader(ID, vertShader);
        GL.DetachShader(ID, fragShader);
        GL.DeleteShader(vertShader);
        GL.DeleteShader(fragShader);
    }

    private int CompileShader(string path, ShaderType type)
    {
        string source = File.ReadAllText(path);
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        // Check compilation errors
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"{type} compilation failed: {infoLog}");
        }

        return shader;
    }



    public void SetMatrix4(string name, Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(ID, name);
        if (location == -1)
        {
            throw new Exception($"Uniform {name} not found!");
        }
        GL.UniformMatrix4(location, false, ref matrix);
    }

    public void SetVector2(string name, Vector2 vector)
    {
        int location = GL.GetUniformLocation(ID, name);
        if (location == -1)
        {
            throw new Exception($"Uniform {name} not found!");
        }
        GL.Uniform2(location, vector);
    }

    public void SetVector3(string name, Vector3 vector)
    {
        int location = GL.GetUniformLocation(ID, name);
        if (location == -1)
        {
            Console.WriteLine($"⚠️ Uniform '{name}' not found!");
            return;
        }
        GL.Uniform3(location, vector);
    }
}
    
