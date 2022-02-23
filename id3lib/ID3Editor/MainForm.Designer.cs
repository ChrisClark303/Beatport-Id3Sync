using System.Windows.Forms;
using System.ComponentModel;

namespace TagEditor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if( disposing && (_presenter != null) )
            {
                _presenter.Dispose();
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._mainMenu = new System.Windows.Forms.MenuStrip();
            this._mainMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._scanMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._mainListBox = new System.Windows.Forms.ListBox();
            this._listBoxContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._editListBoxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._advancedEditListBoxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._compactListBoxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._launchListBoxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._removeV2tag = new System.Windows.Forms.ToolStripMenuItem();
            this._mainMenu.SuspendLayout();
            this._listBoxContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // _mainMenu
            // 
            this._mainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mainMenuItem});
            this._mainMenu.Location = new System.Drawing.Point(0, 0);
            this._mainMenu.Name = "_mainMenu";
            this._mainMenu.Size = new System.Drawing.Size(200, 24);
            this._mainMenu.TabIndex = 0;
            // 
            // _mainMenuItem
            // 
            this._mainMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._scanMenuItem});
            this._mainMenuItem.Name = "_mainMenuItem";
            this._mainMenuItem.Size = new System.Drawing.Size(56, 20);
            this._mainMenuItem.Text = "Main";
            // 
            // _scanMenuItem
            // 
            this._scanMenuItem.Name = "_scanMenuItem";
            this._scanMenuItem.Size = new System.Drawing.Size(188, 26);
            this._scanMenuItem.Text = "Scan Directory";
            this._scanMenuItem.Click += new System.EventHandler(this._scanMenuItem_Click);
            // 
            // _mainListBox
            // 
            this._mainListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._mainListBox.ContextMenuStrip = this._listBoxContextMenu;
            this._mainListBox.ItemHeight = 20;
            this._mainListBox.Location = new System.Drawing.Point(12, 12);
            this._mainListBox.Name = "_mainListBox";
            this._mainListBox.Size = new System.Drawing.Size(650, 264);
            this._mainListBox.TabIndex = 0;
            this._mainListBox.DoubleClick += new System.EventHandler(this._mainListBox_DoubleClick);
            this._mainListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this._mainListBox_MouseDown);
            // 
            // _listBoxContextMenu
            // 
            this._listBoxContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this._listBoxContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._editListBoxMenuItem,
            this._advancedEditListBoxMenuItem,
            this._compactListBoxMenuItem,
            this._launchListBoxMenuItem,
            this._removeV2tag});
            this._listBoxContextMenu.Name = "_listBoxContextMenu";
            this._listBoxContextMenu.Size = new System.Drawing.Size(203, 124);
            // 
            // _editListBoxMenuItem
            // 
            this._editListBoxMenuItem.Name = "_editListBoxMenuItem";
            this._editListBoxMenuItem.Size = new System.Drawing.Size(202, 24);
            this._editListBoxMenuItem.Text = "Edit";
            this._editListBoxMenuItem.Click += new System.EventHandler(this._mainListBoxMenu_EditTag);
            // 
            // _advancedEditListBoxMenuItem
            // 
            this._advancedEditListBoxMenuItem.Name = "_advancedEditListBoxMenuItem";
            this._advancedEditListBoxMenuItem.Size = new System.Drawing.Size(202, 24);
            this._advancedEditListBoxMenuItem.Text = "Advanced Edit";
            this._advancedEditListBoxMenuItem.Click += new System.EventHandler(this._mainListBoxMenu_EditExtendedTag);
            // 
            // _compactListBoxMenuItem
            // 
            this._compactListBoxMenuItem.Name = "_compactListBoxMenuItem";
            this._compactListBoxMenuItem.Size = new System.Drawing.Size(202, 24);
            this._compactListBoxMenuItem.Text = "Compact";
            this._compactListBoxMenuItem.Click += new System.EventHandler(this._mainListBoxMenu_Compact);
            // 
            // _launchListBoxMenuItem
            // 
            this._launchListBoxMenuItem.Name = "_launchListBoxMenuItem";
            this._launchListBoxMenuItem.Size = new System.Drawing.Size(202, 24);
            this._launchListBoxMenuItem.Text = "Launch";
            this._launchListBoxMenuItem.Click += new System.EventHandler(this._mainListBoxMenu_Launch);
            // 
            // _removeV2tag
            // 
            this._removeV2tag.Name = "_removeV2tag";
            this._removeV2tag.Size = new System.Drawing.Size(202, 24);
            this._removeV2tag.Text = "Remove ID3V2 tag";
            this._removeV2tag.Click += new System.EventHandler(this._removeV2tag_Click);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 20);
            this.ClientSize = new System.Drawing.Size(672, 387);
            this.Controls.Add(this._mainListBox);
            this.MainMenuStrip = this._mainMenu;
            this.Name = "MainForm";
            this.Text = "ID3 Editor";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this._mainMenu.ResumeLayout(false);
            this._mainMenu.PerformLayout();
            this._listBoxContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region fields
        protected System.Windows.Forms.MenuStrip _mainMenu;
        protected System.Windows.Forms.ToolStripMenuItem _mainMenuItem;
        protected System.Windows.Forms.ToolStripMenuItem _scanMenuItem;
        protected System.Windows.Forms.ListBox _mainListBox;
        protected System.Windows.Forms.ContextMenuStrip _listBoxContextMenu;
        protected System.Windows.Forms.ToolStripMenuItem _editListBoxMenuItem;
        protected System.Windows.Forms.ToolStripMenuItem _advancedEditListBoxMenuItem;
        protected System.Windows.Forms.ToolStripMenuItem _compactListBoxMenuItem;
        protected System.Windows.Forms.ToolStripMenuItem _launchListBoxMenuItem;
        #endregion
        private ToolStripMenuItem _removeV2tag;
    }
}