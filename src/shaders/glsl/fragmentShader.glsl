#version 400 core

out vec4 FragColor;
in float multiplier;
in float h;

void main() {

    vec3 color = vec3(0.44, 0.61, 0.2)*(multiplier/7);

    if (h < 0) {
        color = vec3(0.02, 0.01, 0.4)*(multiplier+5);
    } 
    FragColor = vec4(color, 1.0);
}