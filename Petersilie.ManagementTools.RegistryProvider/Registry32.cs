using System.Security;

namespace Petersilie.ManagementTools.RegistryProvider
{
    /// <summary>
    /// 32-bit implementation of the <see cref="RegistryBase"/>
    /// </summary>
    public class Registry32 : RegistryBase
    {
        /// <summary>
        /// <see cref="RegView.x86"/>
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
        /// <param name="machine">NetBIOS name of remote machine</param>
        /// <param name="userName">Admin user name for authentication</param>
        /// <param name="password">Password für authentication</param>
        public Registry32(string machine, string userName, 
            SecureString password)
            : base(machine, userName, password)
        {
        }


        /// <summary>
        /// Remote registry constructor.
        /// </summary>
        /// <param name="machine">NetBIOS name of remote machine.</param>
        public Registry32(string machine)
            : base(machine)
        {
        }


        /// <summary>
        /// Constructor for registry on local machine.
        /// </summary>
        public Registry32()
            : base()
        {
        }
    }
}
