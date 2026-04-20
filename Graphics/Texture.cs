using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace OpenGLAsi.Graphics;

public class Texture
{
    public int ID;

    public Texture(String filePath)
    {
        ID = GL.GenTexture();
        
        
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, ID);
        
        // Set texture parameters
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        
        StbImage.stbi_set_flip_vertically_on_load(1); // Flip the image vertically
        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Textures", filePath);
        ImageResult catImageResult = ImageResult.FromMemory(File.ReadAllBytes(fullPath), ColorComponents.RedGreenBlueAlpha);

        
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, catImageResult.Width, catImageResult.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, catImageResult.Data);

        Unbind();
    }
    
    public void Bind()
    {
        GL.BindTexture(TextureTarget.Texture2D, ID);
    }
    
    public void Unbind()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }
    
    public void Delete()
    {
        GL.DeleteTexture(ID);
    }
}