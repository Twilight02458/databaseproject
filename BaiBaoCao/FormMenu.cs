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
    public partial class FormMenu : Form
    {
        private readonly ResidentManagement residentManagement;
        private readonly string username;
        private readonly int userId;
        public FormMenu(string username, int userId)
        {
            InitializeComponent();
            this.username = username;
            this.userId = userId;
            this.residentManagement = new ResidentManagement();
        }

        private void FormMenu_Load(object sender, EventArgs e)
        {

        }

        private void btnResidentManagement_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(username, userId);
            form1.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine($"[DEBUG] btnExit_Click - Starting logout process for user ID: {userId}");

                // Only proceed if we have a valid user ID (greater than 0)
                if (userId > 0)
                {
                    Console.WriteLine($"[DEBUG] Valid user ID: {userId}");

                    // Check if the user exists before attempting to log out
                    Console.WriteLine($"[DEBUG] Calling GetUserId for user ID: {userId}");
                    int validUserId = residentManagement.GetUserId(userId.ToString());

                    if (validUserId > 0)
                    {
                        Console.WriteLine($"[DEBUG] User exists, calling LogLogout for user ID: {userId}");
                        bool logoutResult = residentManagement.LogLogout(userId);
                        Console.WriteLine($"[DEBUG] LogLogout result: {logoutResult}");
                    }
                    else
                    {
                        Console.WriteLine($"[DEBUG] User ID {userId} not found in database");
                    }
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Invalid user ID: {userId}");
                }

                Console.WriteLine("[DEBUG] Closing application");
                Application.Exit(); // Use Application.Exit() instead of Close() to ensure proper shutdown
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error during logout: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException.Message}");
                }
                Application.Exit();
            }
        }

        private void btnStatistic_Click(object sender, EventArgs e)
        {
            using (Form3 statisticsForm = new Form3())
            {
                statisticsForm.ShowDialog();
            }
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            var historyForm = new FormHistory(residentManagement);
            historyForm.ShowDialog();
        }
    }
}
