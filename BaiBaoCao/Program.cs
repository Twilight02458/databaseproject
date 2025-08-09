using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiBaoCao
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (FormLogin loginForm = new FormLogin())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new FormMenu(loginForm.LoggedInUser, loginForm.LoggedInUserId));
                }
            }
        }
    }
}
