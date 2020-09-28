using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Petersilie.ManagementTools.RegistryProvider
{
    static class Program
    {
        public static void Main()
        {
            var reg64 = new Registry64();
            bool retVal;

            retVal = reg64.CreateKey(RegHive.HKEY_CURRENT_USER, "Test\\TestSubKey");
            retVal = reg64.DeleteKey(RegHive.HKEY_CURRENT_USER, "Test\\TestSubKey");
        }
    }
}
