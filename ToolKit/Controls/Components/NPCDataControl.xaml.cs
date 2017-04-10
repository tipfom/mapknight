using mapKnight.ToolKit.Data.Components;
using System;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;

namespace mapKnight.ToolKit.Controls.Components {
    /// <summary>
    /// Interaktionslogik für NPCDataControl.xaml
    /// </summary>
    public partial class NPCDataControl : UserControl {
        private NPCDataComponent referenceComponent;
        private DispatcherTimer updateTimer;

        private NPCDataControl( ) {
            InitializeComponent( );
            updateTimer = new DispatcherTimer();
            updateTimer.Interval = new TimeSpan(0, 0, 0, 1);
            updateTimer.Tick += UpdateTimer_Tick;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e) {
            referenceComponent.Dialog = textbox_dialog.Text.Split(new[ ] { ">>>>>" }, StringSplitOptions.RemoveEmptyEntries);
            updateTimer.Stop( );
        }

        public NPCDataControl(NPCDataComponent referenceComponent) : this( ) {
            this.referenceComponent = referenceComponent;
        }

        private void textbox_dialog_TextChanged(object sender, TextChangedEventArgs e) {
            updateTimer.Stop( );
            updateTimer.Start( );
        }

        public void UpdateTextBox( ) {
            textbox_dialog.Text = String.Join("\n>>>>>\n", referenceComponent.Dialog);
        }
    }
}
