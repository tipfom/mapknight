using mapKnight.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace mapKnight.ToolKit {
    public class Project {
        public string Path { get; private set; } = null;
        public bool IsLocated { get { return Path != null; } }

        public event Action<Map> MapAdded;
        private List<Map> maps = new List<Map>( );

        public Project ( ) {

        }

        public Project (string path) {
            Path = path;
            ExportedProject savedProject = JsonConvert.DeserializeObject<ExportedProject>(File.ReadAllText(path));

            foreach (string map in savedProject.Maps) {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(map))) {
                    AddMap(new Map(ms));
                }
            }
        }

        public void Save ( ) {
            Save(Path);
        }

        public void Save (string path) {
            ExportedProject exportedProject = new ExportedProject( ) {
                Maps = new List<string>( )
            };

            foreach (Map map in maps) {
                map.Texture = "";
                exportedProject.Maps.Add(map.Flush( ));
            }

            using (Stream saveStream = File.Open(path, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(saveStream)) {
                writer.WriteLine(JsonConvert.SerializeObject(exportedProject));
            }

            MessageBox.Show("Completed!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void AddMap (Map map) {
            maps.Add(map);
            MapAdded?.Invoke(map);
        }

        public IEnumerable<Map> GetMaps ( ) {
            return maps;
        }

        public static bool Validate (string path) {
            return false;
        }

        private struct ExportedProject {
            public List<string> Maps;
        }
    }
}
