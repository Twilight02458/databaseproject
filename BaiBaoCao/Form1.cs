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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BaiBaoCao
{
    public partial class Form1 : Form
    {
        private ResidentManagement residentManagement;
        private readonly int userId;
        public Form1(string username)
        {
            InitializeComponent();
            residentManagement = new ResidentManagement();
            LoadResidents();
            LoadHouseholds();
            if (dataGridView1 != null)
                dataGridView1.CellClick += dataGridView1_CellClick;
            if (btnAdd != null)
                btnAdd.Click += btnAdd_Click_1;
            if (btnUpdate != null)
                btnUpdate.Click += btnUpdate_Click_1;
            if (btnDelete != null)
                btnDelete.Click += btnDelete_Click_1;
            if (btnSearch != null)
                btnSearch.Click += btnSearch_Click;
            if (cmbHousehold != null)
                cmbHousehold.SelectedIndexChanged += new EventHandler(cmbHousehold_SelectedIndexChanged);
            if (chkFilterDate != null)
                chkFilterDate.CheckedChanged += new EventHandler(chkFilterDate_CheckedChanged);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (userId != -1)
            {
                residentManagement.LogLogout(userId);
            }
        }
        private void LoadResidents()
        {
            int? householdId = null;
            if (cmbHousehold != null && cmbHousehold.SelectedIndex > 0)
            {
                var selectedItem = cmbHousehold.SelectedItem;
                if (selectedItem != null)
                {
                    householdId = (int?)selectedItem.GetType().GetProperty("Value").GetValue(selectedItem);
                }
            }

            DateTime? filterDate = null;
            if (chkFilterDate != null && chkFilterDate.Checked)
            {
                filterDate = dtpFilterDate.Value.Date;
            }

            residentManagement.DisplayResidents(dataGridView1, txtSearch?.Text, householdId, filterDate);
        }

        private void LoadHouseholds()
        {
            if (cmbHousehold != null)
            {
                residentManagement.LoadHouseholds(cmbHousehold);
            }
        }
        private void label5_Click(object sender, EventArgs e)
        {

        }
        //private void btnAdd_Click(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrWhiteSpace(txtName.Text))
        //    {
        //        MessageBox.Show("Vui lòng nhập họ tên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    DateTime? dateOfBirth = null;
        //    if (chkDateOfBirth != null && chkDateOfBirth.Checked)
        //    {
        //        dateOfBirth = dtpDateOfBirth.Value;
        //    }

        //    int? householdId = null;
        //    if (txtHouseholdId != null && int.TryParse(txtHouseholdId.Text, out int id))
        //    {
        //        householdId = id;
        //    }

        //    bool success = residentManagement.AddResident(
        //        txtName.Text,
        //        dateOfBirth,
        //        txtIdNumber.Text,
        //        txtPhone?.Text,
        //        txtEmail?.Text,
        //        householdId
        //    );

        //    if (success)
        //    {
        //        MessageBox.Show("Thêm cư dân thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        LoadResidents();
        //        ClearInputs();
        //    }
        //}

        // Sự kiện nút Sửa
        //private void btnUpdate_Click(object sender, EventArgs e)
        //{
        //    if (dataGridView1.SelectedRows.Count == 0)
        //    {
        //        MessageBox.Show("Vui lòng chọn một cư dân để sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    if (string.IsNullOrWhiteSpace(txtName.Text))
        //    {
        //        MessageBox.Show("Vui lòng nhập họ tên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    int residentId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
        //    DateTime? dateOfBirth = null;
        //    if (chkDateOfBirth != null && chkDateOfBirth.Checked)
        //    {
        //        dateOfBirth = dtpDateOfBirth.Value;
        //    }

        //    int? householdId = null;
        //    if (txtHouseholdId != null && int.TryParse(txtHouseholdId.Text, out int id))
        //    {
        //        householdId = id;
        //    }

        //    bool success = residentManagement.UpdateResident(
        //        residentId,
        //        txtName.Text,
        //        dateOfBirth,
        //        txtIdNumber.Text,
        //        txtPhone?.Text,
        //        txtEmail?.Text,
        //        householdId
        //    );

        //    if (success)
        //    {
        //        MessageBox.Show("Cập nhật cư dân thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        LoadResidents();
        //        ClearInputs();
        //    }
        //}

        // Sự kiện nút Xóa
        //private void btnDelete_Click(object sender, EventArgs e)
        //{
        //    if (dataGridView1.SelectedRows.Count == 0)
        //    {
        //        MessageBox.Show("Vui lòng chọn một cư dân để xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    int residentId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
        //    DialogResult result = MessageBox.Show($"Bạn có chắc muốn xóa cư dân ID {residentId}?", "Xác nhận",
        //                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        //    if (result == DialogResult.Yes)
        //    {
        //        bool success = residentManagement.DeleteResident(residentId);
        //        if (success)
        //        {
        //            MessageBox.Show("Xóa cư dân thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            LoadResidents();
        //            ClearInputs();
        //        }
        //    }
        //}

        // Sự kiện chọn dòng trên DataGridView
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtName.Text = row.Cells["Họ Tên"].Value.ToString();
                txtIdNumber.Text = row.Cells["CCCD"].Value == DBNull.Value ? "" : row.Cells["CCCD"].Value.ToString();
                
                if (txtPhone != null)
                    txtPhone.Text = row.Cells["SĐT"].Value == DBNull.Value ? "" : row.Cells["SĐT"].Value.ToString();
                
                if (txtEmail != null)
                    txtEmail.Text = row.Cells["Email"].Value == DBNull.Value ? "" : row.Cells["Email"].Value.ToString();
                
                if (row.Cells["Ngày Sinh"].Value != DBNull.Value)
                {
                    dtpDateOfBirth.Value = Convert.ToDateTime(row.Cells["Ngày Sinh"].Value);
                    if (chkDateOfBirth != null)
                        chkDateOfBirth.Checked = true;
                }
                else
                {
                    if (chkDateOfBirth != null)
                        chkDateOfBirth.Checked = false;
                }

                if (txtHouseholdId != null)
                {
                    using (MySqlConnection conn = new MySqlConnection(residentManagement.connectionString))
                    {
                        conn.Open();
                        string query = "SELECT household_id FROM residents WHERE resident_id = @residentId";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@residentId", row.Cells["ID"].Value);
                            object householdId = cmd.ExecuteScalar();
                            txtHouseholdId.Text = householdId == DBNull.Value ? "" : householdId.ToString();
                        }
                    }
                }
            }
        }

        // Xóa các trường nhập liệu
        private void ClearInputs()
        {
            txtName.Clear();
            txtIdNumber.Clear();
            if (txtPhone != null)
                txtPhone.Clear();
            if (txtEmail != null)
                txtEmail.Clear();
            if (txtHouseholdId != null)
                txtHouseholdId.Clear();
            if (chkDateOfBirth != null)
                chkDateOfBirth.Checked = false;
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime? dateOfBirth = null;
            if (chkDateOfBirth != null && chkDateOfBirth.Checked)
            {
                dateOfBirth = dtpDateOfBirth.Value;
            }

            int? householdId = null;
            if (txtHouseholdId != null && int.TryParse(txtHouseholdId.Text, out int id))
            {
                householdId = id;
            }

            bool success = residentManagement.AddResident(
                txtName.Text,
                dateOfBirth,
                txtIdNumber.Text,
                txtPhone?.Text,
                txtEmail?.Text,
                householdId
            );

            if (success)
            {
                MessageBox.Show("Thêm cư dân thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadResidents();
                ClearInputs();
            }
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một cư dân để xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int residentId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
            DialogResult result = MessageBox.Show($"Bạn có chắc muốn xóa cư dân ID {residentId}?", "Xác nhận",
                                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool success = residentManagement.DeleteResident(residentId);
                if (success)
                {
                    MessageBox.Show("Xóa cư dân thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadResidents();
                    ClearInputs();
                }
            }
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một cư dân để sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int residentId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
            DateTime? dateOfBirth = null;
            if (chkDateOfBirth != null && chkDateOfBirth.Checked)
            {
                dateOfBirth = dtpDateOfBirth.Value;
            }

            int? householdId = null;
            if (txtHouseholdId != null && int.TryParse(txtHouseholdId.Text, out int id))
            {
                householdId = id;
            }

            bool success = residentManagement.UpdateResident(
                residentId,
                txtName.Text,
                dateOfBirth,
                txtIdNumber.Text,
                txtPhone?.Text,
                txtEmail?.Text,
                householdId
            );

            if (success)
            {
                MessageBox.Show("Cập nhật cư dân thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadResidents();
                ClearInputs();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadResidents();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void cmbHousehold_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadResidents();
        }

        private void chkFilterDate_CheckedChanged(object sender, EventArgs e)
        {
            LoadResidents();
        }

        private void btnOpenHouseholdForm_Click(object sender, EventArgs e)
        {
            using (Form2 householdForm = new Form2())
            {
                householdForm.ShowDialog();
                LoadHouseholds(); // Refresh the household list after Form2 closes
            }
        }

        private void btnStatistics_Click(object sender, EventArgs e)
        {
            using (Form3 statisticsForm = new Form3())
            {
                statisticsForm.ShowDialog();
            }
        }

        private void btnBills_Click(object sender, EventArgs e)
        {
            using (Form4 billsForm = new Form4())
            {
                billsForm.ShowDialog();
            }
        }
    }
}

