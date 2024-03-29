// Copyright (c) János Janka. All rights reserved.

using System.Runtime.InteropServices;
using System.Security;

namespace System
{
    internal static class GuidUtility
    {
        [SecurityCritical, SuppressUnmanagedCodeSecurity]
        [DllImport("rpcrt4.dll", SetLastError = true)]
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
        internal static extern int UuidCreateSequential(out Guid guid);

        /// <summary>
        /// Creates a new unique string identifier using the bytes of a GUID.
        /// </summary>
        /// <returns>An unique string identifier.</returns>
        public static string NewSequentialId()
        {
            return NewSequentialGuid().ToString("N");
        }

        /// <summary>
        /// Create a new sequential GUID.
        /// </summary>
        /// <returns>Returns with a GUID.</returns>
        public static Guid NewSequentialGuid()
        {
            Guid guid;
            int hr = UuidCreateSequential(out guid);
            if (hr == 0)
            {
                byte[] source = guid.ToByteArray();
                byte[] dest = guid.ToByteArray();
                dest[0] = source[3];
                dest[1] = source[2];
                dest[2] = source[1];
                dest[3] = source[0];
                dest[4] = source[5];
                dest[5] = source[4];
                dest[6] = source[7];
                dest[7] = source[6];
                return new Guid(dest);
            }
            return Guid.NewGuid();
        }
    }
}