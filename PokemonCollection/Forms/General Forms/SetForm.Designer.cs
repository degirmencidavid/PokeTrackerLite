namespace PokemonCollection.Forms.General_Forms
{
    partial class SetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvSets = new System.Windows.Forms.DataGridView();
            this.SetID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SetName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalCards = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Released = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SetImage = new System.Windows.Forms.DataGridViewImageColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSets)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSets
            // 
            this.dgvSets.AllowUserToAddRows = false;
            this.dgvSets.AllowUserToDeleteRows = false;
            this.dgvSets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSets.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SetID,
            this.SetName,
            this.TotalCards,
            this.Released,
            this.SetImage});
            this.dgvSets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSets.Location = new System.Drawing.Point(0, 0);
            this.dgvSets.Name = "dgvSets";
            this.dgvSets.Size = new System.Drawing.Size(800, 531);
            this.dgvSets.TabIndex = 0;
            // 
            // SetID
            // 
            this.SetID.HeaderText = "Set ID";
            this.SetID.Name = "SetID";
            this.SetID.ReadOnly = true;
            // 
            // SetName
            // 
            this.SetName.HeaderText = "Set Name";
            this.SetName.Name = "SetName";
            this.SetName.ReadOnly = true;
            // 
            // TotalCards
            // 
            this.TotalCards.HeaderText = "Total Cards";
            this.TotalCards.Name = "TotalCards";
            this.TotalCards.ReadOnly = true;
            // 
            // Released
            // 
            this.Released.HeaderText = "Release Date";
            this.Released.Name = "Released";
            this.Released.ReadOnly = true;
            // 
            // SetImage
            // 
            this.SetImage.HeaderText = "Image";
            this.SetImage.Name = "SetImage";
            this.SetImage.ReadOnly = true;
            this.SetImage.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // SetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 531);
            this.Controls.Add(this.dgvSets);
            this.Name = "SetForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SetForm";
            this.Load += new System.EventHandler(this.SetForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSets)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSets;
        private System.Windows.Forms.DataGridViewTextBoxColumn SetID;
        private System.Windows.Forms.DataGridViewTextBoxColumn SetName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalCards;
        private System.Windows.Forms.DataGridViewTextBoxColumn Released;
        private System.Windows.Forms.DataGridViewImageColumn SetImage;
    }
}