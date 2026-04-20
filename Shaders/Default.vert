#version 330 core
layout (location = 0) in vec3 aPosition; // vertex position

layout (location = 1) in vec2 aTexCoord; // texture coordinates

// uniform variables
uniform mat4 model; 
uniform mat4 view;
uniform mat4 projection;

out vec2 TexCoord; // output texture coordinates to the fragment shader
void main()
{
    gl_Position = projection * view * model * vec4(aPosition, 1.0);
    TexCoord = aTexCoord; // pass the texture coordinates to the fragment shader
}

//gl_Position = vec4(aPosition, 1.0) * model * view * projection; // set the vertex position
