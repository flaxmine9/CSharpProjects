using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parse_VK
{
    public static class Description
    {
        private static string Login = "";
        private static string PassWord = "";

        public static string GetPassWord()
        {
            return PassWord;
        }

        public static string GetLogin()
        {
            return Login;
        }
    }
}
