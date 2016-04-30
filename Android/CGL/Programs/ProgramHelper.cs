using System.Collections.Generic;
using System.IO;
using GL = Android.Opengl.GLES20;

namespace mapKnight.Android.CGL.Programs {
    static class ProgramHelper {
        public const string VERTEX_SHADER_LOCATION = @"shader/vertex";
        public const string FRAGMENT_SHADER_LOCATION = @"shader/fragment";

        private static Dictionary<string, int> loadedShader = new Dictionary<string, int> ( );

        public static void Load () {
            // load all vertex shader
            foreach (string vertexshaderpath in Content.Context.Assets.List (VERTEX_SHADER_LOCATION)) {
                string shadername = Path.GetFileNameWithoutExtension (vertexshaderpath);
                using (StreamReader filereader = new StreamReader (Content.Context.Assets.Open (Path.Combine (VERTEX_SHADER_LOCATION, vertexshaderpath)))) {
                    loadedShader.Add ($"vertex_{shadername}", LoadShader (GL.GlVertexShader, filereader.ReadToEnd ( )));
                }
            }

            // load all fragment shader
            foreach (string fragmentshaderpath in Content.Context.Assets.List (FRAGMENT_SHADER_LOCATION)) {
                string shadername = Path.GetFileNameWithoutExtension (fragmentshaderpath);
                using (StreamReader filereader = new StreamReader (Content.Context.Assets.Open (Path.Combine (FRAGMENT_SHADER_LOCATION, fragmentshaderpath)))) {
                    loadedShader.Add ($"fragment_{shadername}", LoadShader (GL.GlFragmentShader, filereader.ReadToEnd ( )));
                }
            }
        }

        private static int LoadShader (int type, string code) {
            int shader = GL.GlCreateShader (type);

            GL.GlShaderSource (shader, code);
            GL.GlCompileShader (shader);

            Log.Print (typeof (ProgramHelper), "Loaded new shader (id = " + shader.ToString ( ) + ")");
            Log.Print (typeof (ProgramHelper), "Log = " + GL.GlGetShaderInfoLog (shader));

            return shader;
        }

        public static int GetVertexShader (string name) {
            return loadedShader[$"vertex_{ name }"];
        }

        public static int GetFragmentShader (string name) {
            return loadedShader[$"fragment_{ name }"];
        }

        public static int CreateProgram (params int[ ] shader) {
            int program = GL.GlCreateProgram ( );

            for (int i = 0; i < shader.Length; i++) {
                GL.GlAttachShader (program, shader[i]);
            }

            GL.GlLinkProgram (program);

            Log.Print (typeof (ProgramHelper), "Loaded new program (id = " + program.ToString ( ) + ")");
            Log.Print (typeof (ProgramHelper), "Log = " + GL.GlGetProgramInfoLog (program));

            return program;
        }
    }
}