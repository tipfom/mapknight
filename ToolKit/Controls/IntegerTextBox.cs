using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace mapKnight.ToolKit.Controls {
    public class IntegerTextBox : TextBox {
        public IntegerTextBox ( ) : base( ) {
            PreviewTextInput += IntegerTextBox_PreviewTextInput;
        }

        private void IntegerTextBox_PreviewTextInput (object sender, System.Windows.Input.TextCompositionEventArgs e) {
            int i;
            e.Handled = !int.TryParse(Text.Insert(CaretIndex, e.Text), out i);
        }
    }
}
