namespace PokemonCollection.Forms.Admin_Forms
{
    partial class PokellectorSetScraper
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
            this.txtSetUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnScrape = new System.Windows.Forms.Button();
            this.listBox = new System.Windows.Forms.ListBox();
            this.txtSetID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBarCards = new System.Windows.Forms.ProgressBar();
            this.labelDC = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtSetUrl
            // 
            this.txtSetUrl.Location = new System.Drawing.Point(160, 144);
            this.txtSetUrl.Name = "txtSetUrl";
            this.txtSetUrl.Size = new System.Drawing.Size(331, 20);
            this.txtSetUrl.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Pokellector URL";
            // 
            // btnScrape
            // 
            this.btnScrape.Location = new System.Drawing.Point(531, 143);
            this.btnScrape.Name = "btnScrape";
            this.btnScrape.Size = new System.Drawing.Size(75, 23);
            this.btnScrape.TabIndex = 2;
            this.btnScrape.Text = "Scrape Set";
            this.btnScrape.UseVisualStyleBackColor = true;
            this.btnScrape.Click += new System.EventHandler(this.btnScrape_Click);
            // 
            // listBox
            // 
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(29, 204);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(876, 277);
            this.listBox.TabIndex = 3;
            // 
            // txtSetID
            // 
            this.txtSetID.Location = new System.Drawing.Point(237, 44);
            this.txtSetID.Name = "txtSetID";
            this.txtSetID.Size = new System.Drawing.Size(100, 20);
            this.txtSetID.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(194, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Set ID";
            // 
            // progressBarCards
            // 
            this.progressBarCards.Location = new System.Drawing.Point(519, 44);
            this.progressBarCards.Name = "progressBarCards";
            this.progressBarCards.Size = new System.Drawing.Size(296, 61);
            this.progressBarCards.TabIndex = 5;
            // 
            // labelDC
            // 
            this.labelDC.AutoSize = true;
            this.labelDC.BackColor = System.Drawing.Color.Transparent;
            this.labelDC.Font = new System.Drawing.Font("Century Gothic", 15.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.labelDC.Location = new System.Drawing.Point(553, 108);
            this.labelDC.Name = "labelDC";
            this.labelDC.Size = new System.Drawing.Size(232, 25);
            this.labelDC.TabIndex = 6;
            this.labelDC.Text = "Downloading Cards...";
            // 
            // PokellectorSetScraper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(952, 652);
            this.Controls.Add(this.labelDC);
            this.Controls.Add(this.progressBarCards);
            this.Controls.Add(this.txtSetID);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.btnScrape);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSetUrl);
            this.Name = "PokellectorSetScraper";
            this.Text = "PokelllectorSetScraper";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSetUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnScrape;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.TextBox txtSetID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBarCards;
        private System.Windows.Forms.Label labelDC;
    }
}