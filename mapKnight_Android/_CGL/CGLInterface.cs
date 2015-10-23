using System.Linq;

using GL = Android.Opengl.GLES20;

using Java.Nio;

namespace mapKnight_Android
{
	namespace CGL{
		public class CGLInterface
		{
			FloatBuffer VertexBuffer;
			IntBuffer IndexBuffer;
			FloatBuffer TextureBuffer;

			int RenderProgram;

			private int framebufferID;
			private int framebufferTexture;
			private int renderbufferID;
		
			public CGLInterface (Size ScreenSize)
			{
				RenderProgram = CGLTools.LoadProgram (GlobalContent.FragmentShaderN, GlobalContent.VertexShaderN);
			}

			public void Render()
			{
				
			}
		}
	}
}

