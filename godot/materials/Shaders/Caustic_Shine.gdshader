shader_type spatial;
render_mode cull_disabled;

uniform sampler2D color_ramp;

uniform sampler2D Caustics;
uniform sampler2D Caustics2;
uniform sampler2D Normal_m;
varying vec3 world_position;

void vertex(){
    world_position = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
    float scale = 0.005f;
    vec2 uv = world_position.xz * scale;

    
    // Generate and blend each Worley noise separately
    vec4 worley1 = texture(Caustics, uv + vec2(TIME * 0.05f));
    vec4 worley2 = texture(Caustics2, uv + vec2(TIME * -0.05f));
	vec4 normal = texture(Normal_m, uv + vec2(TIME * -0.05f));
	vec4 normal2 = texture(Normal_m, uv + vec2(TIME * 0.05f));
    // Combine the blended Worley noises
    vec4 worley = worley1 * worley2;
	vec4 normalc = normal + normal2;
    ALBEDO = worley.rgb;
	ALPHA = 0.5 * worley.r;
	EMISSION = 4.0 * worley.rgb;
	ROUGHNESS = 0.45;
	SPECULAR = 0.25;
}



