using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Graphics;

namespace mapKnight.ToolKit {
    public static class Extensions {
        public static BitmapImage ToBitmapImage (this Texture2D texture) {
            using (MemoryStream ms = new MemoryStream( )) {
                texture.SaveAsPng(ms, texture.Width, texture.Height);

                BitmapImage result = new BitmapImage( );
                result.BeginInit( );
                result.StreamSource = ms;
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.EndInit( );
                return result;
            }
        }

        public static void SaveToStream (this BitmapImage image, Stream stream) {
            PngBitmapEncoder encoder = new PngBitmapEncoder( );
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);
        }

        public static Texture2D ToTexture2D (this BitmapImage image, GraphicsDevice g) {
            using (MemoryStream ms = new MemoryStream( )) {
                image.SaveToStream(ms);

                return Texture2D.FromStream(g, ms);
            }
        }

        public static IEnumerable<T> FindDescendants<T> (this DependencyObject parent, Func<T, bool> predicate, bool deepSearch = false) where T : DependencyObject {
            var children = LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>( ).ToList( );

            foreach (var child in children) {
                var typedChild = child as T;
                if ((typedChild != null) && (predicate == null || predicate.Invoke(typedChild))) {
                    yield return typedChild;
                    if (deepSearch) foreach (var foundDescendant in FindDescendants(child, predicate, true)) yield return foundDescendant;
                } else {
                    foreach (var foundDescendant in FindDescendants(child, predicate, deepSearch)) yield return foundDescendant;
                }
            }

            yield break;
        }

        public static TreeViewItem FindContainer (this TreeView treeview, object item) {
            return (TreeViewItem)treeview.ItemContainerGenerator.FindContainer(item);
        }

        private static TreeViewItem FindContainer (this ItemContainerGenerator containerGenerator, object item) {
            TreeViewItem container = (TreeViewItem)containerGenerator.ContainerFromItem(item);
            if (container != null)
                return container;

            foreach (object childItem in containerGenerator.Items) {
                TreeViewItem parent = containerGenerator.ContainerFromItem(childItem) as TreeViewItem;
                if (parent == null)
                    continue;

                container = parent.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (container != null)
                    return container;

                container = parent.ItemContainerGenerator.FindContainer(item);
                if (container != null)
                    return container;
            }
            return null;
        }

        public static void AddRange<T> (this ObservableCollection<T> collection, IEnumerable<T> items) {
            foreach (T item in items) collection.Add(item);
        }

        public static Point ToPoint (this Vector vector) {
            return new Point(vector.X, vector.Y);
        }
    }
}
