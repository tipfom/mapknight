precision mediump float;

uniform sampler2D u_texture;
uniform vec2 u_pixel_offset;

varying vec2 v_texcoord;

void main()
{
    vec4 b0 = texture2D(u_texture, v_texcoord - u_pixel_offset);
    vec4 b1 = texture2D(u_texture, v_texcoord - u_pixel_offset * 2.0);
    vec4 b2 = texture2D(u_texture, v_texcoord - u_pixel_offset * 3.0);
    vec4 b3 = texture2D(u_texture, v_texcoord - u_pixel_offset * 4.0);
    vec4 b4 = texture2D(u_texture, v_texcoord - u_pixel_offset * 5.0);
    vec4 b5 = texture2D(u_texture, v_texcoord - u_pixel_offset * 6.0);
    vec4 b6 = texture2D(u_texture, v_texcoord - u_pixel_offset * 7.0);
    vec4 b7 = texture2D(u_texture, v_texcoord - u_pixel_offset * 8.0);
    
    vec4 c = texture2D(u_texture, v_texcoord);

    vec4 a0 = texture2D(u_texture, v_texcoord + u_pixel_offset);
    vec4 a1 = texture2D(u_texture, v_texcoord + u_pixel_offset * 2.0);
    vec4 a2 = texture2D(u_texture, v_texcoord + u_pixel_offset * 3.0);
    vec4 a3 = texture2D(u_texture, v_texcoord + u_pixel_offset * 4.0);
    vec4 a4 = texture2D(u_texture, v_texcoord + u_pixel_offset * 5.0);
    vec4 a5 = texture2D(u_texture, v_texcoord + u_pixel_offset * 6.0);
    vec4 a6 = texture2D(u_texture, v_texcoord + u_pixel_offset * 7.0);
    vec4 a7 = texture2D(u_texture, v_texcoord + u_pixel_offset * 8.0);

	gl_FragColor = 
        0.014076 * (a7 + b7) +
        0.022439 * (a6 + b6) +
        0.033613 * (a5 + b5) + 
        0.047318 * (a4 + b4) + 
        0.062595 * (a3 + b3) +
        0.077812 * (a2 + b2) + 
        0.090898 * (a1 + b1) +
        0.099783 * (a0 + b0) +
        0.102934 * c;
}
