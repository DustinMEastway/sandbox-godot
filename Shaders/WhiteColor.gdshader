shader_type canvas_item;

uniform bool active = false;

void fragment() {
	vec4 old_color = texture(TEXTURE, UV);
	COLOR = (!active) ? old_color : vec4(1.0, 1.0, 1.0, old_color.a);
}
