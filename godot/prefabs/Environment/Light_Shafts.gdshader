shader_type spatial;
render_mode blend_add, unshaded, cull_disabled;
uniform vec4 albedo : source_color;
uniform sampler2D texture_albedo: source_color;
uniform sampler2D texture_alpha: source_color;
uniform sampler2D texture_alphaN: source_color;
uniform float proximity_fade_distance = 1.0;
uniform sampler2D DEPTH_TEXTURE : hint_depth_texture, filter_linear_mipmap;

vec3 project_on_plane( vec3 vec, vec3 normal ) {
    return vec - normal * dot( vec, normal );
}

vec3 rotate (vec3 v, vec3 n, float a) {
    return v * cos(a) + cross(n, v) * sin(a) + n * dot(n, v) * (1. - cos(a));
}

void vertex() {
    vec3 obj_pos = (MODEL_MATRIX * vec4(0, 0, 0, 1)).xyz;
    vec3 world_camera = (VIEW_MATRIX * vec4(0.0, 0.0, 0.0, 1.0)).xyz;
    vec3 c = world_camera - obj_pos;
    c = normalize(c);
    c = (vec4(c, 0.0) * MODEL_MATRIX).xyz;
    vec3 axis = vec3(0, 1, 0);
    vec3 target = project_on_plane(c, axis);
    target = normalize(target);
    float dot_product = dot(target, NORMAL);
    float angle = acos( dot_product);
    if (target.x < 0.0) {
        angle = -angle;
    }
    VERTEX = rotate(VERTEX, axis, angle);
    NORMAL = rotate(NORMAL, axis, angle);
}

void fragment() {
    vec2 base_uv = UV;
    vec4 albedo_tex = texture(texture_albedo,base_uv + vec2(TIME * 0.02f));
	vec4 alphaN = texture(texture_alphaN,base_uv + vec2(TIME * -0.01f));
	vec4 alphaL = texture(texture_alpha,base_uv );
    ALBEDO = albedo.rgb * albedo_tex.rgb;
    ALPHA = albedo.a * albedo_tex.a * (alphaN.r * 2.0) * alphaL.r * 0.2;
	EMISSION = albedo.rgb * albedo_tex.rgb;
    float depth_tex = textureLod(DEPTH_TEXTURE,SCREEN_UV,0.0).r;
    vec4 world_pos = INV_PROJECTION_MATRIX * vec4(SCREEN_UV*2.0-1.0,depth_tex*2.0-1.0,1.0);
    world_pos.xyz/=world_pos.w;
    ALPHA*=clamp(1.0-smoothstep(world_pos.z+proximity_fade_distance,world_pos.z,VERTEX.z),0.0,1.0);
}
