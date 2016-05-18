using Java.Nio;
using System;
using System.Collections.Generic;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android.CGL.Programs {
    public class EntityProgram {
        private int vertexShader;
        private int fragmentShader;

        private int program;
        private int positionHandle;
        private int textureUniformHandle;
        private int textureCoordinateHandle;
        private int mvpMatrixHandle;

        public EntityProgram () {
            // get shader
            vertexShader = ProgramHelper.GetVertexShader ("matrix");
            fragmentShader = ProgramHelper.GetFragmentShader ("normal");
            program = ProgramHelper.CreateProgram (vertexShader, fragmentShader);

            // get handles
            positionHandle = GL.GlGetAttribLocation (program, "a_position");
            textureUniformHandle = GL.GlGetUniformLocation (program, "u_texture");
            textureCoordinateHandle = GL.GlGetAttribLocation (program, "a_texcoord");
            mvpMatrixHandle = GL.GlGetUniformLocation (program, "u_mvpmatrix");
        }

        public void Draw (FloatBuffer vertexBuffer, FloatBuffer textureBuffer, ShortBuffer indexBuffer, int texture, int bufferOffset, int quadCount, bool alphaBlending = true) {
            indexBuffer.Position (bufferOffset * 6);

            // set texture
            GL.GlActiveTexture (GL.GlTexture0);
            GL.GlBindTexture (GL.GlTexture2d, texture);
            GL.GlUniform1i (textureUniformHandle, 0);
            // set matrix
            GL.GlUniformMatrix4fv (mvpMatrixHandle, 1, false, Screen.DefaultMatrix.MVP, 0);
            // set texture buffer
            GL.GlVertexAttribPointer (textureCoordinateHandle, 2, GL.GlFloat, false, 0, textureBuffer);
            // set vertex buffer
            GL.GlVertexAttribPointer (positionHandle, 2, GL.GlFloat, false, 2 * sizeof (float), vertexBuffer);
            // enable alphablending if wanted
            if (alphaBlending) {
                GL.GlEnable (GL.GlBlend);
                GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);
            }
            //draw
            GL.GlDrawElements (GL.GlTriangles, quadCount * 6, GL.GlUnsignedShort, indexBuffer);
        }

        public void StreamDraw (FloatBuffer vertexBuffer, FloatBuffer textureBuffer, ShortBuffer indexBuffer, List<Tuple<int, int>> streamData, bool alphaBlending = true) {
            // set matrix
            GL.GlUniformMatrix4fv (mvpMatrixHandle, 1, false, Screen.DefaultMatrix.MVP, 0);
            // set texture buffer
            GL.GlVertexAttribPointer (textureCoordinateHandle, 2, GL.GlFloat, false, 0, textureBuffer);
            // set vertex buffer
            GL.GlVertexAttribPointer (positionHandle, 2, GL.GlFloat, false, 2 * sizeof (float), vertexBuffer);
            // enable alphablending if wanted
            if (alphaBlending) {
                GL.GlEnable (GL.GlBlend);
                GL.GlBlendFunc (GL.GlSrcAlpha, GL.GlOneMinusSrcAlpha);
            }

            int overallOffset = 0;
            foreach (Tuple<int, int> drawData in streamData) {
                // set texture
                GL.GlActiveTexture (GL.GlTexture0);
                GL.GlBindTexture (GL.GlTexture2d, drawData.Item1);
                GL.GlUniform1i (textureUniformHandle, 0);

                indexBuffer.Position (overallOffset * 6);
                overallOffset += drawData.Item2;
                GL.GlDrawElements (GL.GlTriangles, drawData.Item2 * 6, GL.GlUnsignedShort, indexBuffer);
            }
        }

        public void Begin () {
            GL.GlUseProgram (program);
            GL.GlEnableVertexAttribArray (positionHandle);
            GL.GlEnableVertexAttribArray (textureCoordinateHandle);
        }

        public void End () {
            GL.GlDisableVertexAttribArray (positionHandle);
            GL.GlDisableVertexAttribArray (textureCoordinateHandle);
        }
    }
}