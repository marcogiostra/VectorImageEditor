namespace partial VectorImageEditor.Controls
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnFillColor = new System.Windows.Forms.Button();
            this.btnSeamColor = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.lblTitle);
            this.flowLayoutPanel1.Controls.Add(this.btnFillColor);
            this.flowLayoutPanel1.Controls.Add(this.btnSeamColor);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(70, 15);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "BallBasket";
            // 
            // btnFillColor
            // 
            this.btnFillColor.Location = new System.Drawing.Point(3, 18);
            this.btnFillColor.Name = "btnFillColor";
            this.btnFillColor.Size = new System.Drawing.Size(160, 23);
            this.btnFillColor.TabIndex = 1;
            this.btnFillColor.Text = "Colore palla";
            this.btnFillColor.UseVisualStyleBackColor = true;
            // 
            // btnSeamColor
            // 
            this.btnSeamColor.Location = new System.Drawing.Point(3, 47);
            this.btnSeamColor.Name = "btnSeamColor";
            this.btnSeamColor.Size = new System.Drawing.Size(180, 23);
            this.btnSeamColor.TabIndex = 2;
            this.btnSeamColor.Text = "Colore bordo e cuciture";
            this.btnSeamColor.UseVisualStyleBackColor = true;
            // 
            // BallBasketEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "BallBasketEditor";
            this.Size = new System.Drawing.Size(200, 100);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnFillColor;
        private System.Windows.Forms.Button btnSeamColor;
    }
}