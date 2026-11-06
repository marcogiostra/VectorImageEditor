namespace VectorImageEditor.Controls.Basket
{
    sealed partial class BallBasketEditor
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
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
        /// Metodo necessario per il supporto della finestra di progettazione.
        /// Non modificare il contenuto di questo metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.grBallBasket = new System.Windows.Forms.GroupBox();
            this.lblSeamColor = new System.Windows.Forms.Label();
            this.lblFillColor = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grBallBasket.SuspendLayout();
            this.SuspendLayout();
            // 
            // grBallBasket
            // 
            this.grBallBasket.Controls.Add(this.lblSeamColor);
            this.grBallBasket.Controls.Add(this.lblFillColor);
            this.grBallBasket.Controls.Add(this.label2);
            this.grBallBasket.Controls.Add(this.label1);
            this.grBallBasket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grBallBasket.Location = new System.Drawing.Point(0, 0);
            this.grBallBasket.Name = "grBallBasket";
            this.grBallBasket.Size = new System.Drawing.Size(187, 76);
            this.grBallBasket.TabIndex = 1;
            this.grBallBasket.TabStop = false;
            this.grBallBasket.Text = "Pallone basket";
            // 
            // lblSeamColor
            // 
            this.lblSeamColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSeamColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblSeamColor.Location = new System.Drawing.Point(7, 46);
            this.lblSeamColor.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.lblSeamColor.Name = "lblSeamColor";
            this.lblSeamColor.Size = new System.Drawing.Size(39, 17);
            this.lblSeamColor.TabIndex = 6;
            this.lblSeamColor.Click += new System.EventHandler(this.lblSeamColor_Click);
            // 
            // lblFillColor
            // 
            this.lblFillColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFillColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblFillColor.Location = new System.Drawing.Point(7, 25);
            this.lblFillColor.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.lblFillColor.Name = "lblFillColor";
            this.lblFillColor.Size = new System.Drawing.Size(39, 17);
            this.lblFillColor.TabIndex = 5;
            this.lblFillColor.Click += new System.EventHandler(this.lbFillColor_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Colore bordo e cuciture";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Colore palla";
            // 
            // BallBasketEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grBallBasket);
            this.Name = "BallBasketEditor";
            this.Size = new System.Drawing.Size(187, 76);
            this.grBallBasket.ResumeLayout(false);
            this.grBallBasket.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox grBallBasket;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSeamColor;
        private System.Windows.Forms.Label lblFillColor;
    }    
}
