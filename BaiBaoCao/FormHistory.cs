using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiBaoCao
{
    public partial class FormHistory : Form
    {
        private readonly ResidentManagement residentManagement;
        public FormHistory(ResidentManagement residentManagement)
        {
            InitializeComponent();
            this.residentManagement = new ResidentManagement();
        }

        private void FormHistory_Load(object sender, EventArgs e)
        {
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;
            LoadLoginHistory();
        }
        private void LoadLoginHistory()
        {
            try
            {
                DateTime? fromDate = dtpFromDate.Checked ? dtpFromDate.Value : (DateTime?)null;
                DateTime? toDate = dtpToDate.Checked ? dtpToDate.Value : (DateTime?)null;

                var history = residentManagement.GetLoginHistory(fromDate, toDate);
                dataGridView1.DataSource = history;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch sử: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadLoginHistory();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;
            LoadLoginHistory();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
