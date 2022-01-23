shader_type canvas_item;

uniform bool active = false;

void fragment() {
	vec4 old_color = texture(TEXTURE, UV);
	COLOR = (!active)
		? vec4(1.0, old_color.g, old_color.b, old_color.a)
		: vec4(1.0, 1.0, 1.0, old_color.a);
}
