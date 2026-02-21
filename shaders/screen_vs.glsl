#version 330 core

layout(location = 0) in vec4 vPosition; //xyz = particle position, w = mass 

uniform mat4 view;
uniform mat4 projection;

void main()
{
    //Only transform the position
    gl_Position = projection * view * vec4(vPosition.xyz, 1.0);

    gl_PointSize = 500.0 / gl_Position.w;
}