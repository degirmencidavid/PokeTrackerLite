namespace PokemonCollection.Forms.User_Forms
{
    partial class CollectionForm
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
            this.dgvCollection = new System.Windows.Forms.DataGridView();
            this.CardID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CardName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CardLanguage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SetNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SetCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CardPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OwnedA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OwnedAM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OwnedB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OwnedC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OwnedPSA10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OwnedPSA9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OwnedPSA8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCollection)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvCollection
            // 
            this.dgvCollection.AllowUserToAddRows = false;
            this.dgvCollection.AllowUserToDeleteRows = false;
            this.dgvCollection.AllowUserToOrderColumns = true;
            this.dgvCollection.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCollection.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CardID,
            this.CardName,
            this.CardLanguage,
            this.SetNumber,
            this.SetCode,
            this.CardPrice,
            this.OwnedA,
            this.OwnedAM,
            this.OwnedB,
            this.OwnedC,
            this.OwnedPSA10,
            this.OwnedPSA9,
            this.OwnedPSA8});
            this.dgvCollection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCollection.Location = new System.Drawing.Point(0, 0);
            this.dgvCollection.Name = "dgvCollection";
            this.dgvCollection.Size = new System.Drawing.Size(1352, 472);
            this.dgvCollection.TabIndex = 1;
            // 
            // CardID
            // 
            this.CardID.HeaderText = "ID";
            this.CardID.Name = "CardID";
            this.CardID.ReadOnly = true;
            // 
            // CardName
            // 
            this.CardName.HeaderText = "Card Name";
            this.CardName.Name = "CardName";
            this.CardName.ReadOnly = true;
            // 
            // CardLanguage
            // 
            this.CardLanguage.HeaderText = "Language";
            this.CardLanguage.Name = "CardLanguage";
            this.CardLanguage.ReadOnly = true;
            // 
            // SetNumber
            // 
            this.SetNumber.HeaderText = "Set Number";
            this.SetNumber.Name = "SetNumber";
            this.SetNumber.ReadOnly = true;
            // 
            // SetCode
            // 
            this.SetCode.HeaderText = "Set ID";
            this.SetCode.Name = "SetCode";
            this.SetCode.ReadOnly = true;
            // 
            // CardPrice
            // 
            this.CardPrice.HeaderText = "Estimated Price";
            this.CardPrice.Name = "CardPrice";
            this.CardPrice.ReadOnly = true;
            // 
            // OwnedA
            // 
            this.OwnedA.HeaderText = "A";
            this.OwnedA.Name = "OwnedA";
            this.OwnedA.ReadOnly = true;
            // 
            // OwnedAM
            // 
            this.OwnedAM.HeaderText = "A-";
            this.OwnedAM.Name = "OwnedAM";
            this.OwnedAM.ReadOnly = true;
            // 
            // OwnedB
            // 
            this.OwnedB.HeaderText = "B";
            this.OwnedB.Name = "OwnedB";
            this.OwnedB.ReadOnly = true;
            // 
            // OwnedC
            // 
            this.OwnedC.HeaderText = "C";
            this.OwnedC.Name = "OwnedC";
            this.OwnedC.ReadOnly = true;
            // 
            // OwnedPSA10
            // 
            this.OwnedPSA10.HeaderText = "PSA 10";
            this.OwnedPSA10.Name = "OwnedPSA10";
            this.OwnedPSA10.ReadOnly = true;
            // 
            // OwnedPSA9
            // 
            this.OwnedPSA9.HeaderText = "PSA 9";
            this.OwnedPSA9.Name = "OwnedPSA9";
            this.OwnedPSA9.ReadOnly = true;
            // 
            // OwnedPSA8
            // 
            this.OwnedPSA8.HeaderText = "PSA 8";
            this.OwnedPSA8.Name = "OwnedPSA8";
            this.OwnedPSA8.ReadOnly = true;
            // 
            // CollectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1352, 472);
            this.Controls.Add(this.dgvCollection);
            this.Name = "CollectionForm";
            this.Text = "CollectionForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCollection)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCollection;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardLanguage;
        private System.Windows.Forms.DataGridViewTextBoxColumn SetNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn SetCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn OwnedA;
        private System.Windows.Forms.DataGridViewTextBoxColumn OwnedAM;
        private System.Windows.Forms.DataGridViewTextBoxColumn OwnedB;
        private System.Windows.Forms.DataGridViewTextBoxColumn OwnedC;
        private System.Windows.Forms.DataGridViewTextBoxColumn OwnedPSA10;
        private System.Windows.Forms.DataGridViewTextBoxColumn OwnedPSA9;
        private System.Windows.Forms.DataGridViewTextBoxColumn OwnedPSA8;
    }
}