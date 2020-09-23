using System;

namespace Petersilie.ManagementTools.RegistryProvider
{
	/// <summary>
	/// Registry hives which can be accessed.
	/// </summary>
	public enum RegHive : uint {
        HKEY_CLASSES_ROOT   = 0x80000000, // 2147483648
        HKEY_CURRENT_USER   = 0x80000001, // 2147483649
        HKEY_LOCAL_MACHINE  = 0x80000002, // 2147483650
        HKEY_USERS          = 0x80000003, // 2147483651
        HKEY_CURRENT_CONFIG = 0x80000005, // 2147483653
    }


	/// <summary>
	/// Registry property data types
	/// </summary>
    public enum RegType {
        REG_SZ          = 1,
        REG_EXPAND_SZ   = 2,
        REG_BINARY      = 3,
        REG_DWORD       = 4,
        REG_MULTI_SZ    = 7,
        REG_QWORD       = 11,
        UNKOWN
    }


	/// <summary>
	/// Registry architectures.
	/// </summary>
	[Flags] public enum RegView {
		x86 = 1 << 5,
		x64 = 1 << 6		
	}
}
