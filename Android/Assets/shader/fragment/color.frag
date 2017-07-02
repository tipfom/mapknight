precision mediump float;

uniform sampler2D u_texture;

varying vec2 v_texcoord;
varying vec4 v_color;

void main()
{
	gl_FragColor = texture2D(u_texture, v_texcoord) * v_color;
}