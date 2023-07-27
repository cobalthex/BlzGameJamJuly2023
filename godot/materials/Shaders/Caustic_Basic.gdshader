shader_type spatial;
render_mode cull_disabled;

uniform sampler2D color_ramp;

uniform sampler2D Caustics;
uniform sampler2D Caustics2;

varying vec3 world_position;

void vertex(){
    world_position = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
    float scale = 0.05f;
    vec2 uv = world_position.xz * scale;

    
    // Generate and blend each Worley noise separately
    vec4 worley1 = texture(Caustics, uv + vec2(TIME * 0.1f));
    vec4 worley2 = texture(Caustics2, uv + vec2(TIME * -0.1f));

    // Combine the blended Worley noises
    vec4 worley = worley1 * worley2;

    ALBEDO = worley.rgb;
	ALPHA = 0.75 * worley.r;
}



