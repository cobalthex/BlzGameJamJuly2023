shader_type spatial;
render_mode cull_disabled;

uniform sampler2D Sand;
uniform sampler2D Stone;
uniform sampler2D highlights;
uniform sampler2D Mask;

varying vec3 world_position;

void vertex(){
    world_position = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
    float scale = 0.02f;
    vec2 uv = world_position.xz * scale;

    
    // Generate and blend each Worley noise separately
    vec4 sandmask = texture(Sand, uv) * texture(Mask, UV).r;
    vec4 stonemask = texture(Stone, uv) * texture(Mask, UV).g;
	vec4 highmask = texture(highlights, uv) * texture(Mask, UV).b;
	vec4 compmask = highmask * (sandmask + stonemask);

    // Combine the blended Worley noises
    vec4 landscape = sandmask + stonemask + compmask * .6;

    ALBEDO = landscape.rgb;
	AO = texture(Mask, UV).a;
}



