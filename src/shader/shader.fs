#version 330 core
#define NR_POINT_LIGHTS 32

struct PointLight {
    vec3  position;
    vec3  color;
    float intensity;
    float constant;
    float linear;
    float quadratic;
};

in vec3 FragPos;
in vec3 Normal;
in vec2 TextCoord;
layout (location = 0) out vec4 FragColor;
layout (location = 1) out vec4 BrightColor;

uniform sampler2D  ourTexture;
uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform vec3       viewPos;

uniform vec3  sunDirection    = vec3(-0.8, -0.2, -0.5);
uniform vec3  sunColor        = vec3(1.0, 0.50, 0.25); 
uniform float sunIntensity    = 1.2;
uniform vec3  skyColor        = vec3(0.16, 0.18, 0.36); 
uniform vec3  groundColor     = vec3(0.05, 0.04, 0.04);
uniform float ambientStrength = 0.25;
uniform float pointLightScale = 0.9; 
uniform vec3  fogColor        = vec3(0.28, 0.20, 0.24);
uniform float fogDensity      = 0.0004;
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir, vec3 albedo)

{
    vec3  lightDir    = normalize(light.position - fragPos);
    float diff        = max(dot(normal, lightDir), 0.0);
    vec3  halfDir     = normalize(lightDir + viewDir);
    float spec        = diff > 0.0 ? pow(max(dot(normal, halfDir), 0.0), 32.0) : 0.0;
    float dist        = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * dist + light.quadratic * dist * dist);

    vec3 diffuse  = diff * albedo * light.color * light.intensity;
    vec3 specular = spec * vec3(0.3) * light.color * light.intensity;
    return (diffuse + specular) * attenuation * pointLightScale;
}

void main()
{
    vec3 norm = normalize(Normal);
    if (!gl_FrontFacing) norm = -norm; 
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 albedo  = texture(ourTexture, TextCoord).rgb;

    vec3 ambient = mix(groundColor, skyColor, norm.y * 0.5 + 0.5) * ambientStrength * albedo;

    vec3  L    = normalize(-sunDirection);
    float diff = max(dot(norm, L), 0.0);
    vec3  H    = normalize(L + viewDir);
    float spec = diff > 0.0 ? pow(max(dot(norm, H), 0.0), 64.0) : 0.0;
    vec3 result = ambient + sunColor * sunIntensity * (diff * albedo + 0.15 * spec);

    for (int i = 0; i < NR_POINT_LIGHTS; i++)
        result += CalcPointLight(pointLights[i], norm, FragPos, viewDir, albedo);

    float d   = length(viewPos - FragPos);
    float fog = 1.0 - exp(-pow(d * fogDensity, 2.0));
    result = mix(result, fogColor, clamp(fog, 0.0, 1.0));

    FragColor = vec4(result, 1.0);

    float brightness = dot(result, vec3(0.2126, 0.7152, 0.0722));
    BrightColor = brightness > 2.5 ? vec4(result, 1.0) : vec4(0.0, 0.0, 0.0, 1.0);
}