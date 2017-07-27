uniform mat4 u_mvpmatrix;

attribute vec4 a_position;

void main()
{
	gl_Position = u_mvpmatrix * a_position;
}