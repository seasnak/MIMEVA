shader_type canvas_item;

uniform float TIME;
uniform vec2 SCREEN_SIZE;

void fragment() {
	// Sample the texture
	vec3 color = texture(SCREEN_TEXTURE, UV).rgb;

	// Calculate scanline intensity based on time and UV.y
	float scanline = sin(TIME * 100.0 + UV.y * 20.0) * 0.2 + 0.8;

	// Apply scanline intensity to color
	color *= scanline;

	// Brighten the top and bottom edges slightly
	color.rgb += vec3(0.02, 0.02, 0.02) * smoothstep(0.02, 0.05, UV.y) * smoothstep(0.95, 0.98, UV.y);

	// Set fragment color
	emit fragment_color = vec4(color, 1.0);
}