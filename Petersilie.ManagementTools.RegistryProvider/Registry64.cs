using System.Security;

namespace Petersilie.ManagementTools.RegistryProvider
{ 
	public class Registry64 : RegistryBase
	{
		/// <summary>
		/// <see cref="RegView.x64"/>
		/// </summary>
		protected override RegView ViewFlag
		{
			get {
				return RegView.x86;
			}
		}


		/// <summary>
		/// Remote registry constructor with credentials.
		/// </summary>
		/// <param name="machine"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		public Registry64(string machine, string userName, SecureString password)
			: base(machine, userName, password)
		{
		}


		/// <summary>
		/// Remote registry constructor.
		/// </summary>
		/// <param name="machine"></param>
		public Registry64(string machine)
			: base(machine)
		{
		}


		/// <summary>
		/// Constructor for registry on local machine.
		/// </summary>
		public Registry64()
			: base()
		{
		}
	}
}
