shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, filter_nearest;

void fragment() {
	vec2 uv = SCREEN_UV;
	uv = sin(uv.y * SCREEN_PIXEL_SIZE.y);
	vec3 c = textureLod(screen_texture, uv, 1.0).rgb;
	
	COLOR.rgb = c;
}
