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
    public partial class Form2 : Form
    {
        private readonly ResidentManagement residentManagement;
        public Form2()
        {
            InitializeComponent();
            residentManagement = new ResidentManagement();
            if (cmbRelationship != null)
            {
                cmbRelationship.Items.AddRange(new object[] { "Head", "Member" });
                cmbRelationship.SelectedIndex = 0;
            }
            if (btnAddHousehold != null)
                btnAddHousehold.Click += btnAddHousehold_Click;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void btnAddHousehold_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtApartmentId.Text))
            {
                MessageBox.Show("Vui lòng nhập ID căn hộ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtApartmentId.Text, out int apartmentId))
            {
                MessageBox.Show("ID căn hộ phải là một số nguyên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!residentManagement.IsValidApartmentId(apartmentId))
            {
                MessageBox.Show("Căn hộ không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string relationship = cmbRelationship.SelectedItem?.ToString();

            bool success = residentManagement.AddHousehold(apartmentId, relationship);
            if (success)
            {
                MessageBox.Show("Thêm hộ gia đình thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                this.Close(); // Close Form2 after successful addition
            }
        }
        private void ClearInputs()
        {
            if (txtApartmentId != null)
                txtApartmentId.Clear();
            if (cmbRelationship != null)
                cmbRelationship.SelectedIndex = 0;
        }
    }
}
