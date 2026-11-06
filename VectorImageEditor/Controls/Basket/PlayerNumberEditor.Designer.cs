namespace VectorImageEditor.Controls.Basket
{
    partial class PlayerNumberEditor
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox groupBoxNumber;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelNumber;
        private System.Windows.Forms.Label labelPosition;
        private System.Windows.Forms.Label labelColor;
        private System.Windows.Forms.Label labelFontSize;
        private System.Windows.Forms.TextBox tbNumber;
        private System.Windows.Forms.ComboBox cbPosition;
        private System.Windows.Forms.NumericUpDown nudFontSize;
        private System.Windows.Forms.Button btnColor;
        private System.Windows.Forms.CheckBox cbShow;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.groupBoxNumber = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelNumber = new System.Windows.Forms.Label();
            this.tbNumber = new System.Windows.Forms.TextBox();
            this.labelPosition = new System.Windows.Forms.Label();
            this.cbPosition = new System.Windows.Forms.ComboBox();
            this.labelColor = new System.Windows.Forms.Label();
            this.btnColor = new System.Windows.Forms.Button();
            this.labelFontSize = new System.Windows.Forms.Label();
            this.nudFontSize = new System.Windows.Forms.NumericUpDown();
            this.cbShow = new System.Windows.Forms.CheckBox();
            this.groupBoxNumber.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxNumber
            // 
            this.groupBoxNumber.AutoSize = true;
            this.groupBoxNumber.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxNumber.Controls.Add(this.tableLayoutPanel);
            this.groupBoxNumber.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxNumber.Location = new System.Drawing.Point(0, 0);
            this.groupBoxNumber.Name = "groupBoxNumber";
            this.groupBoxNumber.Padding = new System.Windows.Forms.Padding(8);
            this.groupBoxNumber.Size = new System.Drawing.Size(140, 166);
            this.groupBoxNumber.TabIndex = 0;
            this.groupBoxNumber.TabStop = false;
            this.groupBoxNumber.Text = "Numero di maglia";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.labelNumber, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.tbNumber, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelPosition, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.cbPosition, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelColor, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.btnColor, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.labelFontSize, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.nudFontSize, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.cbShow, 0, 4);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel.Location = new System.Drawing.Point(8, 21);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel.RowCount = 5;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(124, 137);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelNumber
            // 
            this.labelNumber.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelNumber.AutoSize = true;
            this.labelNumber.Location = new System.Drawing.Point(6, 9);
            this.labelNumber.Name = "labelNumber";
            this.labelNumber.Size = new System.Drawing.Size(44, 13);
            this.labelNumber.TabIndex = 0;
            this.labelNumber.Text = "Numero";
            // 
            // tbNumber
            // 
            this.tbNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbNumber.Location = new System.Drawing.Point(64, 6);
            this.tbNumber.Name = "tbNumber";
            this.tbNumber.Size = new System.Drawing.Size(54, 20);
            this.tbNumber.TabIndex = 1;
            this.tbNumber.TextChanged += new System.EventHandler(this.tbNumber_TextChanged);
            // 
            // labelPosition
            // 
            this.labelPosition.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelPosition.AutoSize = true;
            this.labelPosition.Location = new System.Drawing.Point(6, 36);
            this.labelPosition.Name = "labelPosition";
            this.labelPosition.Size = new System.Drawing.Size(52, 13);
            this.labelPosition.TabIndex = 2;
            this.labelPosition.Text = "Posizione";
            // 
            // cbPosition
            // 
            this.cbPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPosition.Location = new System.Drawing.Point(64, 32);
            this.cbPosition.Name = "cbPosition";
            this.cbPosition.Size = new System.Drawing.Size(54, 21);
            this.cbPosition.TabIndex = 3;
            this.cbPosition.SelectedIndexChanged += new System.EventHandler(this.cbPosition_SelectedIndexChanged);
            // 
            // labelColor
            // 
            this.labelColor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelColor.AutoSize = true;
            this.labelColor.Location = new System.Drawing.Point(6, 64);
            this.labelColor.Name = "labelColor";
            this.labelColor.Size = new System.Drawing.Size(37, 13);
            this.labelColor.TabIndex = 4;
            this.labelColor.Text = "Colore";
            // 
            // btnColor
            // 
            this.btnColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnColor.Location = new System.Drawing.Point(64, 59);
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(54, 23);
            this.btnColor.TabIndex = 5;
            this.btnColor.Text = "Scegli colore...";
            this.btnColor.UseVisualStyleBackColor = true;
            this.btnColor.Click += new System.EventHandler(this.Color_Click);
            // 
            // labelFontSize
            // 
            this.labelFontSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelFontSize.AutoSize = true;
            this.labelFontSize.Location = new System.Drawing.Point(6, 91);
            this.labelFontSize.Name = "labelFontSize";
            this.labelFontSize.Size = new System.Drawing.Size(45, 13);
            this.labelFontSize.TabIndex = 6;
            this.labelFontSize.Text = "Font (%)";
            // 
            // nudFontSize
            // 
            this.nudFontSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nudFontSize.Location = new System.Drawing.Point(64, 88);
            this.nudFontSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFontSize.Name = "nudFontSize";
            this.nudFontSize.Size = new System.Drawing.Size(54, 20);
            this.nudFontSize.TabIndex = 7;
            this.nudFontSize.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudFontSize.ValueChanged += new System.EventHandler(this.nudFontSize_ValueChanged);
            // 
            // cbShow
            // 
            this.cbShow.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.cbShow, 2);
            this.cbShow.Location = new System.Drawing.Point(6, 114);
            this.cbShow.Name = "cbShow";
            this.cbShow.Size = new System.Drawing.Size(112, 17);
            this.cbShow.TabIndex = 6;
            this.cbShow.Text = "Visualizza numero di maglia";
            this.cbShow.UseVisualStyleBackColor = true;
            this.cbShow.CheckedChanged += new System.EventHandler(this.cbShow_CheckedChanged);
            // 
            // PlayerNumberEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.groupBoxNumber);
            this.Name = "PlayerNumberEditor";
            this.Size = new System.Drawing.Size(140, 168);
            this.groupBoxNumber.ResumeLayout(false);
            this.groupBoxNumber.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
