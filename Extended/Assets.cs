using System;
using System.Collections.Generic;
using System.IO;
using mapKnight.Core;
using mapKnight.Extended.Graphics;
using mapKnight.Extended.Graphics.Programs;
using Newtonsoft.Json;
using OpenTK.Graphics.ES20;
using Path = System.IO.Path;
using mapKnight.Extended.Graphics.Particles;

#if __ANDROID__
using Android.Content;
using Android.Graphics;
using Android.Opengl;
using Android.Gestures;
#endif

namespace mapKnight.Extended {
    public static class Assets {
#if __ANDROID__
        public static Context Context { get; set; }
#endif

        public static T Load<T> (params string[ ] paths) {
            Type request = typeof(T);
            if (request == typeof(Texture2D) && paths.Length == 1) {
                return (T)((object)GetTexture("textures", paths[0] + ".png"));
            } else if (request == typeof(string)) {
                return (T)((object)GetText(Path.Combine(paths)));
            } else if (request == typeof(SpriteBatch) && paths.Length == 1) {
                return (T)((object)GetSprite(paths[0]));
            } else if (request == typeof(Entity.Configuration) && paths.Length == 1) {
                return (T)((object)GetEntityConfig(paths[0]));
            } else if (request == typeof(Graphics.Map) && paths.Length == 1) {
                return (T)((object)GetMap(paths[0]));
            } else if (request == typeof(Emitter) && paths.Length == 1) {
                return (T)((object)GetEmitter(paths[0]));
#if __ANDROID__
            } else if (request == typeof(GestureStore) && paths.Length == 1) {
                return (T)((object)GetGestureStore(paths[0]));
#endif
            } else {
                throw new TypeLoadException($"requested filetype { request.FullName } couldnt be loaded with { paths.Length } parameters");
            }
        }

        public static Emitter GetEmitter (string path) {
            Emitter emitter = JsonConvert.DeserializeObject<Emitter>(GetText("paticlesystems", path));
            emitter.Setup( );
            return emitter;
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

        static Dictionary<int, Texture2D> textureCache = new Dictionary<int, Texture2D>( );
        public static Texture2D GetTexture (params string[ ] path) {
            int pathhash = path.GetHashCode( );
            if (!textureCache.ContainsKey(pathhash) || textureCache[pathhash].Disposed) {
                int width, height, id = GL.GenTexture( );
                GL.BindTexture(TextureTarget.Texture2D, id);
#if __ANDROID__
                using (BitmapFactory.Options options = new BitmapFactory.Options( ) { InScaled = false })
                using (Stream stream = GetStream(path))
                using (Bitmap bitmap = BitmapFactory.DecodeStream(stream, null, options)) {
                    GLUtils.TexImage2D((int)TextureTarget.Texture2D, 0, bitmap, 0);
                    width = bitmap.Width;
                    height = bitmap.Height;
                }
#endif

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);

                if (!textureCache.ContainsKey(pathhash)) {
                    textureCache.Add(pathhash, new Texture2D(id, new Size(width, height), path[path.Length - 1]));
                } else {
                    textureCache[pathhash] = new Texture2D(id, new Size(width, height), path[path.Length - 1]);
                }
#if DEBUG
                if (id == 0 || Debug.CheckGL(typeof(Assets)))
                    Debug.Print(typeof(Assets), "failed loading image " + path[path.Length - 1]);
                else
                    Debug.Print(typeof(Assets), "loaded image " + path[path.Length - 1]);
#endif
            }
            return textureCache[pathhash];
        }

        static Dictionary<int, SpriteBatch> spriteCache = new Dictionary<int, SpriteBatch>( );
        public static SpriteBatch GetSprite (string name) {
            int namehash = name.GetHashCode( );
            if (!spriteCache.ContainsKey(namehash)) {
                Texture2D texture = Assets.Load<Texture2D>(name);
                Dictionary<string, int[ ]> spriteContent = new Dictionary<string, int[ ]>( );
                JsonConvert.PopulateObject(GetText("textures", name + ".json"), spriteContent);
                spriteCache.Add(namehash, new SpriteBatch(spriteContent, texture));
            }
            return spriteCache[namehash];
        }

        public static string GetText (params string[ ] path) {
            using (StreamReader reader = new StreamReader(GetStream(path))) {
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

#if DEBUG
            string log = GL.GetShaderInfoLog(shader);
            Debug.Print(typeof(Assets), $"vertexshader {shader} loaded");
            if (!string.IsNullOrWhiteSpace(log))
                Debug.Print(typeof(Assets), "log: " + log);
            Debug.CheckGL(typeof(Assets));
#endif

            return shader;
        }

        private static Dictionary<string, int> loadedFragmentShader = new Dictionary<string, int>( );
        public static int GetFragmentShader (string name) {
            if (loadedFragmentShader.ContainsKey(name))
                return loadedFragmentShader[name];
            int shader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(shader, GetText("shader", "fragment", name + ".txt"));
            GL.CompileShader(shader);

#if DEBUG
            string log = GL.GetShaderInfoLog(shader);
            Debug.Print(typeof(Assets), $"fragmentshader {shader} loaded");
            if (!string.IsNullOrWhiteSpace(log))
                Debug.Print(typeof(Assets), "log: " + log);
            Debug.CheckGL(typeof(Assets));
#endif

            return shader;
        }

        public static Graphics.Map GetMap (string name) {
            return new Graphics.Map(GetStream("maps", name + ".map"));
        }

        private static JsonSerializerSettings entitySerializerSettings = new JsonSerializerSettings( ) {
            TypeNameHandling = TypeNameHandling.Auto,
            Binder = Component.SerializationBinder
        };

        public static Entity.Configuration GetEntityConfig (string name) {
            return JsonConvert.DeserializeObject<Entity.Configuration>(GetText("entities", $"{name}.json"), entitySerializerSettings);
        }

        public static Stream GetStream (params string[ ] path) {
#if __ANDROID__
            return Context.Assets.Open(Path.Combine(path));
#endif
        }

#if __ANDROID__
        public static GestureStore GetGestureStore (string name) {
            GestureStore store = new GestureStore( );
            store.Load(GetStream("gestures", name), true);
            return store;
        }
#endif
    }
}
