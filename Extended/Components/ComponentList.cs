using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mapKnight.Extended.Exceptions;

namespace mapKnight.Extended.Components {
    public class ComponentList : ICollection<Component.Configuration> {
        private List<Component.Configuration> components = new List<Component.Configuration>( );

        public int Count { get { return components.Count; } }

        public bool IsReadOnly { get { return false; } }

        private bool _HasChanged;
        public bool HasChanged { get { if (_HasChanged) { _HasChanged = false; return true; } return false; } }

        public void Add (Component.Configuration item) {
            components.Add(item);
            Sort( );
            _HasChanged = true;
        }

        public Component.Configuration this[int index] {
            get { return components[index]; }
        }

        public void Sort ( ) {
            List<Node> nodeList = new List<Node>( );
            List<Component.Configuration> sortedList = new List<Component.Configuration>( );
            // structure into nodes
            for (int i = 0; i < components.Count; i++) {
                ComponentOrder[ ] orderAttributes = (ComponentOrder[ ])Type.GetType(components[i].GetType( ).FullName.Replace("+Configuration", "")).GetCustomAttributes(typeof(ComponentOrder), false);
                if (orderAttributes.Length == 0) {
                    nodeList.Add(new Node( ) { Item = components[i] });
                } else {
                    foreach (ComponentOrder order in orderAttributes) {
                        Component.Configuration relation = components.Find(component => component.Component == order.Relation);
                        if (order.Before) {
                            nodeList.Add(new Node( ) { Item = components[i], Precursor = relation });
                        } else {
                            nodeList.Add(new Node( ) { Item = relation, Precursor = components[i] });
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

        public void ResolveComponentDependencies ( ) {
            HashSet<Type> instanciatedTypes = new HashSet<Type>(components.Select(config => config.GetType( )));
            for (int i = 0; i < components.Count; i++) {
                Type componentType = Type.GetType(components[i].GetType( ).FullName.Replace("+Configuration", ""));
                ComponentRequirement[ ] requirements = (ComponentRequirement[ ])componentType.GetCustomAttributes(typeof(ComponentRequirement), false);
                foreach (ComponentRequirement requirement in requirements) {
                    Type componentConfigType = Type.GetType(requirement.Requiring.FullName + "+Configuration");
                    if (!instanciatedTypes.Contains(componentConfigType)) {
                        bool canBeInstanciated = requirement.Requiring.GetConstructor(new Type[ ] { typeof(Entity) }) != null;
                        if (canBeInstanciated) {
                            components.Add((Component.Configuration)Activator.CreateInstance(componentConfigType));
                            instanciatedTypes.Add(componentConfigType);
                        } else
                            throw new ComponentDependencyException(ComponentEnum.Animation, requirement.Requiring);
                    }
                }
            }
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

        public IEnumerator<Component.Configuration> GetEnumerator ( ) {
            return components.GetEnumerator( );
        }

        public bool Remove (Component.Configuration item) {
            return components.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator ( ) {
            return components.GetEnumerator( );
        }

        struct Node {
            public Component.Configuration Precursor { get; set; }
            public Component.Configuration Item { get; set; }
        }
    }
}
