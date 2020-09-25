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
    public enum RegView {
        x86 = 1 << 5,
        x64 = 1 << 6        
    }


    /// <summary>
    /// Used for checking permissions on registry keys.
    /// </summary>
    [Flags] public enum RegAccessFlags : UInt32 {
        /// <summary>
        /// Required to query the values of a registry key.
        /// </summary>
        QueryValue = 0x00001,
        /// <summary>
        /// Required to create, delete, or set a registry value.
        /// </summary>
        SetValue = 0x00002,
        /// <summary>
        /// DEFAULT value, allows querying, creating, deleting, 
        /// or setting a registry value.
        /// </summary>
        QueryCreateDelete = QueryValue | SetValue,
        /// <summary>
        /// Required to create a subkey of a registry key.
        /// </summary>
        CreateSubKey = 0x00004,
        /// <summary>
        /// Required to enumerate the subkeys of a registry key.
        /// </summary>
        EnumerateSubKeys = 0x00008,
        /// <summary>
        /// Required to request change notifications for a registry
        /// key or for subkeys of a registry key.
        /// </summary>
        Notify = 0x00010,
        /// <summary>
        /// Required to create a registry key.
        /// </summary>
        Create = 0x00020,
        /// <summary>
        /// Required to delete a registry key.
        /// </summary>
        Delete = 0x10000,
        /// <summary>
        /// Combines the STANDARD_RIGHTS_READ, KEY_QUERY_VALUE,
        /// KEY_ENUMERATE_SUB_KEYS, and KEY_NOTIFY values.
        /// </summary>
        ReadControl = 0x20000,
        /// <summary>
        /// Rquired to modify the DACL in the object's security descriptor.
        /// </summary>
        WriteDAC = 0x40000,
        /// <summary>
        /// Required to change the owner in the object's security descriptor.
        /// </summary>
        WriteOwner = 0x80000
    }
}
