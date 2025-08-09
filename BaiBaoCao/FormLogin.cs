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
    public partial class FormLogin : Form
    {
        private readonly ResidentManagement residentManagement;
        public string LoggedInUser { get; private set; }
        public int LoggedInUserId { get; private set; } = -1;
        public FormLogin()
        {
            InitializeComponent();
            residentManagement = new ResidentManagement();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
        //private void btnLogin_Click(object sender, EventArgs e)
        //{
            
        //}

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (residentManagement.ValidateUser(username, password))
            {
                LoggedInUser = username;
                LoggedInUserId = residentManagement.GetUserId(username);
                if (LoggedInUserId != -1)
                {
                    residentManagement.LogLogin(LoggedInUserId);
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
