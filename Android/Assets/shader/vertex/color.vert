 precision mediump float;

uniform mat4 u_mvpmatrix;

attribute vec4 a_color;
attribute vec4 a_position;
attribute vec2 a_texcoord;

varying vec2 v_texcoord;
varying vec4 v_color;

void main()
{
	v_color = a_color;
	v_texcoord = a_texcoord;
	gl_Position = u_mvpmatrix * a_position;
}