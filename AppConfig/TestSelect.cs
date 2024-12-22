using System;
using System.Windows.Forms;

namespace ABT.Test.TestLib.TestSpec {
    public partial class TestSelect : Form {
        internal static Object TestObject { get; private set; }

        public TestSelect() {
            InitializeComponent();
            listTO.MultiSelect = false;
            listTG.MultiSelect = false;
            ListLoad(listTO);
        }

        private void ListClear(ListView listView) {
            listView.Clear();
            listView.View = View.Details;
            listView.Columns.Add("ID");
            listView.Columns.Add("Description");
            if (listView == listTO) {
                OK.Enabled = false;
                ListClear(listTG);
            }
        }

        private void ListLoad(ListView listView) {
            ListClear(listView);
            if (listView == listTO) foreach (TO to in TestLib.TestSpecification.TestOperations) listTO.Items.Add(new ListViewItem(new String[] { to.NamespaceLeaf, to.Description }));
            else foreach (TG tg in TestLib.TestSpecification.TestOperations[listTO.SelectedItems[0].Index].TestGroups) {
                    if (tg.Independent) listTG.Items.Add(new ListViewItem(new String[] { tg.Class, tg.Description }));
                }
            listView.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            listView.Columns[1].Width = -2;
            // https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.columnheader.width?redirectedfrom=MSDN&view=windowsdesktop-7.0#System_Windows_Forms_ColumnHeader_Width
            listView.ResetText();
        }

        private void OK_Click(Object sender, EventArgs e) {
            if (listTG.SelectedItems.Count == 1) {
                TestObject =TestLib.TestSpecification.TestOperations[listTO.SelectedItems[0].Index].TestGroups[listTG.SelectedItems[0].Index];
                DialogResult = DialogResult.OK;
            } else if (listTO.SelectedItems.Count == 1) {
                TestObject = TestLib.TestSpecification.TestOperations[listTO.SelectedItems[0].Index];
                DialogResult = DialogResult.OK;
            }
        }

        private void List_MouseDoubleClick(Object sender, MouseEventArgs e) { OK_Click(sender, e); }

        public static Object Get() {
            TestSelect testSelect = new TestSelect();
            testSelect.ShowDialog(); // Waits until user clicks OK button.
            testSelect.Dispose();
            return TestObject;
        }

        private void List_TOChanged(Object sender, ListViewItemSelectionChangedEventArgs e) {
            if (listTO.SelectedItems.Count == 1) ListLoad(listTG);
            OK.Enabled = true;
        }

        private void List_TGChanged(Object sender, ListViewItemSelectionChangedEventArgs e) {
            OK.Enabled = true;
        }
    }
}
