using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace mapKnight.ToolKit {
    class AddRemoveTreeView : TreeView {
        public event EventHandler<TreeNode> OnAddNormalButtonClicked;
        public event EventHandler<TreeNode> OnAddDefaultButtonClicked;
        public event EventHandler<TreeNode> OnRemoveButtonClicked;

        private MenuItem addNormalItem = new MenuItem ();
        private MenuItem addDefaultItem = new MenuItem ();
        private MenuItem removeItem = new MenuItem ();

        private List<TreeNode> activeAddNormalItems = new List<TreeNode> ();
        private List<TreeNode> activeAddDefaultItems = new List<TreeNode> ();
        private List<TreeNode> activeRemoveItems = new List<TreeNode> ();

        public AddRemoveTreeView () {
            addNormalItem.Name = "add_normal";
            addNormalItem.Text = "Add";
            addNormalItem.Click += HandleAddNormalItemClick;
            addNormalItem.Shortcut = Shortcut.CtrlA;

            addNormalItem.Name = "add_default";
            addDefaultItem.Text = "Add Default";
            addDefaultItem.Click += HandleAddDefaultItemClick;
            addDefaultItem.Shortcut = Shortcut.CtrlD;

            addNormalItem.Name = "remove";
            removeItem.Text = "Remove";
            removeItem.Click += HandleRemoveItemClick;
            removeItem.Shortcut = Shortcut.CtrlR;

            this.KeyDown += HandleKeyDown;
        }

        private void HandleKeyDown (object sender, KeyEventArgs e) {
            if (e.KeyData == (Keys.Control | Keys.A)) {
                if (OnAddNormalButtonClicked != null)
                    OnAddNormalButtonClicked.Invoke (this, this.SelectedNode);
            }

            if (e.KeyData == (Keys.Control | Keys.R)) {
                if (OnRemoveButtonClicked != null)
                    OnRemoveButtonClicked.Invoke (this, this.SelectedNode);
            }
            
            if (e.KeyData == (Keys.Control | Keys.D)) {
                if (OnAddDefaultButtonClicked != null)
                    OnAddDefaultButtonClicked.Invoke (this, this.SelectedNode);
            }
        }

        public void EnableAddNormalButton (TreeNode node) {
            if (node.ContextMenu == null)
                node.ContextMenu = new ContextMenu ();

            if (!activeAddNormalItems.Contains (node)) {
                node.ContextMenu.MenuItems.Add (0, addNormalItem.CloneMenu ());
                activeAddNormalItems.Add (node);
            }

        }

        public void DisableAddNormalButton (TreeNode node) {
            if (node.ContextMenu == null)
                node.ContextMenu = new ContextMenu ();

            if (activeAddNormalItems.Contains (node)) {
                node.ContextMenu.MenuItems.Remove (node.ContextMenu.MenuItems.Find ("add_normal", false)[0]);
                activeAddNormalItems.Remove (node);
            }
        }

        public void EnableAddDefaultButton (TreeNode node) {
            if (node.ContextMenu == null)
                node.ContextMenu = new ContextMenu ();

            if (!activeAddDefaultItems.Contains (node)) {
                node.ContextMenu.MenuItems.Add (1, addDefaultItem.CloneMenu ());
                activeAddDefaultItems.Add (node);
            }
        }

        public void DisableAddDefaultButton (TreeNode node) {
            if (node.ContextMenu == null)
                node.ContextMenu = new ContextMenu ();
            else if (activeAddDefaultItems.Contains (node)) {
                node.ContextMenu.MenuItems.Remove (node.ContextMenu.MenuItems.Find ("add_default", false)[0]);
                activeAddDefaultItems.Remove (node);
            }
        }

        public void EnableRemoveButton (TreeNode node) {
            if (node.ContextMenu == null)
                node.ContextMenu = new ContextMenu ();


            if (!activeRemoveItems.Contains (node)) {
                node.ContextMenu.MenuItems.Add (removeItem.CloneMenu ());
                activeRemoveItems.Add (node);
            }
        }

        public void DisableRemoveButton (TreeNode node) {
            if (node.ContextMenu == null)
                node.ContextMenu = new ContextMenu ();
            else if (activeRemoveItems.Contains (node)) {
                node.ContextMenu.MenuItems.Remove (node.ContextMenu.MenuItems.Find ("remove", false)[0]);
                activeRemoveItems.Remove (node);
            }
        }

        private void HandleAddNormalItemClick (object sender, EventArgs e) {
            if (OnAddNormalButtonClicked != null)
                OnAddNormalButtonClicked.Invoke (this, activeAddNormalItems.Find ((TreeNode obj) => obj.ContextMenu == ((MenuItem)sender).Parent));
        }

        private void HandleAddDefaultItemClick (object sender, EventArgs e) {
            if (OnAddDefaultButtonClicked != null)
                OnAddDefaultButtonClicked.Invoke (this, activeAddDefaultItems.Find ((TreeNode obj) => obj.ContextMenu == ((MenuItem)sender).Parent));
        }

        private void HandleRemoveItemClick (object sender, EventArgs e) {
            if (OnRemoveButtonClicked != null)
                OnRemoveButtonClicked.Invoke (this, activeRemoveItems.Find ((TreeNode obj) => obj.ContextMenu == ((MenuItem)sender).Parent));
        }

    }
}
