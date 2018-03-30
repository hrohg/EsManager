using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ES.Common.Helpers
{
    public class PasswordHelper
    {
        public static string Convert(SecureString secStr)
        {
            IntPtr passwordBSTR = default(IntPtr);
            string insecurePassword = "";
            try
            {
                passwordBSTR = Marshal.SecureStringToBSTR(secStr);
                insecurePassword = Marshal.PtrToStringBSTR(passwordBSTR);
            }
            catch
            {
                insecurePassword = "";
            }
            return insecurePassword;
        }
    }
}
