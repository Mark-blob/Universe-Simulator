#version 450 core

uniform float dt;

struct Particle
{
    vec3 position;
    float mass;
    vec3 velocity;
    float color;
};

layout(std430, binding = 0) buffer Particles
{
    Particle particles[];
};

layout(local_size_x = 256) in;

void main()
{
    uint idx = gl_GlobalInvocationID.x;
    if (idx >= particles.length()) return;
    particles[idx].position += particles[idx].velocity * dt;
}