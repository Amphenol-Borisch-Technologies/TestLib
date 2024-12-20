using System.ComponentModel;
using System.Windows.Forms;

namespace ABT.Test.TestLib.TestConfig {
    partial class TestSelect {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region
        private void InitializeComponent() {
            this.LabelSelections = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.ListSelections = new System.Windows.Forms.ListView();
            this.radioButtonTestOperations = new System.Windows.Forms.RadioButton();
            this.radioButtonTestGroups = new System.Windows.Forms.RadioButton();
            this.selectionType = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.selectionType.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelSelections
            // 
            this.LabelSelections.AutoSize = true;
            this.LabelSelections.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.LabelSelections.Location = new System.Drawing.Point(12, 6);
            this.LabelSelections.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelSelections.Name = "LabelSelections";
            this.LabelSelections.Size = new System.Drawing.Size(56, 13);
            this.LabelSelections.TabIndex = 0;
            this.LabelSelections.Text = "Selections";
            this.LabelSelections.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LabelSelections.Click += new System.EventHandler(this.LabelSelections_Click);
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Enabled = false;
            this.OK.Location = new System.Drawing.Point(310, 588);
            this.OK.Margin = new System.Windows.Forms.Padding(2);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(58, 36);
            this.OK.TabIndex = 4;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // ListSelections
            // 
            this.ListSelections.FullRowSelect = true;
            this.ListSelections.GridLines = true;
            this.ListSelections.HideSelection = false;
            this.ListSelections.LabelWrap = false;
            this.ListSelections.Location = new System.Drawing.Point(10, 22);
            this.ListSelections.MultiSelect = false;
            this.ListSelections.Name = "ListSelections";
            this.ListSelections.ShowGroups = false;
            this.ListSelections.Size = new System.Drawing.Size(666, 254);
            this.ListSelections.TabIndex = 0;
            this.ListSelections.UseCompatibleStateImageBehavior = false;
            this.ListSelections.View = System.Windows.Forms.View.Details;
            this.ListSelections.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.List_SelectionChanged);
            this.ListSelections.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.List_MouseDoubleClick);
            // 
            // radioButtonTestOperations
            // 
            this.radioButtonTestOperations.AutoSize = true;
            this.radioButtonTestOperations.Location = new System.Drawing.Point(5, 15);
            this.radioButtonTestOperations.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonTestOperations.Name = "radioButtonTestOperations";
            this.radioButtonTestOperations.Size = new System.Drawing.Size(100, 17);
            this.radioButtonTestOperations.TabIndex = 2;
            this.radioButtonTestOperations.TabStop = true;
            this.radioButtonTestOperations.Text = "Test Operations";
            this.radioButtonTestOperations.UseVisualStyleBackColor = true;
            this.radioButtonTestOperations.CheckedChanged += new System.EventHandler(this.GroupBoxSelect_CheckedChanged);
            // 
            // radioButtonTestGroups
            // 
            this.radioButtonTestGroups.AutoSize = true;
            this.radioButtonTestGroups.Location = new System.Drawing.Point(4, 36);
            this.radioButtonTestGroups.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonTestGroups.Name = "radioButtonTestGroups";
            this.radioButtonTestGroups.Size = new System.Drawing.Size(83, 17);
            this.radioButtonTestGroups.TabIndex = 3;
            this.radioButtonTestGroups.TabStop = true;
            this.radioButtonTestGroups.Text = "Test Groups";
            this.radioButtonTestGroups.UseVisualStyleBackColor = true;
            this.radioButtonTestGroups.CheckedChanged += new System.EventHandler(this.GroupBoxSelect_CheckedChanged);
            // 
            // selectionType
            // 
            this.selectionType.Controls.Add(this.radioButtonTestGroups);
            this.selectionType.Controls.Add(this.radioButtonTestOperations);
            this.selectionType.Location = new System.Drawing.Point(9, 588);
            this.selectionType.Margin = new System.Windows.Forms.Padding(2);
            this.selectionType.Name = "selectionType";
            this.selectionType.Padding = new System.Windows.Forms.Padding(2);
            this.selectionType.Size = new System.Drawing.Size(121, 63);
            this.selectionType.TabIndex = 1;
            this.selectionType.TabStop = false;
            // 
            // listView1
            // 
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(10, 308);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowGroups = false;
            this.listView1.Size = new System.Drawing.Size(666, 254);
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // TestSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 658);
            this.ControlBox = false;
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.selectionType);
            this.Controls.Add(this.ListSelections);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.LabelSelections);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Tests";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SelectTests_Load);
            this.selectionType.ResumeLayout(false);
            this.selectionType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private Label LabelSelections;
        private Button OK;
        private ListView ListSelections;
        private RadioButton radioButtonTestOperations;
        private RadioButton radioButtonTestGroups;
        private GroupBox selectionType;
        private ListView listView1;
    }
}