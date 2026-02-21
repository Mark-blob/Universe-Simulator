#version 330 core
out vec4 FragColor;

void main()
{
    vec2 coord = gl_PointCoord * 2.0 - 1.0; // [-1,1] coordinates
    if(dot(coord, coord) > 1.0) discard;    // circle mask

    FragColor = vec4(coord * 100, 1.0, 1.0);
}