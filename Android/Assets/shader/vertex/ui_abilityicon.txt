uniform mat4 u_mvpMatrix;

attribute vec4 a_position;
attribute vec2 a_baseTexcoord;
attribute vec2 a_ampTexcoord;

varying vec2 v_baseTexcoord;
varying vec2 v_ampTexcoord;

void main()
{
    v_baseTexcoord = a_baseTexcoord;
	v_ampTexcoord = a_ampTexcoord;

	gl_Position = u_mvpMatrix * a_position;
}