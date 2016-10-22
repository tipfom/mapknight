uniform mat4 u_mvpmatrix;

attribute vec4 a_position; 
attribute vec2 a_size;
attribute vec4 a_color;

varying vec4 v_color;

void main()
{
	v_color = a_color;
	gl_Position = u_mvpmatrix * a_position;
	gl_PointSize = a_size.x;
}