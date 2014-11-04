using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WebMaster.com
{
    /// <summary>
    /// Event Handler for SubItem events
    /// </summary>
    public delegate void SubItemEventHandler(object sender, SubItemEventArgs e);
    /// <summary>
    /// Event Handler for SubItemEndEditing events
    /// </summary>
    public delegate void SubItemEndEditingEventHandler(object sender, SubItemEndEditingEventArgs e);

    /// <summary>
    /// Event Args for SubItemClicked event
    /// </summary>
    public class SubItemEventArgs : EventArgs
    {
        public SubItemEventArgs(ListViewItem item, int subItem) {
            _subItemIndex = subItem;
            _item = item;
        }
        private int _subItemIndex = -1;
        private ListViewItem _item = null;
        public int SubItem {
            get { return _subItemIndex; }
        }
        public ListViewItem Item {
            get { return _item; }
        }
    }

    /// <summary>
    /// Event Args for SubItemEndEditingClicked event
    /// </summary>
    public class SubItemEndEditingEventArgs : SubItemEventArgs
    {
        private string _text = string.Empty;
        private bool _cancel = true;

        public SubItemEndEditingEventArgs(ListViewItem item, int subItem, string display, bool cancel) :
            base(item, subItem) {
            _text = display;
            _cancel = cancel;
        }
        public string DisplayText {
            get { return _text; }
            set { _text = value; }
        }
        public bool Cancel {
            get { return _cancel; }
            set { _cancel = value; }
        }
    }
    public class ListViewEx : System.Windows.Forms.ListView
    {
        #region Component Designer generated code
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (this.cellEditors != null) {
                foreach (Control c in this.cellEditors) {
                    if (c != null) {
                        c.Leave -= new EventHandler(_editControl_Leave);
                        c.KeyPress -= new KeyPressEventHandler(_editControl_KeyPress);
                    }
                }
            }
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
        }

        #endregion
        #region Interop structs, imports and constants
        /// <summary>
        /// MessageHeader for WM_NOTIFY
        /// </summary>
        private struct NMHDR
        {
            public IntPtr hwndFrom;
            public Int32 idFrom;
            public Int32 code;
        }
        // Windows Messages that will abort editing
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int WM_SIZE = 0x05;
        private const int WM_NOTIFY = 0x4E;

        private const int HDN_FIRST = -300;
        private const int HDN_BEGINDRAG = (HDN_FIRST - 10);
        private const int HDN_ITEMCHANGINGA = (HDN_FIRST - 0);
        private const int HDN_ITEMCHANGINGW = (HDN_FIRST - 20);
        #endregion

        public event SubItemEventHandler SubItemClicked;
        public event SubItemEventHandler SubItemBeginEditing;
        public event SubItemEndEditingEventHandler SubItemEndEditing;

        public ListViewEx() {
            // This	call is	required by	the	Windows.Forms Form Designer.
            InitializeComponent();

            base.FullRowSelect = true;
            base.View = View.Details;
            base.AllowColumnReorder = false;
        }
        private bool _doubleClickActivation = false;
        /// <summary>
        /// cell editors, it should has the same order with the table columns. 
        /// </summary>
        private Control[] cellEditors = null;
        /// <summary>
        /// this is used to adjust the cell editor control's size. 
        /// if index is the column index and if the size != -1, it will use the new value to 
        /// adjust the cell editor control's size
        /// </summary>
        private int adjustColIndex = -1 ;
        private Size newSize = new Size(-1,-1);        
        /// <summary>
        /// Is a double click required to start editing a cell?
        /// </summary>
        public bool DoubleClickActivation {
            get { return _doubleClickActivation; }
            set { _doubleClickActivation = value; }
        }
        /// <summary>
        /// Set the colIndex's cell edit control's size with the new size value.
        /// To ignore this function, you can set the colIndex=-1
        /// if the size value is -1,-1, -1 means that no change, use the default. 
        /// </summary>
        /// <param name="colIndex"></param>
        /// <param name="newSize"></param>
        public void setAdjustCellCtrl(int colIndex, Size newSize) {
            this.adjustColIndex = colIndex;
            this.newSize = new Size(newSize.Width, newSize.Height);
        }
        /// <summary>
        /// This is used to mark that when do edit the column, the edit control's text 
        /// will not be changed. 
        /// </summary>
        private List<int> ignoredUpdateTextWgtList = new List<int>();        
        /// <summary>
        /// Find ListViewItem and SubItem Index at position (x,y)
        /// </summary>
        /// <param name="x">relative to ListView</param>
        /// <param name="y">relative to ListView</param>
        /// <param name="item">Item at position (x,y)</param>
        /// <returns>SubItem index</returns>
        public int GetSubItemAt(int x, int y, out ListViewItem item) {
            item = this.GetItemAt(x, y);

            if (item != null) {
                Rectangle lviBounds;
                int subItemX;

                lviBounds = item.GetBounds(ItemBoundsPortion.Entire);
                subItemX = lviBounds.Left;
                for (int i = 0; i < this.Columns.Count; i++) {
                    ColumnHeader h = this.Columns[i];
                    if (x < subItemX + h.Width) {
                        return h.Index;
                    }
                    subItemX += h.Width;
                }
            }

            return -1;
        }
        /// <summary>
        /// Get bounds for a SubItem
        /// </summary>
        /// <param name="Item">Target ListViewItem</param>
        /// <param name="SubItem">Target SubItem index</param>
        /// <returns>Bounds of SubItem (relative to ListView)</returns>
        public Rectangle GetSubItemBounds(ListViewItem Item, int SubItem) {
            Rectangle subItemRect = Rectangle.Empty;
            if (SubItem >= this.Columns.Count) {
                throw new IndexOutOfRangeException("SubItem " + SubItem + " out of range");
            }
            if (Item == null) {
                throw new ArgumentNullException("Item");
            }
            Rectangle lviBounds = Item.GetBounds(ItemBoundsPortion.Entire);
            int subItemX = lviBounds.Left;

            ColumnHeader col;
            int i = 0;
            for (i = 0; i < this.Columns.Count; i++) {
                col = this.Columns[i];
                if (col.Index == SubItem) {
                    break;
                }
                subItemX += col.Width;
            }
            subItemRect = new Rectangle(subItemX, lviBounds.Top, this.Columns[i].Width, lviBounds.Height);
            return subItemRect;
        }

        protected override void WndProc(ref	Message msg) {
            switch (msg.Msg) {
                // Look	for	WM_VSCROLL,WM_HSCROLL or WM_SIZE messages.
                case WM_VSCROLL:
                case WM_HSCROLL:
                case WM_SIZE:
                    EndEditing(false);
                    break;
                case WM_NOTIFY:
                    // Look for WM_NOTIFY of events that might also change the
                    // editor's position/size: Column reordering or resizing
                    NMHDR h = (NMHDR)Marshal.PtrToStructure(msg.LParam, typeof(NMHDR));
                    if (h.code == HDN_BEGINDRAG ||
                        h.code == HDN_ITEMCHANGINGA ||
                        h.code == HDN_ITEMCHANGINGW)
                        EndEditing(false);
                    break;
            }

            base.WndProc(ref msg);
        }
        #region Initialize editing depending of DoubleClickActivation property
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
            base.OnMouseUp(e);
            if (DoubleClickActivation) {
                return;
            }

            EditSubitemAt(new Point(e.X, e.Y));
        }

        protected override void OnDoubleClick(EventArgs e) {
            base.OnDoubleClick(e);

            if (!DoubleClickActivation) {
                return;
            }

            Point pt = this.PointToClient(Cursor.Position);

            EditSubitemAt(pt);
        }

        ///<summary>
        /// Fire SubItemClicked
        ///</summary>
        ///<param name="p">Point of click/doubleclick</param>
        private void EditSubitemAt(Point p) {
            ListViewItem item;
            int idx = GetSubItemAt(p.X, p.Y, out item);
            if (idx >= 0) {
                OnSubItemClicked(new SubItemEventArgs(item, idx));
            }
        }

        #endregion

        #region In-place editing functions
        // The control performing the actual editing
        private Control _editingControl;
        // The ListViewItem being edited
        private ListViewItem _editItem;
        // The SubItem being edited
        private int _editSubItem;

        protected void OnSubItemBeginEditing(SubItemEventArgs e) {
            if (SubItemBeginEditing != null) {
                SubItemBeginEditing(this, e);
            }
        }
        protected void OnSubItemEndEditing(SubItemEndEditingEventArgs e) {
            if (SubItemEndEditing != null) {
                SubItemEndEditing(this, e);
            }
        }
        protected void OnSubItemClicked(SubItemEventArgs e) {
            if (SubItemClicked != null) {
                SubItemClicked(this, e);
            }
        }
        public void setCellEditors(Control[] editors) {
            this.setCellEditors(editors, null);
        }
        /// <summary>
        /// The list is used to mark that when do edit the column, the edit control's text 
        /// will not be changed. 
        /// </summary>
        /// <param name="editors"></param>
        /// <param name="ignoreTextUpdateIndexs"></param>
        public void setCellEditors(Control[] editors, List<int> ignoreTextUpdateIndexs) {
            this.cellEditors = editors;
            foreach (Control c in this.cellEditors) {
                if (c != null) {
                    c.Leave += new EventHandler(_editControl_Leave);
                    c.KeyPress += new KeyPressEventHandler(_editControl_KeyPress);
                }
            }
            this.ignoredUpdateTextWgtList.Clear();
            if (ignoreTextUpdateIndexs != null) {
                foreach (int index in ignoreTextUpdateIndexs) {
                    if (index >= 0 && index < this.Columns.Count) {
                        if (!ignoredUpdateTextWgtList.Contains(index)) {
                            ignoredUpdateTextWgtList.Add(index);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Begin in-place editing of given cell, the subItemIndex is used to located the 
        /// relative cellEditor. 
        /// </summary>
        /// <param name="subItemIndex">SubItem index to edit</param>
        /// <param name="Item">ListViewItem to edit</param>
        /// <param name="SubItem">SubItem index to edit</param>
        public void StartEditing(int subItemIndex, ListViewItem Item) {
            if (subItemIndex < 0 || subItemIndex > this.cellEditors.Length) {
                return;
            }
            Control c = this.cellEditors[subItemIndex];
            if (c == null) {
                return;
            }
            OnSubItemBeginEditing(new SubItemEventArgs(Item, subItemIndex));

            Rectangle rcSubItem = GetSubItemBounds(Item, subItemIndex);

            if (rcSubItem.X < 0) {
                // Left edge of SubItem not visible - adjust rectangle position and width
                rcSubItem.Width += rcSubItem.X;
                rcSubItem.X = 0;
            }
            if (rcSubItem.X + rcSubItem.Width > this.Width) {
                // Right edge of SubItem not visible - adjust rectangle width
                rcSubItem.Width = this.Width - rcSubItem.Left;
            }

            // Subitem bounds are relative to the location of the ListView!
            rcSubItem.Offset(Left, Top);

            // In case the editing control and the listview are on different parents,
            // account for different origins
            Point origin = new Point(0, 0);
            Point lvOrigin = this.Parent.PointToScreen(origin);
            Point ctlOrigin = c.Parent.PointToScreen(origin);

            rcSubItem.Offset(lvOrigin.X - ctlOrigin.X, lvOrigin.Y - ctlOrigin.Y);

            // adjust the cell editor control if need 
            if (this.adjustColIndex > 0 && this.adjustColIndex < this.cellEditors.Length) {
                int w = this.newSize.Width;
                int h = this.newSize.Height;
                if (w == -1) {
                    w = rcSubItem.Width;
                }
                if (h == -1) {
                    h = rcSubItem.Height;
                }
                int x = rcSubItem.Width-w ;

                rcSubItem.Width = w;
                rcSubItem.Height = h;
                x = x < 0 ? 0 : x;
                rcSubItem.X += x;
            }

            // Position and show editor
            c.Bounds = rcSubItem;
            if (!this.ignoredUpdateTextWgtList.Contains(subItemIndex)) {
                c.Text = Item.SubItems[subItemIndex].Text;
            }
            c.Visible = true;
            c.BringToFront();
            c.Focus();

            _editingControl = c;
            //_editingControl.Leave += new EventHandler(_editControl_Leave);
            //_editingControl.KeyPress += new KeyPressEventHandler(_editControl_KeyPress);

            _editItem = Item;
            _editSubItem = subItemIndex;
        }

        private void _editControl_Leave(object sender, EventArgs e) {
            // cell editor losing focus
            EndEditing(true);
        }

        private void _editControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case (char)(int)Keys.Escape: {
                        EndEditing(false);
                        break;
                    }

                case (char)(int)Keys.Enter: {
                        EndEditing(true);
                        break;
                    }
            }
        }

        /// <summary>
        /// Accept or discard current value of cell editor control
        /// </summary>
        /// <param name="AcceptChanges">Use the _editingControl's Text as new SubItem text or discard changes?</param>
        public void EndEditing(bool AcceptChanges) {
            if (_editingControl == null) {
                return;
            }
            string text = AcceptChanges ?
                    _editingControl.Text :	// Use editControl text if changes are accepted
                    _editItem.SubItems[_editSubItem].Text;	// or the original subitem's text, if changes are discarded

            SubItemEndEditingEventArgs e = new SubItemEndEditingEventArgs(
                _editItem,		// The item being edited
                _editSubItem,	// The subitem index being edited
                text,
                !AcceptChanges	// Cancel?
            );

            OnSubItemEndEditing(e);

            _editItem.SubItems[_editSubItem].Text = e.DisplayText;

            //_editingControl.Leave -= new EventHandler(_editControl_Leave);
            //_editingControl.KeyPress -= new KeyPressEventHandler(_editControl_KeyPress);

            _editingControl.Visible = false;

            _editingControl = null;
            _editItem = null;
            _editSubItem = -1;
        }
        #endregion
    }
}
