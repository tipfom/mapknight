using System;
using System.Collections.Generic;
using System.IO;
using mapKnight.Core;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.Programs;
using Newtonsoft.Json;
using OpenTK.Graphics.ES20;

namespace mapKnight.Extended {
    public static class Assets {
        public static IAssetProvider AssetProvider;

        public static T Load<T> (params string[ ] paths) {
            Type request = typeof(T);
            if (request == typeof(Texture2D) && paths.Length == 1) {
                return (T)((object)GetTexture(paths[0]));
            } else if (request == typeof(string)) {
                return (T)((object)GetText(Path.Combine(paths)));
            } else if (request == typeof(SpriteBatch) && paths.Length == 1) {
                return (T)((object)GetSprite(paths[0]));
            } else if (request == typeof(Entity.Configuration) && paths.Length == 1) {
                return (T)((object)GetEntityConfig(paths[0]));
            } else if (request == typeof(Graphics.Map) && paths.Length == 1) {
                return (T)((object)GetMap(paths[0]));
            } else {
                throw new TypeLoadException($"requested filetype { request.FullName } couldnt be loaded with { paths.Length } parameters");
            }
        }

        public static void Destroy ( ) {
            foreach (Texture2D texture in textureCache.Values)
                texture.Dispose( );
            textureCache = null;

            ColorProgram.Program.Dispose( );
            MatrixProgram.Program.Dispose( );
            foreach (int shader in loadedVertexShader.Values)
                GL.DeleteShader(shader);
            loadedVertexShader = null;
            foreach (int shader in loadedFragmentShader.Values)
                GL.DeleteShader(shader);
            loadedFragmentShader = null;
        }

        static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>( );
        public static Texture2D GetTexture (string name) {
            if (!textureCache.ContainsKey(name))
                textureCache.Add(name, AssetProvider.GetTexture(name + ".png"));
            return textureCache[name];
        }

        public static SpriteBatch GetSprite (string name) {
            Texture2D texture = GetTexture(name);
            Dictionary<string, int[ ]> spriteContent = new Dictionary<string, int[ ]>( );
            JsonConvert.PopulateObject(GetText("textures", name + ".json"), spriteContent);
            return new SpriteBatch(spriteContent, texture);
        }

        public static string GetText (params string[ ] path) {
            using (StreamReader reader = new StreamReader(AssetProvider.GetStream(path))) {
                return reader.ReadToEnd( );
            }
        }

        private static Dictionary<string, int> loadedVertexShader = new Dictionary<string, int>( );
        public static int GetVertexShader (string name) {
            if (loadedVertexShader.ContainsKey(name))
                return loadedVertexShader[name];
            int shader = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(shader, GetText("shader", "vertex", name + ".txt"));
            GL.CompileShader(shader);

            string log = GL.GetShaderInfoLog(shader);
            Log.Print(typeof(Assets), "Loaded new shader (id = " + shader.ToString( ) + ")");
            Log.Print(typeof(Assets), "Log = " + log);
            Log.Print(typeof(Assets), "GL.GLGetError returned " + GL.GetErrorCode( ).ToString( ));

            return shader;
        }

        private static Dictionary<string, int> loadedFragmentShader = new Dictionary<string, int>( );
        public static int GetFragmentShader (string name) {
            if (loadedFragmentShader.ContainsKey(name))
                return loadedFragmentShader[name];
            int shader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(shader, GetText("shader", "fragment", name + ".txt"));
            GL.CompileShader(shader);

            string log = GL.GetShaderInfoLog(shader);
            Log.Print(typeof(Assets), "Loaded new shader (id = " + shader.ToString( ) + ")");
            Log.Print(typeof(Assets), "Log = " + log);
            Log.Print(typeof(Assets), "GL.GLGetError returned " + GL.GetErrorCode( ).ToString( ));

            return shader;
        }

        public static Graphics.Map GetMap (string name) {
            return new Graphics.Map(AssetProvider.GetStream("maps", name + ".map"));
        }

        private static JsonSerializerSettings entitySerializerSettings = new JsonSerializerSettings( ) {
            TypeNameHandling = TypeNameHandling.Auto,
            Binder = Component.SerializationBinder
        };

        public static Entity.Configuration GetEntityConfig (string name) {
            return JsonConvert.DeserializeObject<Entity.Configuration>(GetText("entities", $"{name}.json"), entitySerializerSettings);
        }

        public interface IAssetProvider {
            Stream GetStream (params string[ ] path);
            Texture2D GetTexture (string name);
        }
    }
}
