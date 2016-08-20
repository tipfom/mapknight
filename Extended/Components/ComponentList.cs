using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mapKnight.Extended.Components.Attributes;
using mapKnight.Extended.Exceptions;

namespace mapKnight.Extended.Components {

    public class ComponentList : ICollection<Component.Configuration> {
        private bool _HasChanged;
        private List<Component.Configuration> components = new List<Component.Configuration>( );

        public int Count { get { return components.Count; } }

        public bool HasChanged { get { if (_HasChanged) { _HasChanged = false; return true; } return false; } }
        public bool IsReadOnly { get { return false; } }

        public Component.Configuration this[int index] {
            get { return components[index]; }
        }

        public void Add (Component.Configuration item) {
            components.Add(item);
            Sort( );
            _HasChanged = true;
        }

        public void Clear ( ) {
            components.Clear( );
        }

        public bool Contains (Component.Configuration item) {
            return components.Contains(item);
        }

        public void CopyTo (Component.Configuration[ ] array, int arrayIndex) {
            components.CopyTo(array, arrayIndex);
        }

        public T GetConfiguration<T> ( ) where T : Component.Configuration {
            Type type = typeof(T);
            return (T)components.Find(c => c.GetType( ) == type);
        }

        public IEnumerator<Component.Configuration> GetEnumerator ( ) {
            return components.GetEnumerator( );
        }

        IEnumerator IEnumerable.GetEnumerator ( ) {
            return components.GetEnumerator( );
        }

        public bool Remove (Component.Configuration item) {
            return components.Remove(item);
        }

        public void ResolveComponentDependencies ( ) {
            HashSet<Type> instanciatedTypes = new HashSet<Type>(components.Select(config => config.GetType( )));
            for (int i = 0; i < components.Count; i++) {
                Type componentType = Type.GetType(components[i].GetType( ).FullName.Replace("+Configuration", ""));
                ComponentRequirement[ ] requirements = (ComponentRequirement[ ])componentType.GetCustomAttributes(typeof(ComponentRequirement), false);
                foreach (ComponentRequirement requirement in requirements) {
                    Type componentConfigType = Type.GetType(requirement.Requiring.FullName + "+Configuration");
                    if (!instanciatedTypes.Contains(componentConfigType)) {
                        bool canBeInstanciated = requirement.Requiring.GetCustomAttributes(typeof(Instantiatable), false).Length > 0;
                        if (canBeInstanciated) {
                            components.Add((Component.Configuration)Activator.CreateInstance(componentConfigType));
                            instanciatedTypes.Add(componentConfigType);
                        } else
                            throw new ComponentDependencyException(components[i].Component, requirement.Requiring);
                    }
                }
            }
        }

        public void Sort ( ) {
            List<Node> nodeList = new List<Node>( );
            List<Component.Configuration> sortedList = new List<Component.Configuration>( );
            // structure into nodes
            for (int i = 0; i < components.Count; i++) {
                UpdateAfter[ ] afterAttribute = (UpdateAfter[ ])Type.GetType(components[i].GetType( ).FullName.Replace("+Configuration", "")).GetCustomAttributes(typeof(UpdateAfter), false);
                UpdateBefore[ ] beforeAttribute = (UpdateBefore[ ])Type.GetType(components[i].GetType( ).FullName.Replace("+Configuration", "")).GetCustomAttributes(typeof(UpdateBefore), false);

                if (beforeAttribute.Length + afterAttribute.Length == 0) {
                    nodeList.Add(new Node( ) { Item = components[i] });
                } else {
                    foreach (UpdateAfter attr in afterAttribute) {
                        Component.Configuration relation = components.Find(component => component.Component == attr.Relation);
                        nodeList.Add(new Node( ) { Item = components[i], Precursor = relation });
                    }
                    foreach (UpdateBefore attr in beforeAttribute) {
                        Component.Configuration relation = components.Find(component => component.Component == attr.Relation);
                        if (relation == null) {
                            nodeList.Add(new Node( ) { Item = components[i] });
                        } else {
                            nodeList.Add(new Node( ) { Item = relation, Precursor = components[i] });
                            nodeList.Add(new Node( ) { Item = components[i] });
                        }
                    }
                }
            }
            while (nodeList.Count > 0) {
                for (int i = 0; i < nodeList.Count; i++) {
                    bool isLastInChain = !nodeList.Any(node => node.Precursor == nodeList[i].Item);
                    if (isLastInChain) {
                        if (!sortedList.Contains(nodeList[i].Item))
                            sortedList.Add(nodeList[i].Item);
                        nodeList.RemoveAt(i);
                        break;
                    }
                }
            }
            components = sortedList;
        }

        private struct Node {
            public Component.Configuration Item { get; set; }
            public Component.Configuration Precursor { get; set; }
        }
    }
}