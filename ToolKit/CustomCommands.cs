using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace mapKnight.ToolKit {

    public class CustomCommands {

        public static readonly RoutedUICommand Up = new RoutedUICommand(
            "Up",
            "Up",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.Up),
                new KeyGesture(Key.PageUp)
            }
            );

        public static readonly RoutedUICommand Down = new RoutedUICommand(
            "Down",
            "Down",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.Down),
                new KeyGesture(Key.PageDown)
            }
            );

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

        public static readonly RoutedUICommand EditorUndo = new RoutedUICommand(
            "EditorUndo",
            "EditorUndo",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.Z, ModifierKeys.Control)
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

        public static readonly RoutedUICommand EditorAltA = new RoutedUICommand(
            "EditorAltA",
            "EditorAltA",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.A, ModifierKeys.Alt)
            }
            );

        public static readonly RoutedUICommand EditorAltS = new RoutedUICommand(
            "EditorAltS",
            "EditorAltS",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.S, ModifierKeys.Alt)
            }
            );

        public static readonly RoutedUICommand EditorAltD = new RoutedUICommand(
            "EditorAltD",
            "EditorAltD",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.D, ModifierKeys.Alt)
            }
            );

        public static readonly RoutedUICommand EditorAltF = new RoutedUICommand(
            "EditorAltF",
            "EditorAltF",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.F, ModifierKeys.Alt)
            }
            );

        public static readonly RoutedUICommand Settings = new RoutedUICommand(
            "Settings",
            "Settings",
            typeof(CustomCommands),
            new InputGestureCollection( ) {
                new KeyGesture(Key.S, ModifierKeys.Alt | ModifierKeys.Shift)
            }
            );
    }
}