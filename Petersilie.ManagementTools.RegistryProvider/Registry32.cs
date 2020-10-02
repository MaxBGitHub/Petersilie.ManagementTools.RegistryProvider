using System.Security;

namespace Petersilie.ManagementTools.RegistryProvider
{
    /// <summary>
    /// Provides the 32-bit view of a local or remote registry.
    /// </summary>
    public class Registry32 : RegistryBase
    {
        /// <summary>
        /// <see cref="RegView.x86"/>
        /// </summary>
        internal protected override RegView ViewFlag
        {
            get {
                return RegView.x86;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Registry32"/>
        /// class which accesses the registry of the specified remote
        /// computer with the specified user credentials. 
        /// Some functions will fail if the specified user does not have
        /// administrative priviliges.
        /// </summary>
        /// <param name="machine">The NetBIOS name of the 
        /// remote computer</param>
        /// <param name="userName">The user name of the user which 
        /// will be impersionated</param>
        /// <param name="password">The password of the user which 
        /// will be impersionated</param>
        public Registry32(string machine, string userName, 
            SecureString password)
            : base(machine, userName, password)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Registry32"/>
        /// class which accesses the registry of the specified remote
        /// computer. Some functions will fail if application is not run
        /// with administrative priviliges.
        /// </summary>
        /// <param name="machine">The NetBIOS name of 
        /// the remote computer</param>
        public Registry32(string machine)
            : base(machine)
        {
        }


        /// <summary>
        /// Constructor for the registry of the local computer
        /// </summary>
        public Registry32()
            : base()
        {
        }
    }
}
