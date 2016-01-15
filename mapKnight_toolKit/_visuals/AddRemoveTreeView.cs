using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace mapKnight.ToolKit
{
    class AddRemoveTreeView : TreeView
    {
        public event EventHandler<TreeNode> OnAddButtonClicked;
        public event EventHandler<TreeNode> OnRemoveButtonClicked;

        private MenuItem addItem = new MenuItem();
        private MenuItem removeItem = new MenuItem();

        private List<TreeNode> activeAddItems = new List<TreeNode>();
        private List<TreeNode> activeRemoveItems = new List<TreeNode>();

        public AddRemoveTreeView()
        {
            addItem.Text = "Add";
            addItem.Click += HandleAddItemClick;
            addItem.Shortcut = Shortcut.CtrlA;

            removeItem.Text = "Remove";
            removeItem.Click += HandleRemoveItemClick;
            removeItem.Shortcut = Shortcut.CtrlR;

            this.KeyDown += HandleKeyDown;
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode & Keys.Control) == Keys.Control && (e.KeyCode & Keys.A) == Keys.A)
            {
                if (OnAddButtonClicked != null)
                    OnAddButtonClicked.Invoke(this, this.SelectedNode);
            }

            if ((e.KeyCode & Keys.Control) == Keys.Control && (e.KeyCode & Keys.R) == Keys.R)
            {
                if (OnRemoveButtonClicked != null)
                    OnRemoveButtonClicked.Invoke(this, this.SelectedNode);
            }
        }

        public void EnableAddButton(TreeNode node)
        {
            if (node.ContextMenu == null)
                node.ContextMenu = new ContextMenu();

            if (!activeAddItems.Contains(node))
            {
                node.ContextMenu.MenuItems.Add(0, addItem.CloneMenu());
                activeAddItems.Add(node);
            }

        }

        public void DisableAddButton(TreeNode node)
        {
            if (node.ContextMenu == null)
                node.ContextMenu = new ContextMenu();
            else if (activeAddItems.Contains(node))
            {
                node.ContextMenu.MenuItems.RemoveAt(0);
                activeAddItems.Remove(node);
            }
        }

        public void EnableRemoveButton(TreeNode node)
        {
            if (node.ContextMenu == null)
                node.ContextMenu = new ContextMenu();


            if (!activeRemoveItems.Contains(node))
            {
                node.ContextMenu.MenuItems.Add(removeItem.CloneMenu());
                activeRemoveItems.Add(node);
            }
        }

        public void DisableRemoveButton(TreeNode node)
        {
            if (node.ContextMenu == null)
                node.ContextMenu = new ContextMenu();
            else if (activeRemoveItems.Contains(node))
            {
                if (activeAddItems.Contains(node))
                    node.ContextMenu.MenuItems.RemoveAt(1);
                else
                    node.ContextMenu.MenuItems.RemoveAt(0);
                activeRemoveItems.Remove(node);
            }
        }

        private void HandleAddItemClick(object sender, EventArgs e)
        {
            if (OnAddButtonClicked != null)
                OnAddButtonClicked.Invoke(this, this.SelectedNode);
        }

        private void HandleRemoveItemClick(object sender, EventArgs e)
        {
            if (OnRemoveButtonClicked != null)
                OnRemoveButtonClicked.Invoke(this, this.SelectedNode);
        }

    }
}
