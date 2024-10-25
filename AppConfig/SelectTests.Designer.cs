using System.ComponentModel;
using System.Windows.Forms;

namespace ABT.Test.Lib.AppConfig {
    partial class SelectTests {
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
            this.selectionType.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelSelections
            // 
            this.LabelSelections.AutoSize = true;
            this.LabelSelections.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.LabelSelections.Location = new System.Drawing.Point(16, 7);
            this.LabelSelections.Name = "LabelSelections";
            this.LabelSelections.Size = new System.Drawing.Size(70, 16);
            this.LabelSelections.TabIndex = 0;
            this.LabelSelections.Text = "Selections";
            this.LabelSelections.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Enabled = false;
            this.OK.Location = new System.Drawing.Point(413, 366);
            this.OK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(77, 44);
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
            this.ListSelections.Location = new System.Drawing.Point(13, 27);
            this.ListSelections.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ListSelections.MultiSelect = false;
            this.ListSelections.Name = "ListSelections";
            this.ListSelections.ShowGroups = false;
            this.ListSelections.Size = new System.Drawing.Size(887, 312);
            this.ListSelections.TabIndex = 0;
            this.ListSelections.UseCompatibleStateImageBehavior = false;
            this.ListSelections.View = System.Windows.Forms.View.Details;
            this.ListSelections.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.List_SelectionChanged);
            this.ListSelections.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.List_MouseDoubleClick);
            // 
            // radioButtonTestOperations
            // 
            this.radioButtonTestOperations.AutoSize = true;
            this.radioButtonTestOperations.Location = new System.Drawing.Point(7, 18);
            this.radioButtonTestOperations.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButtonTestOperations.Name = "radioButtonTestOperations";
            this.radioButtonTestOperations.Size = new System.Drawing.Size(327, 20);
            this.radioButtonTestOperations.TabIndex = 2;
            this.radioButtonTestOperations.TabStop = true;
            this.radioButtonTestOperations.Text = "FilePro Traveler Operations for Production Testing";
            this.radioButtonTestOperations.UseVisualStyleBackColor = true;
            this.radioButtonTestOperations.CheckedChanged += new System.EventHandler(this.GroupBoxSelect_CheckedChanged);
            // 
            // radioButtonTestGroups
            // 
            this.radioButtonTestGroups.AutoSize = true;
            this.radioButtonTestGroups.Location = new System.Drawing.Point(5, 44);
            this.radioButtonTestGroups.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButtonTestGroups.Name = "radioButtonTestGroups";
            this.radioButtonTestGroups.Size = new System.Drawing.Size(225, 20);
            this.radioButtonTestGroups.TabIndex = 3;
            this.radioButtonTestGroups.TabStop = true;
            this.radioButtonTestGroups.Text = "Test Groups for Trouble-shooting";
            this.radioButtonTestGroups.UseVisualStyleBackColor = true;
            this.radioButtonTestGroups.CheckedChanged += new System.EventHandler(this.GroupBoxSelect_CheckedChanged);
            // 
            // selectionType
            // 
            this.selectionType.Controls.Add(this.radioButtonTestGroups);
            this.selectionType.Controls.Add(this.radioButtonTestOperations);
            this.selectionType.Location = new System.Drawing.Point(12, 346);
            this.selectionType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.selectionType.Name = "selectionType";
            this.selectionType.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.selectionType.Size = new System.Drawing.Size(355, 70);
            this.selectionType.TabIndex = 1;
            this.selectionType.TabStop = false;
            // 
            // SelectTests
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 466);
            this.ControlBox = false;
            this.Controls.Add(this.selectionType);
            this.Controls.Add(this.ListSelections);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.LabelSelections);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectTests";
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
    }
}