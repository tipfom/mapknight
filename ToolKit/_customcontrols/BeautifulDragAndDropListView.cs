using System;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace mapKnight.ToolKit {
    class BeautifulDragAndDropListView : BeautifulListView {
        public event EventHandler<Bitmap> BitmapDroped;

        public BeautifulDragAndDropListView () : base () {
            this.AllowDrop = true;
            this.DragEnter += BeautifulDragAndDropListView_DragEnter;
            this.DragDrop += BeautifulDragAndDropListView_DragDrop;
        }

        private void BeautifulDragAndDropListView_DragDrop (object sender, DragEventArgs e) {
            if (e.Effect == DragDropEffects.Copy) {
                string[] files = (string[])e.Data.GetData (DataFormats.FileDrop, false);
                // check if file has the png header
                // http://www.libpng.org/pub/png/spec/1.2/PNG-Structure.html
                for (int i = 0; i < files.Length; i++) {
                    if (File.ReadAllBytes (files[i]).ToList ().Take (8).SequenceEqual (new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })) {
                        if (BitmapDroped != null) {
                            BitmapDroped (this, new Bitmap (files[i]));
                        }
                    }
                }
            }
        }

        private void BeautifulDragAndDropListView_DragEnter (object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent (DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            } else {
                e.Effect = DragDropEffects.None;
            }
        }
    }
}
