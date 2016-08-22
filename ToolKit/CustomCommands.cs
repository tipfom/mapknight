using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace mapKnight.ToolKit {

    public class CustomCommands {

        public static readonly RoutedUICommand EditorDelete = new RoutedUICommand(
            "EditorDelete",
            "EditorDelete",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.Delete, ModifierKeys.Control |ModifierKeys.Shift)
            }
            );

        public static readonly RoutedUICommand EditorNew = new RoutedUICommand(
                    "EditorNew",
            "EditorNew",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.N, ModifierKeys.Control |ModifierKeys.Shift)
            }
            );

        public static readonly RoutedUICommand EditorOpen = new RoutedUICommand(
            "EditorOpen",
            "EditorOpen",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.O, ModifierKeys.Control |ModifierKeys.Shift)
            }
            );

        public static readonly RoutedUICommand EditorUp = new RoutedUICommand(
            "EditorUp",
            "EditorUp",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.Up, ModifierKeys.Alt |ModifierKeys.Shift)
            }
            );

        public static readonly RoutedUICommand EditorDown = new RoutedUICommand(
            "EditorDown",
            "EditorDown",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.Down, ModifierKeys.Alt |ModifierKeys.Shift)
            }
            );

        public static readonly RoutedUICommand EditorR = new RoutedUICommand(
            "EditorR",
            "EditorR",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.R, ModifierKeys.Control |ModifierKeys.Shift)
            }
            );
    }
}