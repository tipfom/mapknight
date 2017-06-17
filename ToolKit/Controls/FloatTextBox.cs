using System.Windows.Controls;

namespace mapKnight.ToolKit.Controls {
    public class FloatTextBox : TextBox {
        public FloatTextBox ( ) : base( ) {
            PreviewTextInput += FloatTextBox_PreviewTextInput;
        }

        private void FloatTextBox_PreviewTextInput (object sender, System.Windows.Input.TextCompositionEventArgs e) {
            float f;
            e.Handled = !float.TryParse(((TextBox)sender).Text.Insert(((TextBox)sender).CaretIndex, e.Text), out f) || (((TextBox)sender).Text.Length == 0 && e.Text == "-");
        }
    }
}
