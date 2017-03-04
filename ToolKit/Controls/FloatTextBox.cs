using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace mapKnight.ToolKit.Controls {
    public class FloatTextBox : TextBox {
        public FloatTextBox ( ) : base( ) {
            PreviewTextInput += FloatTextBox_PreviewTextInput;
        }

        private void FloatTextBox_PreviewTextInput (object sender, System.Windows.Input.TextCompositionEventArgs e) {
            float f;
            e.Handled = !float.TryParse(((TextBox)sender).Text.Insert(((TextBox)sender).CaretIndex, e.Text), out f);
        }
    }
}
