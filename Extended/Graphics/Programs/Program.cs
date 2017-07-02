using System;
using mapKnight.Core;
using mapKnight.Extended.Graphics.Buffer;
using mapKnight.Extended.Graphics.Handle;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended.Graphics.Programs {
    public abstract class Program : IDisposable {
        protected int glProgram;

        private AttributeHandle positionHandle;

        public Program (string vertexShader, string fragmentShader) {
            glProgram = CompileProgram(vertexShader, fragmentShader);

            positionHandle = new AttributeHandle(glProgram, "a_position");
        }

        public virtual void Begin ( ) {
            GL.UseProgram(glProgram);
            positionHandle.Enable( );
        }

        public virtual void End ( ) {
            positionHandle.Disable( );
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        protected void Apply (IAttributeBuffer vertexbuffer, bool alphaBlending) {
            if (alphaBlending)
                EnableAlphaBlending( );
            vertexbuffer.Bind(positionHandle);
        }

        public void Dispose ( ) {
            GL.DeleteProgram(glProgram);
        }

        private void EnableAlphaBlending ( ) {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        private static int CompileProgram (string vertexShader, string fragmentShader) {
            int program = GL.CreateProgram( );

            GL.AttachShader(program, Assets.GetVertexShader(vertexShader));
            GL.AttachShader(program, Assets.GetFragmentShader(fragmentShader));

            GL.LinkProgram(program);

            string log = GL.GetProgramInfoLog(program);
#if DEBUG
            Debug.Print(typeof(Program), $"program { program } loaded");
            if (!string.IsNullOrWhiteSpace(log))
                Debug.Print(typeof(Program), "log: " + log);
            Debug.CheckGL(typeof(Program));
#endif

            return program;
        }
    }
}
