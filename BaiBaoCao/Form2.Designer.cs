namespace BaiBaoCao
{
    partial class Form2
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtApartmentId = new System.Windows.Forms.TextBox();
            this.cmbRelationship = new System.Windows.Forms.ComboBox();
            this.btnAddHousehold = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(241, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(326, 42);
            this.label1.TabIndex = 0;
            this.label1.Text = "Thêm Hộ Gia Đình";
            // 
            // txtApartmentId
            // 
            this.txtApartmentId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtApartmentId.Location = new System.Drawing.Point(248, 122);
            this.txtApartmentId.Name = "txtApartmentId";
            this.txtApartmentId.Size = new System.Drawing.Size(170, 30);
            this.txtApartmentId.TabIndex = 1;
            // 
            // cmbRelationship
            // 
            this.cmbRelationship.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbRelationship.FormattingEnabled = true;
            this.cmbRelationship.Location = new System.Drawing.Point(248, 188);
            this.cmbRelationship.Name = "cmbRelationship";
            this.cmbRelationship.Size = new System.Drawing.Size(170, 33);
            this.cmbRelationship.TabIndex = 2;
            // 
            // btnAddHousehold
            // 
            this.btnAddHousehold.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddHousehold.Location = new System.Drawing.Point(464, 122);
            this.btnAddHousehold.Name = "btnAddHousehold";
            this.btnAddHousehold.Size = new System.Drawing.Size(108, 43);
            this.btnAddHousehold.TabIndex = 3;
            this.btnAddHousehold.Text = "Thêm hộ ";
            this.btnAddHousehold.UseVisualStyleBackColor = true;
            this.btnAddHousehold.Click += new System.EventHandler(this.btnAddHousehold_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(108, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Hộ:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(108, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "Mối Quan Hệ:";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(857, 506);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnAddHousehold);
            this.Controls.Add(this.cmbRelationship);
            this.Controls.Add(this.txtApartmentId);
            this.Controls.Add(this.label1);
            this.Name = "Form2";
            this.Text = "Thêm Hộ Gia Đình";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtApartmentId;
        private System.Windows.Forms.ComboBox cmbRelationship;
        private System.Windows.Forms.Button btnAddHousehold;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}