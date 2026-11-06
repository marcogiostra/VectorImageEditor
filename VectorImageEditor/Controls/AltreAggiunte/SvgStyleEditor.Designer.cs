namespace VectorImageEditor.Controls.AltreAggiunte
{
    partial class SvgStyleEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnLoadSvg = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.btnLoadSvg);
            this.groupBox1.Location = new System.Drawing.Point(1, 1);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(1);
            this.groupBox1.Size = new System.Drawing.Size(89, 56);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SVG Style";
            // 
            // btnLoadSvg
            // 
            this.btnLoadSvg.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadSvg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadSvg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnLoadSvg.Location = new System.Drawing.Point(10, 21);
            this.btnLoadSvg.Margin = new System.Windows.Forms.Padding(0);
            this.btnLoadSvg.Name = "btnLoadSvg";
            this.btnLoadSvg.Size = new System.Drawing.Size(57, 21);
            this.btnLoadSvg.TabIndex = 7;
            this.btnLoadSvg.Text = "Load...";
            this.btnLoadSvg.UseVisualStyleBackColor = true;
            this.btnLoadSvg.Click += new System.EventHandler(this.btnLoadSvg_Click);
            // 
            // SvgStyleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox1);
            this.Name = "SvgStyleEditor";
            this.Size = new System.Drawing.Size(94, 62);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnLoadSvg;
    }
}
