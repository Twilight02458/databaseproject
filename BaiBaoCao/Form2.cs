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
    public partial class Form2 : Form
    {
        private readonly ResidentManagement residentManagement;
        public Form2()
        {
            InitializeComponent();
            residentManagement = new ResidentManagement();
            if (btnAddHousehold != null)
                btnAddHousehold.Click += btnAddHousehold_Click;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Form initialization code if needed
        }

        private void btnAddHousehold_Click(object sender, EventArgs e)
        {
            try
            {
                int? newHouseholdId = residentManagement.AddHousehold();
                if (newHouseholdId.HasValue)
                {
                    MessageBox.Show($"Thêm hộ gia đình thành công! Mã hộ: {newHouseholdId}", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không thể thêm hộ gia đình. Vui lòng thử lại!", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            // No inputs to clear after removing apartment ID
        }
    }
}
