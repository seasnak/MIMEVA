shader_type canvas_item;

// uniform sampler2D screen_texture : hint_screen_texture, filter_nearest;

uniform bool active = false;

void fragment() {
	
    vec4 prev_color = texture(TEXTURE, UV);
    
	vec4 new_color;
    
    if (active == true) {
        new_color = vec4(1.0, 1.0, 1.0, prev_color.a);
    }
    else {
        new_color = prev_color;
    }

    COLOR = new_color;

}