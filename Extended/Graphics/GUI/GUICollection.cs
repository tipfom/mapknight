using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace mapKnight.Extended.Graphics.GUI {
    public class GUICollection : IEnumerable, IEnumerator {
        private List<GUIItem> items;
        public bool Active { get; private set; } = false;
        private int crawlingIndex = -1;

        public GUICollection (Screen parent) {
            items = new List<GUIItem>( );
            parent.ActivationChanged += ( ) => {
                Active = parent.Active;
                if (parent.Active)
                    GUIRenderer.Target = this;
            };
        }

        public T Add<T> (T item) where T : GUIItem {
            if (!items.Contains(item)) {
                items.Add(item);
                item.Changed += (sender) => {
                    if (Active)
                        GUIRenderer.Update(sender);
                };
            }
            return item;
        }

        public void Update (TimeSpan time) {
            foreach (GUIItem item in this)
                item.Update(time);
        }

        public IEnumerator GetEnumerator ( ) {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator ( ) {
            return GetEnumerator( );
        }

        public GUIItem Current {
            get {
                return items[crawlingIndex];
            }
        }

        object IEnumerator.Current {
            get {
                return items[crawlingIndex];
            }
        }
        
        public bool MoveNext ( ) {
            crawlingIndex++;
            return (crawlingIndex < items.Count);
        }

        internal IEnumerable<GUIItem> FindAll (Func<GUIItem, bool> p) {
            return items.FindAll(item => p(item));
        }

        public void Reset ( ) {
            crawlingIndex = -1;
        }
    }
}
