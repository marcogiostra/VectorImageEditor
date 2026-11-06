namespace VectorImageEditor.Controls.Basket
{
    partial class OggettoBasketEditor
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.grOpbjectBasket = new System.Windows.Forms.GroupBox();
            this.cmbKind = new System.Windows.Forms.ComboBox();
            this.lblBorderColor = new System.Windows.Forms.Label();
            this.lblFillColor = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grOpbjectBasket.SuspendLayout();
            this.SuspendLayout();
            // 
            // grOpbjectBasket
            // 
            this.grOpbjectBasket.Controls.Add(this.lblBorderColor);
            this.grOpbjectBasket.Controls.Add(this.lblFillColor);
            this.grOpbjectBasket.Controls.Add(this.label2);
            this.grOpbjectBasket.Controls.Add(this.label1);
            this.grOpbjectBasket.Controls.Add(this.cmbKind);
            this.grOpbjectBasket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grOpbjectBasket.Location = new System.Drawing.Point(0, 0);
            this.grOpbjectBasket.Name = "grOpbjectBasket";
            this.grOpbjectBasket.Size = new System.Drawing.Size(150, 110);
            this.grOpbjectBasket.TabIndex = 0;
            this.grOpbjectBasket.TabStop = false;
            this.grOpbjectBasket.Text = "Oggetto  basket";
            // 
            // cmbKind
            // 
            this.cmbKind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbKind.FormattingEnabled = true;
            this.cmbKind.Location = new System.Drawing.Point(6, 19);
            this.cmbKind.Name = "cmbKind";
            this.cmbKind.Size = new System.Drawing.Size(138, 21);
            this.cmbKind.TabIndex = 0;
            this.cmbKind.SelectedIndexChanged += new System.EventHandler(this.cmbKind_SelectedIndexChanged);
            // 
            // lblBorderColor
            // 
            this.lblBorderColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBorderColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblBorderColor.Location = new System.Drawing.Point(6, 76);
            this.lblBorderColor.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.lblBorderColor.Name = "lblBorderColor";
            this.lblBorderColor.Size = new System.Drawing.Size(39, 17);
            this.lblBorderColor.TabIndex = 10;
            this.lblBorderColor.Click += new System.EventHandler(this.lblBorderColor_Click);
            // 
            // lblFillColor
            // 
            this.lblFillColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFillColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblFillColor.Location = new System.Drawing.Point(6, 51);
            this.lblFillColor.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.lblFillColor.Name = "lblFillColor";
            this.lblFillColor.Size = new System.Drawing.Size(39, 17);
            this.lblFillColor.TabIndex = 9;
            this.lblFillColor.Click += new System.EventHandler(this.lblFillColor_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Colore bordo ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Colore oggetto";
            // 
            // OggettoBasketEditor2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grOpbjectBasket);
            this.Name = "OggettoBasketEditor2";
            this.Size = new System.Drawing.Size(150, 110);
            this.grOpbjectBasket.ResumeLayout(false);
            this.grOpbjectBasket.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grOpbjectBasket;
        private System.Windows.Forms.ComboBox cmbKind;
        private System.Windows.Forms.Label lblBorderColor;
        private System.Windows.Forms.Label lblFillColor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
