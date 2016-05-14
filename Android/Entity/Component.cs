using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace mapKnight.Android.Entity {
    public abstract class Component {
        protected Entity Owner;

        public object State;

        public Component (Entity owner) {
            this.Owner = owner;
            this.State = null;
        }

        public virtual void Update (float dt) {

        }

        public override string ToString () {
            return this.GetType ().Name;
        }

        public enum Type {
            Animation,
            Collision,
            Draw,
            Gravity,
            Motion,
            Push,
            Skelet,
            Sprite,
            Texture
        }

        public abstract class Config {
            public Type Type { get { return (Type)Enum.Parse (typeof (Type), this.GetType ().Name.Replace ("ComponentConfig", "")); } }
            public abstract Component Create (Entity owner);
            // used for sorting
            // -2 is last called, 2 is first
            public abstract int Priority { get; }
        }

        public class Comparer : IComparer<Component.Config> {
            public int Compare (Config x, Config y) {
                if (x.Priority > y.Priority) {
                    return -1; // the ones with the higher priority need to be called first
                } else if (x.Priority < y.Priority) {
                    return 1;
                } else {
                    return 0;
                }
            }
        }

        public class Binder : SerializationBinder {
            public override System.Type BindToType (string assemblyName, string typeName) {
                System.Type resolvingType = System.Type.GetType ($"mapKnight.Android.Entity.Components.Configs.{typeName}ComponentConfig");
                return resolvingType;
            }

            public override void BindToName (System.Type serializedType, out string assemblyName, out string typeName) {
                assemblyName = null;
                typeName = serializedType.Name;
            }
        }

        public enum Action {
            None,
            Result,

            //////////////////////////////////////////////////////////////////////////////////////////////
            // graphics
            VertexData,
            TextureData,
            Animation,

            //////////////////////////////////////////////////////////////////////////////////////////////
            // logic

            //////////////////////////////////////////////////////////////////////////////////////////////
            // physics
            Velocity,
            Acceleration
        }

        public struct Info {
            public Action Action;
            public Type Sender;
            public object Data;
        }
    }
}