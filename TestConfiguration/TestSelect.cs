using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ABT.Test.TestLib.TestConfiguration {
    public partial class TestSelect : Form {
        private static TestSpace testSpace;

        public TestSelect() {
            InitializeComponent();
            TestList.MultiSelect = false;
            TestOperations.Enabled = TestOperations.Checked = TestGroups.Enabled = true;
            TestGroups.Checked = false;
            ListLoad();
        }

        public static TestSpace Get() {
            TestSelect testSelect = new TestSelect();
            testSelect.ShowDialog(); // Waits until user clicks OK button.
            testSelect.Dispose();
            return testSpace;
        }

        private void ListLoad() {
            TestList.Clear();
            TestList.View = View.Details;
            TestList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            if (TestOperations.Checked) {
                TestGroups.Checked = false;
                Text = "Select Test Operation";
                TestList.Columns.Add("Operation");
                TestList.Columns.Add("Description");
                foreach (TestOperation testOperation in TestLib.testDefinition.TestSpace.TestOperations) TestList.Items.Add(new ListViewItem(new String[] { testOperation.NamespaceTrunk, testOperation.Description }));
            } else {
                TestOperations.Checked = false;
                Text = "Select Test Group";
                TestList.Columns.Add("Operation");
                TestList.Columns.Add("Group");
                TestList.Columns.Add("Description");
                foreach (TestOperation testOperation in TestLib.testDefinition.TestSpace.TestOperations) {
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

            testSpace = Serializing.DeserializeFromFile<TestSpace>(TestLib.TestDefinitionXML);
            testSpace.IsOperation = TestOperations.Checked;
            if (testSpace.IsOperation) { // A TestOperation was selected, including all its TestGroups, and all their Methods.  This is a full test run, so test data will be saved.
                TestOperation selectedOperation = testSpace.TestOperations[TestList.SelectedItems[0].Index];
                testSpace.TestOperations.RemoveAll(to => to != selectedOperation); // Retain only the selected TestOperation, all its TestGroups, and all their Methods.
            } else { // Only a TestGroup was selected, including all its Methods.  This is a partial test run, so test data won't be saved.
                TestOperation selectedOperation = testSpace.TestOperations.Find(nt => nt.NamespaceTrunk.Equals(TestList.SelectedItems[0].SubItems[0].Text));
                testSpace.TestOperations.RemoveAll(to => to != selectedOperation); // Retain only the selected TestOperation, all it's TestGroups, and all their Methods.
                TestGroup selectedGroup = testSpace.TestOperations[0].TestGroups.Find(tg => tg.Class.Equals(TestList.SelectedItems[0].SubItems[1].Text));
                testSpace.TestOperations[0].TestGroups.RemoveAll(tg => tg != selectedGroup); // From the selected TestOperation, retain only the selected TestGroup and all its Methods.
            }
            DialogResult = DialogResult.OK;
        }

        private void TestOperations_Clicked(Object sender, EventArgs e) { ListLoad(); }

        private void TestGroups_Clicked(Object sender, EventArgs e) { ListLoad(); }
    }
}
