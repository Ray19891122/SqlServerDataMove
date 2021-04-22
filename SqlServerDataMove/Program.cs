using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlServerDataMove
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] pars)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (pars != null && pars.Length > 0)
            {
                string[] parsList = pars[0].Split('|');
                Application.Run(new LoginForm(parsList));
            }
            else
                Application.Run(new LoginForm());
        }
    }
}
