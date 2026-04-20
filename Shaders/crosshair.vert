#version 330 core
layout(location = 0) in vec2 aPosition;

uniform mat4 projection;
uniform vec2 offset;

void main()
{
    vec2 pos = aPosition + offset;
    gl_Position = projection * vec4(pos, 0.0, 1.0);
}