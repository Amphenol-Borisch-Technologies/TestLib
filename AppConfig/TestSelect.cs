using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ABT.Test.TestLib.TestDefinition {
    public partial class TestSelect : Form {
        internal static TestOperation TestOperation { get; private set; }
        internal static TestGroup TestGroup { get; private set; }

        public TestSelect() {
            InitializeComponent();
            TestList.MultiSelect = false;
            TestOperations.Enabled = TestOperations.Checked = TestGroups.Enabled = true;
            TestGroups.Checked = false;
            ListLoad();
        }

        public static (TestOperation, TestGroup) Get() {
            TestSelect testSelect = new TestSelect();
            testSelect.ShowDialog(); // Waits until user clicks OK button.
            testSelect.Dispose();
            return (TestOperation, TestGroup);
        }

        private void ListLoad() {
            TestList.Clear();
            TestOperation = null;
            TestGroup = null;
            TestList.View = View.Details;
            TestList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            if (TestOperations.Checked) {
                TestGroups.Checked = false;
                Text = "Select Test Operation";
                TestList.Columns.Add("Operation");
                TestList.Columns.Add("Description");
                foreach (TestOperation testOperation in TestSelection.TestSpace.TestOperations) TestList.Items.Add(new ListViewItem(new String[] { testOperation.NamespaceTrunk, testOperation.Description }));
            } else {
                TestOperations.Checked = false;
                Text = "Select Test Group";
                TestList.Columns.Add("Operation");
                TestList.Columns.Add("Group");
                TestList.Columns.Add("Description");
                foreach (TestOperation testOperation in TestSelection.TestSpace.TestOperations) {
                    foreach (TestGroup testGroup in testOperation.TestGroups)
                        if (testGroup.Independent) TestList.Items.Add(new ListViewItem(new String[] { testOperation.NamespaceTrunk, testGroup.Class, testGroup.Description }));
                }
            }
            foreach (ColumnHeader ch in TestList.Columns) ch.Width = -2;
            TestList.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            // https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.columnheader.width?redirectedfrom=MSDN&view=windowsdesktop-7.0#System_Windows_Forms_ColumnHeader_Width
            TestList.ResetText();
            OK.Enabled = false;
        }

        private void TestList_Changed(Object sender, ListViewItemSelectionChangedEventArgs e) { OK.Enabled = (TestList.SelectedItems.Count == 1); }

        private void TestList_MouseDoubleClick(Object sender, MouseEventArgs e) { OK_Click(sender, e); }

        private void OK_Click(Object sender, EventArgs e) {
            Debug.Assert(TestList.SelectedItems.Count == 1);
            if (TestOperations.Checked) {
                TestOperation = TestSelection.TestSpace.TestOperations[TestList.SelectedItems[0].Index];
                TestGroup = null;
            } else {
                TestOperation = TestSelection.TestSpace.TestOperations.Find(to => to.NamespaceTrunk.Equals(TestList.SelectedItems[0].Text));
                TestGroup = TestOperation.TestGroups.Find(tg => tg.Class.Equals(TestList.SelectedItems[0].SubItems[1].Text));
            }
            DialogResult = DialogResult.OK;
        }

        private void TestOperations_Clicked(Object sender, EventArgs e)  { ListLoad(); }

        private void TestGroups_Clicked(Object sender, EventArgs e) { ListLoad(); }
    }
}
