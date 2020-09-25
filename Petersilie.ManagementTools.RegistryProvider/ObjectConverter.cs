using System;
using System.Linq;
using System.Management;

namespace Petersilie.ManagementTools.RegistryProvider
{
    /* Converts any ManagementBaseObject property to the
    ** desired C# data type. */
    internal static class ObjectConverter
    {
        /* Check if ManagementObject contains that property. */
        public static bool PropertyExists(ManagementBaseObject baseObj, 
            string property)
        {
            // Total amount of properties within ManagementBaseObject.
            int propCount = baseObj.Properties.Count;
            // Total amount of system properties within ManagementBaseObject.
            int syspCount = baseObj.SystemProperties.Count;
            // Array which will hold the properties.
            var propData = new PropertyData[propCount + syspCount];

            // Copy properties to array.
            baseObj.Properties.CopyTo(propData, 0);
            // Copy system properties to array.
            baseObj.SystemProperties.CopyTo(propData, propCount);

            for (int i = 0; i < propData.Length; i++) {
                // Look for desired property.
                if (propData[i].Name == property)
                    return true;
            }
            return false;
        }

        /* Converts the named property of the ManagementBaseObject to a string. */
        public static string ToString(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return string.Empty;
            try { if (baseObj[property] == null) return string.Empty; }
            catch { return string.Empty; }
            return baseObj[property]?.ToString() ?? string.Empty;
        }


        public static UInt64[] ToUInt64Array(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return new UInt64[0];
            try { if (baseObj[property] == null) return new UInt64[0]; }
            catch { return new UInt64[0]; }
            return ((System.Collections.IEnumerable)baseObj[property])
                .Cast<object>()
                .Select(o => Convert.ToUInt64(o?.ToString()))
                .ToArray();
        }

        public static UInt32[] ToUInt32Array(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return new UInt32[0];
            try { if (baseObj[property] == null) return new UInt32[0]; }
            catch { return new UInt32[0]; }
            return ((System.Collections.IEnumerable)baseObj[property])
                .Cast<object>()
                .Select(o => Convert.ToUInt32(o?.ToString()))
                .ToArray();
        }

        public static UInt16[] ToUInt16Array(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return new UInt16[0];
            try { if (baseObj[property] == null) return new UInt16[0]; }
            catch { return new UInt16[0]; }
            return ((System.Collections.IEnumerable)baseObj[property])
                .Cast<object>()
                .Select(o => Convert.ToUInt16(o?.ToString()))
                .ToArray();
        }

        public static Int16[] ToInt16Array(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return new Int16[0];
            try { if (baseObj[property] == null) return new Int16[0]; }
            catch { return new Int16[0]; }
            return ((System.Collections.IEnumerable)baseObj[property])
                .Cast<object>()
                .Select(o => Convert.ToInt16(o?.ToString()))
                .ToArray();
        }

        public static byte[] ToByteArray(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return new byte[0];
            try { if (baseObj[property] == null) return new byte[0]; }
            catch { return new byte[0]; }
            return ((System.Collections.IEnumerable)baseObj[property])
                .Cast<object>()
                .Select(o => Convert.ToByte(o?.ToString()))
                .ToArray();
        }

        public static string[] ToStringArray(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return new string[0];
            try { if (baseObj[property] == null) return new string[0]; }
            catch { return new string[0]; }
            return ((System.Collections.IEnumerable)baseObj[property])
                .Cast<object>()
                .Select(o => o.ToString())
                .ToArray();
        }

        public static Int16 ToInt16(ManagementBaseObject baseObj,
            string property)
        {
            if (!PropertyExists(baseObj, property)) return 0;
            try { if (baseObj[property] == null) return 0; }
            catch { return 0; }
            Int16 i;
            Int16.TryParse(baseObj[property]?.ToString(), out i);
            return i;
        }

        public static bool ToBool(ManagementBaseObject baseObj,
            string property)
        {
            if (!PropertyExists(baseObj, property)) return false;
            if (baseObj[property] == null) return false;
            bool b;
            bool.TryParse(baseObj[property]?.ToString(), out b);
            return b;
        }

        public static byte ToByte(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return 0;
            try { if (baseObj[property] == null) return 0; }
            catch { return 0; }
            byte b;
            byte.TryParse(baseObj[property]?.ToString(), out b);
            return b;
        }

        public static UInt16 ToUInt16(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return 0;
            try { if (baseObj[property] == null) return 0; }
            catch { return 0; }
            UInt16 u;
            UInt16.TryParse(baseObj[property]?.ToString(), out u);
            return u;
        }

        public static UInt32 ToUInt32(ManagementBaseObject baseObj,
            string property)
        {
            if (!PropertyExists(baseObj, property)) return 0;
            try { if (baseObj[property] == null) return 0; }
            catch { return 0; }
            UInt32 u;
            UInt32.TryParse(baseObj[property]?.ToString(), out u);
            return u;
        }

        public static UInt64 ToUInt64(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return 0;
            try { if (baseObj[property] == null) return 0; }
            catch { return 0; }
            UInt64 u;
            UInt64.TryParse(baseObj[property]?.ToString(), out u);
            return u;
        }

        public static DateTime? ToDateTime(ManagementBaseObject baseObj,
            string property)
        {
            if (!PropertyExists(baseObj, property)) return null;
            try { if (baseObj[property] == null) return null; }
            catch { return null; }
            DateTime dt = ManagementDateTimeConverter.ToDateTime(baseObj[property].ToString());
            return dt;
        }

        public static float ToFloat(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return 0;
            try { if (baseObj[property] == null) return 0; }
            catch { return 0; }
            float d;
            float.TryParse(baseObj[property]?.ToString(), out d);
            return d;
        }

        public static double ToDouble(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return 0;
            try { if (baseObj[property] == null) return 0; }
            catch { return 0; }
            double d;
            double.TryParse(baseObj[property]?.ToString(), out d);
            return d;
        }

        public static Int32 ToInt32(ManagementBaseObject baseObj, 
            string property)
        {
            if (!PropertyExists(baseObj, property)) return 0;
            try { if (baseObj[property] == null) return 0; }
            catch { return 0; }
            Int32 i;
            Int32.TryParse(baseObj[property]?.ToString(), out i);
            return i;
        }
    }
}
