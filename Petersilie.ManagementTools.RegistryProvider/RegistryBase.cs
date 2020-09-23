﻿using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management;
using System;


namespace Petersilie.ManagementTools.RegistryProvider
{
	/// <summary>
	/// Base class for <see cref="Registry32"/>
	/// and <see cref="Registry64"/> class.
	/// </summary>
	public abstract class RegistryBase : IDisposable
	{
		#region Architecture constants

		// Add these to a NamedValueCollection of a InvokeMethodOptions object.
		// Depending on the specified architecture you will access either x64 or x86 RegKeys.
		const string INVOKE_PROVIDERARCHITECTURE = "__ProviderArchitecture";
		const string INVOKE_REQUIREDARCHITECTURE = "__RequiredArchitecture";

		#endregion


		#region METHOD_* constants

		/* =======================================================================
		**							METHOD_* CONSTANTS
		** =======================================================================
		**	The METHOD_* constants are used to invoke methods within the 
		**	StdRegProv class which contains methods that manipulate system
		**	registry keys and values. StdRegProv is preinstalled in the WMI
		**	namespaces root\default and root\cimv2.
		**	The methods are invoked using the ManagementObject.InvokeMethod
		**	and the ManagementObject.GetMethodParameters methods. 
		**
		*/
		/* The EnumKey method enumerates the subkeys for a path. */
		const string METHOD_ENUMKEY = "EnumKey";
		/* The EnumValues method enumerates the values of the fiven subkey.
		** If it has not been changed, the default value of the registry key
		** is always returned. The method returns an empty string ("") if the 
		** data is empty. */
		const string METHOD_ENUMVALUES = "EnumValues";

		#endregion


		#region PROP_* constants

		/* =======================================================================
		**							PROP_* CONSTANTS
		** =======================================================================
		** The PROP_* constants are used to access parameters within
		** methods which where invoke or prepared by the 
		** ManagementObject.InvokeMethod or 
		** ManagementObject.GetMethodParameters method.
		**
		*/
		/* A registry tree, also known as a hive, that contains the sSubKeyName
		** path. The HKEY_LOCAL_MACHINE should be used as default. */
		const string PROP_HDEFKEY = "hDefKey";
		/* The key to be verified. */
		const string PROP_SSUBKEYNAME = "sSubKeyName";		
		/* An array of named value strings. The elements in this array
		** correspond directly to the elements of the Types 
		** parameter (PROP_TYPES). Returns null if only the default 
		** value is available. */
		const string PROP_SNAMES = "sNames";
		/* An array of data values types (integers). You can use these types
		** to determine which of the several Get methods to call. For example,
		** if the data value is REG_SZ, you call the GetStringValue method
		** to retrieve the named value's data value. The elements of this
		** array correspond directly with the elements of the
		** sNames (PROP_SNAMES) parameter. */
		const string PROP_TYPES	= "Types";
		/* Gets the return value of an invoked method.
		** Methods which are used are found within the
		** METHOD_* constants. */
		const string PROP_RETURNVALUE = "ReturnValue";		
		/* A named value whose data value you are retrieving. Specify an empty
		** string to get the default named value. */
		const string PROP_SVALUENAME = "sValueName";
		/* An expanded string data value for the named value. The string is only
		** expanded if the environment variable (for example, %PATH%) 
		** is defined. */
		const string PROP_SVALUE = "sValue";
		/* A DWORD data value for the named value. */
		const string PROP_UVALUE = "uValue";

		#endregion


		#region GET_* constants

		/* =======================================================================
		**							GET_* CONSTANTS
		** =======================================================================
		** The GET_* constants store the names of GET methods which can be invoked
		** to retrieve the data of a StdRegProv class. 
		** These methods are used to cast the native data types to C# compatible
		** data types and are used for that only.
		**
		*/
		/* The GetStringValue method returns the data value for a named value whose
		** data type is REG_SZ. */
		const string GET_SZ = "GetStringValue";
		/* The GetExpandedStringValue method returns the data value for a named
		** value whose data type is REG_EXPAND_SZ. */
		const string GET_EXPAND_SZ = "GetExpandedStringValue";
		/* The GetMultiStringValue method returns the data value for a named value
		** whose data type is REG_MULTI_SZ. */
		const string GET_MULTI_SZ = "GetMultiStringValue";
		/* The GetDWORDValue method returns the data value for a named value whose
		** data type is REG_DWORD. */
		const string GET_DWORD = "GetDWORDValue";
		/* The GetQWORDValue method gets the data value for a named value whose
		** data type is REG_QWORD. */
		const string GET_QWORD = "GetQWORDValue";
		/* The GetBinaryValue method returns the data value for a named value 
		** whose data type is REG_BINARY. */
		const string GET_BINARY	= "GetBinaryValue";

		#endregion


		#region FORMAT_MESSAGE_* constants

		/* =======================================================================
		**						FORMAT_MESSAGE_* CONSTANTS
		** =======================================================================
		** These constants are used for the dwFlags parameter of the FormatMessage
		** function and are used only for that.
		**
		*/
		/* Allocated buffer large enough to hold formatted message,
		** and places pointer to allocated buffer specified by 
		** lpBuffer. If length of formatted message exceeds 128K 
		** bytes the FormatMessage function will fail. */
		const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
		/* Insert sequences in the message definition are ignored and 
        ** passed through to output buffer unchanged. 
        ** Use this to format the message later. */
		const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
		/* Function should search system message-table resources for 
        ** requested message. Flag CANNOT be used with 
        ** FORMAT_MESSAGE_FROM_STRING. */
		const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
		/* Arguments parameter is not a va_list struct, but pointer to
        ** to an array of values that represents the arguments. 
        ** Flag CANNOT be used with 64-bit integer values. */
		const uint FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000;
		/* lpSource param is module handle containing the 
        ** message-table resources to search. If lpSource handle is 
        ** NULL, the current process's app image file will be searched. */
		const uint FORMAT_MESSAGE_FROM_HMODULE = 0x00000800;
		/* lpSource param is pointer to null-terminated string that
        ** contains a message definition. Flag CANNOT be used with
        ** FORMAT_MESSAGE_FROM_HMODULE or FORMAT_MESSAGE_FROM_SYSTEM */
		const uint FORMAT_MESSAGE_FROM_STRING = 0x00000400;

		#endregion


		#region WinApi implementations

		/* =======================================================================
		**					FormatMessage function (winbase.h)
		** =======================================================================
		**	Formats a message string. The function requires a message definition
		**	as input. The message definition ca come from a buffer passed into
		**	the function. It can come from a message table resource in an
		**	already-loaded module. Or the caller can ask the function to search
		**	the system's message table resource(s) for the message definition.
		* 
		** ##################
		** #   PARAMETERS   #
		** ##################
		**
		** dwFlags: 
		** ========
		**	Formatting options, and how to interpret lpSource param.
		**	The low-order byte of dwFlags specifies how the function
		**	handles line breaks in the output buffer. The low-order
		**	byte can also specify the max width of a formatted output line.
		**	Use the FORMAT_MESSAGE_* constants for this parameter.
		**
		** lpSource: 
		** =========
		**	The location of the message definition. The type of this parameter
		**	depends upon the settings in the dwFlags parameter.
		**	Can be one of the following: 
		**		- FORMAT_MESSAGE_FROM_HMODULE
		**		- FORMAT_MESSAGE_FROM_STRING
		**	If neither of these flags is set in dwFlags, then lpSource is ignored.
		**
		** dwMessageId: 
		** ============
		**	The message identifier for the requested message.
		**	This parameter is ignored if dwFlags includes 
		**	FORMAT_MESSAGE_FROM_STRING.
		** 
		** dwLanguageId:
		** =============
		**	The language identifier for the requested message.
		**	This parameter is ingored if dwFlags includes 
		**	FORMAT_MESSAGE_FROM_STRING.
		**	If you pass a specific LANGID in this parameter, FormatMessage will
		**	return a message for that LANGID only. If the function cannot find
		**	a message for that LANGID, it sets Last-Error to 
		**	ERROR_RESOURCE_LANG_NOT_FOUND. If you pass in zero, FormatMessage
		**	looks for a message for LANGIDs in the following order:
		**		1) Language neutral
		**		2) Thread LANGID.
		**		3) User default LANGID.
		**		4) System default LANGID.
		**		5) US English.
		**
		** lpBuffer:
		** =========
		**	A pointer to a buffer that receives the null-terminated string that
		**	specifies the formatted message. If dwFlags includes 
		**	FORMAT_MESSAGE_ALLOCATE_BUFFER, the function allocates a buffer using
		**	the LocalAlloc function, and places the pointer to the buffer at the
		**	address specified in lpBuffer.
		**	This buffer cannot be larger than 64K bytes.
		**
		** nSize:
		** ======
		**	If the FORMAT_MESSAGE_ALLOCATE_BUFFER flag is not set, this parameter
		**	specifies the size of the output buffer, in TCHARs. 
		**	If FORMAT_MESSAGE_ALLOCATE_BUFFER is set, this parameter specifies
		**	the minimum number of TCHARs to allocate for an output buffer.
		**	The output buffer cannot be larger than 64K bytes.
		** 
		** pArguments:
		** ===========
		**	An array of values that are used as insert values in the formatted
		**	message. A %1 in the format string indicates the first value in 
		**	the Arguments array; a %2 indicates the second argument; and so on.
		**	If you do not have a pointer of type va_list*, then specify the
		**	FORMAT_MESSAGE_ARGUMENT_ARRAY flags and pass a pointer to an array
		**	of DWORD_PTR values; those values are input to the message formatted
		**	as the insert values. Each insert must have a corresponding element
		**	in the array.
		** 
		** ####################
		** #   RETURN VALUE   #
		** ####################
		**	If the function succeeds, the return value is the number of TSCHARs
		**	stored in the output buffer, excluding the terminated null character.
		**	If the function fails, the return value is zero. To get extended error
		**	information, call GetLastError. 
		*/
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern uint FormatMessage(	uint dwFlags, 
											IntPtr lpSource,       
											uint dwMessageId, 
											uint dwLanguageId,	
											ref IntPtr lpBuffer, 
											uint nSize,			
											IntPtr pArguments);


		/* =======================================================================
		**					LocalFree function (winbase.h)
		** =======================================================================
		**	Frees the specified local memory object and invalidates its handle.
		**	
		** ##################
		** #   PARAMETERS   #
		** ##################
		** 
		** hMem:
		** =====		
		**	A handle to the local memory object. This handle is returned by either
		**	the LocalAlloc or LocalReAlloc function. It is not safe to free 
		**	memory allocated with GlobalAlloc.
		** 
		** ####################
		** #   RETURN VALUE   #
		** ####################
		**	If the function succeeds, the return value is NULL.
		**	If the function fails, the return value is equal to a handle to the
		**	local memory object. To get extended error information, call
		**	GetLastError.
		*/
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr LocalFree(IntPtr hMem);

		#endregion


		#region Public properties

		/// <summary>
		/// Name of the machine whose registry 
		/// will be accessed.
		/// </summary>
		public string Machine { get; }

		#endregion


		#region Private properties

		// Management class which is used.
		private readonly ManagementClass _mgmt;

		#endregion


		#region Error message creation

		/* Uses the WinAPI function FormatMessage to translate a System Error Code
		** to a Error Message defined by the WinAPI.
		** Use this function to get an error message for the ReturnValue property
		** when accessing registry keys. */
		private string GetErrorMessage(int nLastError)
		{
			// Buffer in which we store the message
			IntPtr lpMsgBuf = IntPtr.Zero;
			// Get formatted message.
			uint dwChars = FormatMessage(
				FORMAT_MESSAGE_ALLOCATE_BUFFER | 
				FORMAT_MESSAGE_FROM_SYSTEM | 
				FORMAT_MESSAGE_IGNORE_INSERTS,
				IntPtr.Zero, (uint)nLastError,
				0, ref lpMsgBuf,
				0, IntPtr.Zero);

			// FormatMessage failed... handle Win32 error
			if (dwChars == 0) {
				int error = Marshal.GetLastWin32Error();
				return GetErrorMessage(error);
			}

			// Convert buffer to string
			string retVal = Marshal.PtrToStringAnsi(lpMsgBuf);
			// Free buffer
			lpMsgBuf = LocalFree(lpMsgBuf);

			return retVal;
		}

		#endregion


		#region Registry key retrieval

		/// <summary>
		/// Gets all registry keys within the specified parent registry key
		/// </summary>
		/// <param name="hive">The registry tree, also known as hive, 
		/// that contains the parent key path</param>
		/// <param name="keyPath">Registry key path</param>
		/// <param name="fullPath">TRUE if the function should append the sub 
		/// key name to the parent key</param>
		/// <returns></returns>
		public string[] GetSubKeys(RegHive hive, string keyPath, bool fullPath)
		{
			// Define input parameters for EnumKey method.
			var inParams = _mgmt.GetMethodParameters(METHOD_ENUMKEY);
			inParams[PROP_HDEFKEY]		= hive;
			inParams[PROP_SSUBKEYNAME]	= keyPath;

			// Invoke the EnumKey method with the input paramters
			var outParams = _mgmt.InvokeMethod(
				METHOD_ENUMKEY, // EnumKey method.
				inParams,		// EnumKey parameters.
				Architecture);  // x64 or x86 specific architecture.

			// Convert native array to C# string array.				
			string[] keys = ObjectConverter.ToStringArray(outParams, PROP_SNAMES);

			// Append parent if neccessary.
			if (fullPath) {
				for (int i = 0; i < keys.Length; i++) {
					keys[i] = keyPath + "\\" + keys[i];
				}
			}
			return keys;
		}


		/// <summary>
		/// Gets all registry keys within the specified parent registry key
		/// </summary>
		/// <param name="hive">The registry tree, also known as hive, 
		/// that contains the parent key path</param>
		/// <param name="keyPath">Registry key path</param>
		/// <returns></returns>
		public string[] GetSubKeys(RegHive hive, string keyPath)
		{
			// Define input parameters for EnumKey method.
			var inParams = _mgmt.GetMethodParameters(METHOD_ENUMKEY);
			inParams[PROP_HDEFKEY]		= hive;
			inParams[PROP_SSUBKEYNAME]	= keyPath;

			// Invoke the EnumKey method with the input parameters.
			var outParams = _mgmt.InvokeMethod(
				METHOD_ENUMKEY,	// EnumKey method.
				inParams,		// EnumKey parameters.
				Architecture);	// x64 or x86 specific architecture.

			// Convert native array to C# string array.
			string[] keys = ObjectConverter.ToStringArray(outParams, PROP_SNAMES);

			return keys;
		}

		#endregion


		#region Type based conversions

		/// <summary>
		/// Stores all registry key properties in a 
		/// dictionary with their respective data type
		/// </summary>
		/// <param name="hive">The registry tree, also known as hive, 
		/// that contains the parent key path</param>
		/// <param name="keyPath">Registry key path</param>
		/// <returns></returns>
		/// <exception cref="Win32Exception"></exception>
		public int GetRegTypes(RegHive hive, string keyPath, 
			out Dictionary<string, RegType> typeMap)
		{
			// Define input parameters for EnumValues method.
			var inParams = _mgmt.GetMethodParameters(METHOD_ENUMVALUES);
			inParams[PROP_HDEFKEY]		= hive;
			inParams[PROP_SSUBKEYNAME]	= keyPath;

			// Invoke the EnumValues method with the input parameters.
			var outParams = _mgmt.InvokeMethod(
				METHOD_ENUMVALUES,	// EnumValues method.
				inParams,			// EnumValues parameters.
				Architecture);		// x64 or x86 specific architecture.
			
			int retVal; // 0 on success.
			try {
				// Get return value of EnumValues method.
				string sResult = outParams[PROP_RETURNVALUE]?.ToString();
				// Parse return value to int.
				if (int.TryParse((sResult ?? "-1"), out retVal))
				{
					// Check if type conversion was successfull.
					retVal = retVal == -1 
						? Marshal.GetLastWin32Error() // Value was null.
						: retVal; // Value was success or error code.
				}
			}
			catch {				
				typeMap = null;
				int lr = Marshal.GetLastWin32Error();
				return 0 != lr ? lr : -1;
			}

			var dict = new Dictionary<string, RegType>();
			switch (retVal)
			{
				// Values where received successfully 
				case 0:
					{
						// Convert native array to C# string array.
						string[] names = ObjectConverter.ToStringArray(
							outParams,	
							PROP_SNAMES); // Array of property names.

						// Convert native array to C# 16-bit integer array.
						Int16[] types = ObjectConverter.ToInt16Array(
							outParams, 
							PROP_TYPES); // Array of property types.

						// Map property name and data type.
						for (int i = 0; i < names.Length; i++) {
							dict.Add(names[i], (RegType)types[i]);
						}
						typeMap = dict;
						break;
					}
				// Something along the line failed
				default:
					{
						typeMap = null;
						break;
					}
			}
			return retVal;
		}


		/* Gets the registry data type for the named property 
		** which must exist within the properties of the reg key. */
		private int GetRegType(RegHive hive, string keyPath, string property, out RegType type)
		{
			// Define input parameters for EnumValues method.
			var inParams = _mgmt.GetMethodParameters(METHOD_ENUMVALUES);
			inParams[PROP_HDEFKEY]		= hive;
			inParams[PROP_SSUBKEYNAME]	= keyPath;

			// Invoke the EnumValues method with the input parameters.
			var outParams = _mgmt.InvokeMethod(
				METHOD_ENUMVALUES,	// EnumValues method.
				inParams,			// EnumValues parameters.
				Architecture);		// x64 or x86 specific architecture.

			int retVal; // 0 on success.
			try {
				// Get return value of EnumValues method.
				string sResult = outParams[PROP_RETURNVALUE]?.ToString();
				// Parse return value to int.
				if (int.TryParse((sResult ?? "-1"), out retVal))
				{
					// Check if type conversion was successfull.
					retVal = retVal == -1 
						? Marshal.GetLastWin32Error() // Value was null.
						: retVal; // Value was success or error code.
				}				
			}
			catch {
				type = RegType.UNKOWN;
				int lr = Marshal.GetLastWin32Error();
				return 0 != lr ? lr : -1;
			}

			type = RegType.UNKOWN;
			switch (retVal)
			{
				// Values where received successfully 
				case 0:
					{
						// Convert native array to C# string array.
						string[] names = ObjectConverter.ToStringArray(
							outParams,  
							PROP_SNAMES); // Array of property names.
						
						// Convert native array to C# 16-bit integer array.
						Int16[] types = ObjectConverter.ToInt16Array(
							outParams, 
							PROP_TYPES); // Array of property types.

						// Map property name and data type.
						for (int i = 0; i < names.Length; i++) {
							if (names[i] == property) {
								type = (RegType)types[i];
								return retVal;
							}
						}					
						break;
					}
			}
			return retVal;
		}


		// Converts the stored data of the baseObj to a usable C# datatype
		private object TranslateValue(ManagementBaseObject baseObj,
			string method, string property)
		{
			switch (method)
			{
				case GET_BINARY:    // byte[]
					return ObjectConverter.ToByteArray(baseObj, property);
				case GET_DWORD:     // UInt32
					return ObjectConverter.ToUInt32(baseObj, property);
				case GET_EXPAND_SZ: // string
					return ObjectConverter.ToString(baseObj, property);
				case GET_MULTI_SZ:  // string[]
					return ObjectConverter.ToStringArray(baseObj, property);
				case GET_QWORD:     // UInt64
					return ObjectConverter.ToUInt64(baseObj, property);
				case GET_SZ:        // string
					return ObjectConverter.ToString(baseObj, property);
				default:
					return null;
			}
		}

		#endregion


		#region Method mapping

		/* Set the Get method name and property name for
		** the specific registry data type. */
		private bool GetMappedMethodAndProperty(RegType rt, 
			out string method, out string property)
		{
			bool retVal = false;

			switch (rt)
			{
				case RegType.REG_BINARY:
					{
						method		= GET_BINARY;
						property	= PROP_UVALUE;
						retVal		= true;
						break;
					}
				case RegType.REG_DWORD:
					{
						method		= GET_DWORD;
						property	= PROP_UVALUE;
						retVal		= true;
						break;
					}
				case RegType.REG_EXPAND_SZ:
					{
						method		= GET_EXPAND_SZ;
						property	= PROP_SVALUE;
						retVal		= true;
						break;
					}
				case RegType.REG_MULTI_SZ:
					{
						method		= GET_MULTI_SZ;
						property	= PROP_SVALUE;
						retVal		= true;
						break;
					}
				case RegType.REG_QWORD:
					{
						method		= GET_QWORD;
						property	= PROP_UVALUE;
						retVal		= true;
						break;
					}
				case RegType.REG_SZ:
					{
						method		= GET_SZ;
						property	= PROP_SVALUE;
						retVal		= true;
						break;
					}
				default:
					{
						method		= string.Empty;
						property	= string.Empty;
						retVal		= false;
						break;
					}
			}
			return retVal;
		}

		#endregion


		#region Value retrieval

		/// <summary>
		/// Gets all values of the specified registry key
		/// </summary>
		/// <param name="hive">The registry tree, also known as hive, 
		/// that contains the parent key path</param>
		/// <param name="keyPath">Registry key path</param>
		/// <param name="silentlyContinue">TRUE if the method should 
		/// not be interupted by Exceptions</param>
		/// <returns></returns>
		/// <exception cref="Win32Exception"></exception>
		public object[] GetValues(RegHive hive, string keyPath, 
			bool silentlyContinue = false)
		{
			Dictionary<string, RegType> typeMap;
			// Get data type of all properties
			int retVal = GetRegTypes(hive, keyPath, out typeMap);		
			if ( 0 != retVal ) {
				string errMsg = GetErrorMessage(retVal);
				throw new Win32Exception(errMsg);
			}

			int i = 0;
			object[] results = new object[typeMap.Keys.Count];

			// Load values into array.
			foreach (var key in typeMap.Keys)
			{
				try {
					results[i] = GetValue(hive, typeMap[key], keyPath, key);
				}
				catch (Win32Exception) {
					if ( !silentlyContinue ) {
						throw;
					}
				}
				finally {
					i++;
				}
			}
			return results;
		}


		/// <summary>
		/// Gets all values of the specified registry key
		/// </summary>
		/// <param name="hive">The registry tree, also known as hive, 
		/// that contains the parent key path</param>
		/// <param name="keyPath">Registry key path</param>
		/// <returns></returns>
		public IEnumerable<object> GetValues(RegHive hive, string keyPath)
		{
			Dictionary<string, RegType> typeMap;
			// Get data type of all properties.
			var retVal = GetRegTypes(hive, keyPath, out typeMap);
			if ( 0 != retVal ) {
				string errMsg = GetErrorMessage(retVal);
				throw new Win32Exception(errMsg);
			}

			foreach (var key in typeMap.Keys) {
				// Convert reg property value to C# data type.
				yield return GetValue(hive, typeMap[key], keyPath, key);
			}
		}


		/* Get the value of the named property. */
		private object GetValue(RegHive hive, RegType regType, 
			string keyPath, string property)
		{
			ManagementBaseObject inParams	= null;
			ManagementBaseObject outParams	= null;

			string method;
			string valProp;
			if ( !GetMappedMethodAndProperty(regType, out method, out valProp) ) {
				return null;
			}

			// Define input parameters for the Get method.
			inParams = _mgmt.GetMethodParameters(method);
			inParams[PROP_HDEFKEY]		= hive;			// Registry hive of key.
			inParams[PROP_SSUBKEYNAME]	= keyPath;		// Registry key.
			inParams[PROP_SVALUENAME]	= property;		// Value property.

			// Invoke the Get method with the input parameters.
			outParams = _mgmt.InvokeMethod(method, inParams, Architecture);
			var retVal = ObjectConverter.ToInt32(outParams, PROP_RETURNVALUE);
			// Check for errors.
			if (retVal != 0) {
				inParams.Dispose();
				outParams.Dispose();
				// Get message and throw.
				string errMsg = GetErrorMessage(retVal);
				throw new Win32Exception(retVal, errMsg);
			}

			object value = TranslateValue(outParams, method, valProp);

			inParams.Dispose();
			outParams.Dispose();

			return value;
		}


		/// <summary>
		/// Gets the actual value of the specified registry key value/property
		/// </summary>
		/// <param name="hive">The registry tree, also known as hive, 
		/// that contains the parent key path</param>
		/// <param name="keyPath">Registry key path</param>
		/// <param name="value">Value name / Property name</param>
		/// <returns></returns>
		/// <exception cref="Win32Exception"></exception>
		public object GetValue(RegHive hive, string keyPath, string property)
		{
			RegType type;
			// Get the data type of the registry value
			var retVal = GetRegType(hive, keyPath, property, out type);
			if ( 0 != retVal ) {
				string errMsg = GetErrorMessage(retVal);
				throw new Win32Exception(errMsg);
			}

			if (RegType.UNKOWN == type) {
				return null;
			}

			ManagementBaseObject inParams	= null;
			ManagementBaseObject outParams	= null;

			string method;
			string valProp;
			if ( !GetMappedMethodAndProperty(type, out method, out valProp) ) {
				return null;
			}

			// Define input parameters for the Get method.
			inParams = _mgmt.GetMethodParameters(method);
			inParams[PROP_HDEFKEY]		= hive;		// Registry hive of key.
			inParams[PROP_SSUBKEYNAME]	= keyPath;	// Registry key.
			inParams[PROP_SVALUENAME]	= property;	// Value property.

			// Invoke the Get method with the input parameters.
			outParams = _mgmt.InvokeMethod(method, inParams, Architecture);
			retVal = ObjectConverter.ToInt32(outParams, PROP_RETURNVALUE);
			// Check for errors.
			if (retVal != 0) {
				inParams.Dispose();
				outParams.Dispose();
				// Get message and throw.
				string errMsg = GetErrorMessage(retVal);
				throw new Win32Exception(retVal, errMsg);
			}

			object value = TranslateValue(outParams, method, valProp);

			inParams.Dispose();
			outParams.Dispose();

			return value;
		}

		#endregion


		#region Architecture initialization

		/* The InvokeMethodOptions are essential when trying to access
		** the registry keys and values of a specific architecture.
		** Especially when searching for applications within the 
		** ...\CurrentVersion\Uninstall key you need to differentiate
		** between the 64-bit Registry and the 32-bit Registry. */
		protected InvokeMethodOptions Architecture;
		
		// ViewFlag determines if the hive view is x64 or x86
		protected virtual RegView ViewFlag { get; }
		
		// Initialize the architecture.
		protected virtual void InitializeArchitecture()
		{
			Architecture = new InvokeMethodOptions();
			Architecture.Context.Add(INVOKE_PROVIDERARCHITECTURE, (int)ViewFlag);
			Architecture.Context.Add(INVOKE_REQUIREDARCHITECTURE, true);
		}

		#endregion


		#region Constructors

		/// <summary>
		/// Instance for the local registry
		/// </summary>
		public RegistryBase()
		{
			Machine = Environment.MachineName;
			_mgmt = ManagementProvider.StdRegMgmtProvider();
			InitializeArchitecture();
		}

		/// <summary>
		/// Instance for a remote registry
		/// </summary>
		/// <param name="machine">Name of the remote computer</param>
		public RegistryBase(string machine)
		{
			Machine = machine;
			_mgmt = ManagementProvider.StdRegMgmtProvider(machine);
			InitializeArchitecture();
		}

		/// <summary>
		/// Instance for a remote registry with credentials
		/// </summary>
		/// <param name="machine">Name of the remote computer</param>
		/// <param name="userName">Name of elevated account</param>
		/// <param name="password">Password of elevated account</param>
		public RegistryBase(string machine, string userName, System.Security.SecureString password)
		{
			Machine = machine;
			_mgmt = ManagementProvider.StdRegMgmtProvider(machine, userName, password);
			InitializeArchitecture();
		}

		#endregion


		#region IDisposable implementation

		~RegistryBase() { Dispose(false); }
		public void Dispose() { Dispose(true); }
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				GC.SuppressFinalize(this);
			}
			_mgmt.Dispose();
		}

		#endregion
	}
}