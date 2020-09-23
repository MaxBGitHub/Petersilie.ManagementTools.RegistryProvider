using System.Management;
using System.Security;

namespace Petersilie.ManagementTools.RegistryProvider
{
	/* Creates different instances of the StdRegProv
	** WMI class which can be then used to access the
	** Registry of a machine. */
	internal class ManagementProvider
	{
		/* StdRegProv class is preinstalled in the WMI
		** Namespace root\default and root\cimv2. 
		** This class provides all methods needed to 
		** access the registry on a local or remote machine. */
		const string PROVIDER = "StdRegProv";

		/* The WMI namespace which contains the preinstalled
		** StdRegProv class. */
		const string NAMESPACE = "root\\default";


		/* Creates an instance of the StdRegProv class
		** for the local machine. */
		public static ManagementClass StdRegMgmtProvider()
		{
			var options = new ConnectionOptions() {
				Impersonation		= ImpersonationLevel.Impersonate,
				EnablePrivileges	= true
			};

			var scope = new ManagementScope(NAMESPACE, options);
			var path = new ManagementPath(PROVIDER);

			return new ManagementClass(scope, path, null);
		}


		/* Creates an instance of the StdRegProv class
		** for a remote machine without credentials to 
		** authenticate on the machine (use this when 
		** your app is run with domain admin account). */
		public static ManagementClass StdRegMgmtProvider(string machine)
		{
			var options = new ConnectionOptions() {
				Impersonation		= ImpersonationLevel.Impersonate,
				EnablePrivileges	= true
			};

			var scope = new ManagementScope($"\\\\{machine}{NAMESPACE}", options);
			var path = new ManagementPath(PROVIDER);

			return new ManagementClass(scope, path, null);
		}


		/* Creates an instance of the StdRegProv class 
		** for a remote machine with credentials to 
		** authenticate on the machine (use this when
		** your app is run with a normal user account). */
		public static ManagementClass StdRegMgmtProvider(string machine, 
			string userName, SecureString password)
		{
			var options = new ConnectionOptions() {
				Impersonation		= ImpersonationLevel.Impersonate,
				Username			= userName,
				SecurePassword		= password,
				EnablePrivileges	= true
			};

			var scope = new ManagementScope($"\\\\{machine}{NAMESPACE}", options);
			var path = new ManagementPath(PROVIDER);

			return new ManagementClass(scope, path, null);
		}
	}
}
