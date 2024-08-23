shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, filter_nearest;

void fragment() {
	// Sample the texture
	vec3 color = texture(screen_texture, UV).rgb;
	// Set fragment color
	COLOR.rgb = color;
}