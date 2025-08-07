using System.Windows.Forms.DataVisualization.Charting;

namespace BaiBaoCao
{
    partial class Form3
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartAgeGroups = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblTotalResidents = new System.Windows.Forms.Label();
            this.lblTotalHouseholds = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartAgeGroups)).BeginInit();
            this.SuspendLayout();
            // 
            // chartAgeGroups
            // 
            chartArea1.Name = "ChartArea1";
            this.chartAgeGroups.ChartAreas.Add(chartArea1);

            legend1.Name = "Legend1";
            this.chartAgeGroups.Legends.Add(legend1);

            this.chartAgeGroups.Location = new System.Drawing.Point(57, 37);
            this.chartAgeGroups.Name = "chartAgeGroups";

            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.ChartType = SeriesChartType.Column; // ✅ DÒNG QUAN TRỌNG NHẤT


            this.chartAgeGroups.Series.Add(series1);

            this.chartAgeGroups.Size = new System.Drawing.Size(523, 405);
            this.chartAgeGroups.TabIndex = 0;
            this.chartAgeGroups.Text = "chart1";

            // 
            // lblTotalResidents
            // 
            this.lblTotalResidents.AutoSize = true;
            this.lblTotalResidents.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalResidents.Location = new System.Drawing.Point(586, 50);
            this.lblTotalResidents.Name = "lblTotalResidents";
            this.lblTotalResidents.Size = new System.Drawing.Size(79, 29);
            this.lblTotalResidents.TabIndex = 1;
            this.lblTotalResidents.Text = "label1";
            // 
            // lblTotalHouseholds
            // 
            this.lblTotalHouseholds.AutoSize = true;
            this.lblTotalHouseholds.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalHouseholds.Location = new System.Drawing.Point(586, 106);
            this.lblTotalHouseholds.Name = "lblTotalHouseholds";
            this.lblTotalHouseholds.Size = new System.Drawing.Size(79, 29);
            this.lblTotalHouseholds.TabIndex = 2;
            this.lblTotalHouseholds.Text = "label2";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1151, 755);
            this.Controls.Add(this.lblTotalHouseholds);
            this.Controls.Add(this.lblTotalResidents);
            this.Controls.Add(this.chartAgeGroups);
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartAgeGroups)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartAgeGroups;
        private System.Windows.Forms.Label lblTotalResidents;
        private System.Windows.Forms.Label lblTotalHouseholds;
    }
}