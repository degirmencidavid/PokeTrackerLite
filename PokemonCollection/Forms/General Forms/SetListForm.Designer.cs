namespace PokemonCollection.Forms.General_Forms
{
    partial class SetListForm
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
            this.dgvCards = new System.Windows.Forms.DataGridView();
            this.CardID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CardName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CardLanguage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SetNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SetCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CardPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnCloseCards = new System.Windows.Forms.Button();
            this.pbSet = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCards)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSet)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvCards
            // 
            this.dgvCards.AllowUserToAddRows = false;
            this.dgvCards.AllowUserToDeleteRows = false;
            this.dgvCards.AllowUserToOrderColumns = true;
            this.dgvCards.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCards.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CardID,
            this.CardName,
            this.CardLanguage,
            this.SetNumber,
            this.SetCode,
            this.CardPrice});
            this.dgvCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCards.Location = new System.Drawing.Point(0, 0);
            this.dgvCards.Name = "dgvCards";
            this.dgvCards.Size = new System.Drawing.Size(862, 473);
            this.dgvCards.TabIndex = 0;
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
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(0, 0);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(43, 35);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "SETS";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnCloseCards
            // 
            this.btnCloseCards.Location = new System.Drawing.Point(654, 12);
            this.btnCloseCards.Name = "btnCloseCards";
            this.btnCloseCards.Size = new System.Drawing.Size(134, 48);
            this.btnCloseCards.TabIndex = 2;
            this.btnCloseCards.Text = "CLOSE ALL CARDS";
            this.btnCloseCards.UseVisualStyleBackColor = true;
            this.btnCloseCards.Click += new System.EventHandler(this.btnCloseCards_Click);
            // 
            // pbSet
            // 
            this.pbSet.BackColor = System.Drawing.Color.Transparent;
            this.pbSet.Location = new System.Drawing.Point(643, 92);
            this.pbSet.Name = "pbSet";
            this.pbSet.Size = new System.Drawing.Size(195, 127);
            this.pbSet.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbSet.TabIndex = 3;
            this.pbSet.TabStop = false;
            // 
            // SetListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 473);
            this.Controls.Add(this.pbSet);
            this.Controls.Add(this.btnCloseCards);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.dgvCards);
            this.Name = "SetListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SetListForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCards)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCards;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnCloseCards;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardLanguage;
        private System.Windows.Forms.DataGridViewTextBoxColumn SetNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn SetCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardPrice;
        private System.Windows.Forms.PictureBox pbSet;
    }
}