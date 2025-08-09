using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BaiBaoCao
{
    public partial class Form3 : Form
    {
        private readonly ResidentManagement residentManagement;
        public Form3()
        {
            InitializeComponent();
            residentManagement = new ResidentManagement();
            LoadStatistics();

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
        private void LoadStatistics()
        {
            try
            {
                // Tổng số cư dân
                int totalResidents = GetTotalResidents();
                lblTotalResidents.Text = $"Tổng số cư dân: {totalResidents}";

                // Tổng số hộ gia đình
                int totalHouseholds = GetTotalHouseholds();
                lblTotalHouseholds.Text = $"Tổng số hộ gia đình: {totalHouseholds}";

                // Thống kê nhóm tuổi
                LoadAgeGroupChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}\nStack Trace: {ex.StackTrace}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetTotalResidents()
        {
            using (MySqlConnection conn = new MySqlConnection(residentManagement.connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM residents";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        private int GetTotalHouseholds()
        {
            using (MySqlConnection conn = new MySqlConnection(residentManagement.connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM households";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        private void LoadAgeGroupChart()
        {
            DataTable ageGroups = new DataTable();
            ageGroups.Columns.Add("AgeGroup", typeof(string));
            ageGroups.Columns.Add("Count", typeof(int));

            using (MySqlConnection conn = new MySqlConnection(residentManagement.connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        CASE 
                            WHEN DATEDIFF(CURDATE(), date_of_birth)/365 < 18 THEN 'Dưới 18'
                            WHEN DATEDIFF(CURDATE(), date_of_birth)/365 BETWEEN 18 AND 60 THEN '18-60'
                            ELSE 'Trên 60'
                        END AS AgeGroup,
                        COUNT(*) AS Count
                    FROM residents
                    WHERE date_of_birth IS NOT NULL
                    GROUP BY AgeGroup";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(ageGroups);
                    }
                }
            }

            // Tạo biểu đồ
            chartAgeGroups.Series.Clear();
            chartAgeGroups.Series.Add("AgeGroups");
            chartAgeGroups.Series["AgeGroups"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
            chartAgeGroups.Series["AgeGroups"].Color = System.Drawing.Color.FromArgb(65, 105, 225); // RoyalBlue
            chartAgeGroups.Series["AgeGroups"].IsValueShownAsLabel = true;

            foreach (DataRow row in ageGroups.Rows)
            {
                chartAgeGroups.Series["AgeGroups"].Points.AddXY(row["AgeGroup"], row["Count"]);
            }

            chartAgeGroups.ChartAreas[0].AxisX.Title = "Nhóm tuổi";
            chartAgeGroups.ChartAreas[0].AxisY.Title = "Số lượng";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadStatistics();
        }
    }
}
    
