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
    public partial class Form4 : Form
    {
        private readonly ResidentManagement residentManagement;
        

        public Form4()
        {
            InitializeComponent();
            residentManagement = new ResidentManagement();
            LoadHouseholds();
            LoadBills();
            cbStatus.Items.AddRange(new[] { "pending", "paid", "overdue" });
            cbStatus.SelectedIndex = 0;
            cbFilterStatus.Items.AddRange(new[] { "Tất cả", "pending", "paid", "overdue" });
            cbFilterStatus.SelectedIndex = 0;
            nudMonth.Minimum = 1;
            nudMonth.Maximum = 12;
            nudYear.Minimum = 2000;
            nudYear.Maximum = DateTime.Now.Year + 1;
            nudYear.Value = DateTime.Now.Year;
        }


        private void Form4_Load(object sender, EventArgs e)
        {

        }
        private void LoadHouseholds()
        {
            residentManagement.LoadHouseholds(cbHousehold);
        }

        private void LoadBills()
        {
            int? householdId = cbHousehold.SelectedIndex > 0 ? (int?)cbHousehold.SelectedValue : null;
            string status = cbFilterStatus.SelectedIndex > 0 ? cbFilterStatus.SelectedItem.ToString() : null;
            int? month = nudFilterMonth.Value > 0 ? (int?)nudFilterMonth.Value : null;
            int? year = nudFilterYear.Value > 0 ? (int?)nudFilterYear.Value : null;
            residentManagement.DisplayBills(dataGridViewBills, householdId, status, month, year);
        }


        private bool ValidateInput()
        {
            if (cbHousehold.SelectedIndex <= 0 || cbHousehold.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn một hộ gia đình hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtServiceType.Text))
            {
                MessageBox.Show("Vui lòng nhập loại phí!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtAmount.Text) || !decimal.TryParse(txtAmount.Text, out _))
            {
                MessageBox.Show("Vui lòng nhập số tiền hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (nudMonth.Value < 1 || nudMonth.Value > 12)
            {
                MessageBox.Show("Tháng phải từ 1 đến 12!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (nudYear.Value < 2000 || nudYear.Value > DateTime.Now.Year + 1)
            {
                MessageBox.Show($"Năm phải từ 2000 đến {DateTime.Now.Year + 1}!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void ClearInputs()
        {
            cbHousehold.SelectedIndex = 0;
            txtServiceType.Clear();
            txtAmount.Clear();
            nudMonth.Value = 1;
            nudYear.Value = DateTime.Now.Year;
            cbStatus.SelectedIndex = 0;
            dtpDueDate.Value = DateTime.Now.AddDays(14);
        }

        private void dataGridViewBills_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewBills.SelectedRows.Count > 0)
            {
                var row = dataGridViewBills.SelectedRows[0];
                cbHousehold.SelectedValue = Convert.ToInt32(row.Cells["Hộ Gia Đình"].Value);
                txtServiceType.Text = row.Cells["Loại Phí"].Value.ToString();
                txtAmount.Text = row.Cells["Số Tiền"].Value.ToString();
                nudMonth.Value = Convert.ToInt32(row.Cells["Tháng"].Value);
                nudYear.Value = Convert.ToInt32(row.Cells["Năm"].Value);
                cbStatus.SelectedItem = row.Cells["Trạng Thái"].Value.ToString();
                dtpDueDate.Value = Convert.ToDateTime(row.Cells["Ngày Đến Hạn"].Value);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Debug: Show ComboBox state
                string debugInfo = $"Items count: {cbHousehold.Items.Count}\n";
                debugInfo += $"SelectedIndex: {cbHousehold.SelectedIndex}\n";
                debugInfo += $"SelectedValue: {cbHousehold.SelectedValue}\n";
                debugInfo += $"SelectedItem: {cbHousehold.SelectedItem}\n";
                
                if (cbHousehold.SelectedItem != null)
                {
                    var selectedItem = (DataRowView)cbHousehold.SelectedItem;
                    debugInfo += $"Selected Item Value: {selectedItem["Value"]}\n";
                    debugInfo += $"Selected Item Text: {selectedItem["Text"]}\n";
                }
                
                Console.WriteLine(debugInfo);
                MessageBox.Show(debugInfo, "Debug - Before ValidateInput");

                if (!ValidateInput()) 
                {
                    MessageBox.Show("Validation failed", "Debug");
                    return;
                }

                // Get the selected value from the DataRowView
                if (cbHousehold.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn một hộ gia đình!", "Lỗi", 
                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedHousehold = (DataRowView)cbHousehold.SelectedItem;
                int? householdId = selectedHousehold["Value"] as int?;

                if (!householdId.HasValue)
                {
                    MessageBox.Show("Hộ gia đình không hợp lệ!", "Lỗi", 
                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string serviceType = txtServiceType.Text.Trim();
                if (!decimal.TryParse(txtAmount.Text, out decimal amount))
                {
                    MessageBox.Show("Vui lòng nhập số tiền hợp lệ!", "Lỗi", 
                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int month = (int)nudMonth.Value;
                int year = (int)nudYear.Value;
                string status = cbStatus.SelectedItem?.ToString() ?? "pending";
                DateTime dueDate = dtpDueDate.Value;

                if (residentManagement.AddBill(householdId.Value, serviceType, amount, month, year, status, dueDate))
                {
                    MessageBox.Show("Thêm hóa đơn thành công!", "Thành công", 
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBills();
                    
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Không thể thêm hóa đơn. Vui lòng thử lại!", "Lỗi", 
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}\n\nChi tiết: {ex.StackTrace}", 
                      "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewBills.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn hóa đơn để sửa!", "Cảnh báo",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInput())
                {
                    return;
                }

                // Get the selected bill ID
                int billId = Convert.ToInt32(dataGridViewBills.SelectedRows[0].Cells["ID"].Value);

                // Get the selected household
                if (cbHousehold.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn một hộ gia đình!", "Lỗi",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedHousehold = (DataRowView)cbHousehold.SelectedItem;
                if (selectedHousehold["Value"] == DBNull.Value || selectedHousehold["Value"] == null)
                {
                    MessageBox.Show("Hộ gia đình không hợp lệ!", "Lỗi",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int householdId = Convert.ToInt32(selectedHousehold["Value"]);
                string serviceType = txtServiceType.Text.Trim();

                if (!decimal.TryParse(txtAmount.Text, out decimal amount))
                {
                    MessageBox.Show("Số tiền không hợp lệ!", "Lỗi",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int month = (int)nudMonth.Value;
                int year = (int)nudYear.Value;
                string status = cbStatus.SelectedItem?.ToString() ?? "pending";
                DateTime dueDate = dtpDueDate.Value;

                if (residentManagement.UpdateBill(billId, householdId, serviceType, amount, month, year, status, dueDate))
                {
                    MessageBox.Show("Cập nhật hóa đơn thành công!", "Thành công",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBills();
                    
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật hóa đơn. Vui lòng thử lại!", "Lỗi",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi cập nhật hóa đơn: {ex.Message}\n\nChi tiết: {ex.StackTrace}",
                              "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewBills.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn hóa đơn để xóa!", "Cảnh báo",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get the selected bill details for confirmation message
                var selectedRow = dataGridViewBills.SelectedRows[0];
                string serviceType = selectedRow.Cells["Loại Phí"].Value?.ToString() ?? "";
                string amount = selectedRow.Cells["Số Tiền"].Value?.ToString() ?? "";
                string month = selectedRow.Cells["Tháng"].Value?.ToString() ?? "";
                string year = selectedRow.Cells["Năm"].Value?.ToString() ?? "";

                string confirmationMessage = $"Bạn có chắc chắn muốn xóa hóa đơn này?\n\n" +
                                           $"Loại phí: {serviceType}\n" +
                                           $"Số tiền: {amount}\n" +
                                           $"Tháng/Năm: {month}/{year}";

                if (MessageBox.Show(confirmationMessage, "Xác nhận xóa",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int billId = Convert.ToInt32(selectedRow.Cells["ID"].Value);

                    if (residentManagement.DeleteBill(billId))
                    {
                        MessageBox.Show("Xóa hóa đơn thành công!", "Thành công",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadBills();
                        
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa hóa đơn. Vui lòng thử lại!", "Lỗi",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi xóa hóa đơn: {ex.Message}\n\nChi tiết: {ex.StackTrace}",
                              "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            LoadBills();
           
        }
    }
}
