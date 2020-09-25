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
			bool granted;
			var reg64 = new Registry64();
			reg64.GetPermission(RegHive.HKEY_LOCAL_MACHINE,
				@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
				3, out granted);
		}
	}
}
