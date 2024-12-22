using System.ComponentModel;
using System.Windows.Forms;

namespace ABT.Test.TestLib.TestSpec {
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
            this.LabelTO = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.listTO = new System.Windows.Forms.ListView();
            this.listTG = new System.Windows.Forms.ListView();
            this.labelTG = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LabelTO
            // 
            this.LabelTO.AutoSize = true;
            this.LabelTO.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.LabelTO.Location = new System.Drawing.Point(12, 6);
            this.LabelTO.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelTO.Name = "LabelTO";
            this.LabelTO.Size = new System.Drawing.Size(82, 13);
            this.LabelTO.TabIndex = 0;
            this.LabelTO.Text = "Test Operations";
            this.LabelTO.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Enabled = false;
            this.OK.Location = new System.Drawing.Point(310, 588);
            this.OK.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(58, 36);
            this.OK.TabIndex = 2;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // listTO
            // 
            this.listTO.FullRowSelect = true;
            this.listTO.GridLines = true;
            this.listTO.HideSelection = false;
            this.listTO.LabelWrap = false;
            this.listTO.Location = new System.Drawing.Point(10, 22);
            this.listTO.MultiSelect = false;
            this.listTO.Name = "listTO";
            this.listTO.ShowGroups = false;
            this.listTO.Size = new System.Drawing.Size(666, 156);
            this.listTO.TabIndex = 0;
            this.listTO.UseCompatibleStateImageBehavior = false;
            this.listTO.View = System.Windows.Forms.View.Details;
            this.listTO.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.List_TOChanged);
            this.listTO.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.List_MouseDoubleClick);
            // 
            // listTG
            // 
            this.listTG.FullRowSelect = true;
            this.listTG.GridLines = true;
            this.listTG.HideSelection = false;
            this.listTG.LabelWrap = false;
            this.listTG.Location = new System.Drawing.Point(10, 206);
            this.listTG.MultiSelect = false;
            this.listTG.Name = "listTG";
            this.listTG.ShowGroups = false;
            this.listTG.Size = new System.Drawing.Size(666, 351);
            this.listTG.TabIndex = 1;
            this.listTG.UseCompatibleStateImageBehavior = false;
            this.listTG.View = System.Windows.Forms.View.Details;
            this.listTG.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.List_TGChanged);
            this.listTG.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.List_MouseDoubleClick);
            // 
            // labelTG
            // 
            this.labelTG.AutoSize = true;
            this.labelTG.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.labelTG.Location = new System.Drawing.Point(12, 190);
            this.labelTG.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTG.Name = "labelTG";
            this.labelTG.Size = new System.Drawing.Size(65, 13);
            this.labelTG.TabIndex = 0;
            this.labelTG.Text = "Test Groups";
            this.labelTG.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // TestSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 673);
            this.ControlBox = false;
            this.Controls.Add(this.labelTG);
            this.Controls.Add(this.listTG);
            this.Controls.Add(this.listTO);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.LabelTO);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Tests";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private Label LabelTO;
        private Button OK;
        private ListView listTO;
        private ListView listTG;
        private Label labelTG;
    }
}