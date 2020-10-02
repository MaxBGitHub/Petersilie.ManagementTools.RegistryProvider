using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petersilie.ManagementTools.RegistryProvider
{
    static class MMain
    {
        public static void Main(string[] args)
        {
            var regBee = new RegistryNode(
                RegView.x64, 
                RegHive.HKEY_CURRENT_USER);

            regBee.BuildHive(6);

            var foundBee = regBee.Find(bee => bee.Key.Contains("Default"));
        }
    }
}
