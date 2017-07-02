precision mediump float;

uniform sampler2D u_texture;
uniform float u_brightness;

varying vec2 v_texcoord;

void main()
{
	gl_FragColor = texture2D(u_texture, v_texcoord);

    gl_FragColor.a = 1.0 - gl_FragColor.a * u_brightness;
}
