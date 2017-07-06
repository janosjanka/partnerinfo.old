// Copyright (c) János Janka. All rights reserved.

using System;
using System.Globalization;
using System.Text;
using Partnerinfo.Logging;

namespace Partnerinfo.Project.Actions
{
    [Obsolete("Use the ActionLinkService class for generating new action links.")]
    public static class ActionEventConverter
    {
        private const string InternalSalt = "$6ae59cce4dd4$";
        private static char[] s_separators = new[] { '-' };

        static ActionEventConverter()
        {
#if DEBUG
            RouteLink = "http://" + string.Join("/", Properties.Settings.Default.AppHost, "a");
#else
            RouteLink = "http://www." + string.Join("/", Properties.Settings.Default.AppHost, "a");
#endif
        }

        public static string RouteLink { get; set; }

        /// <summary>
        /// Gets the action URL.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static string ActionLink(ActionEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            return string.Join("/", RouteLink, EncodeActionEvent(args));
        }

        /// <summary>
        /// Encodes action URL arguments to a string representation: {crc}-{type}-{id}-{contact?}/{salt?}
        /// </summary>
        /// <param name="args">The action URL arguments to encode.</param>
        /// <returns>
        /// A string representation of the action URL arguments.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static string EncodeActionEvent(ActionEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            var builder = new StringBuilder(16);
            builder.Append(args.TargetType.ToString("x"));
            builder.Append('-');
            builder.Append(args.TargetId.ToString("x"));

            if (args.ContactId != null)
            {
                builder.Append('-');
                builder.Append(args.ContactId.Value.ToString("x"));
            }

            string crc = CompCrc32(builder.ToString(), args.Salt).ToString("x");
            builder.Insert(0, '-');
            builder.Insert(0, crc);

            if (args.Salt != null)
            {
                builder.Append('/');
                builder.Append(args.Salt);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Decodes action URL arguments to an typed object.
        /// </summary>
        /// <param name="args">The encoded string to decode.</param>
        /// <param name="salt">The salt to add.</param>
        /// <returns>
        /// An object representing the encoded parameters.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when a null reference is passed to the method.</exception>
        public static ActionEventArgs DecodeActionEvent(string args, string salt = null)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            int separator = args.IndexOf('-');

            if ((separator >= 1) && (separator <= args.Length - 2))
            {
                uint checksum;
                string crc = args.Substring(0, separator);
                string val = args.Substring(separator + 1);

                if ((val.Length > 0)
                    && TryGetHexValue(crc, out checksum)
                    && CompCrc32(val, salt) == checksum)
                {
                    int targetType;
                    int targetId;
                    int contactId;
                    string[] slots = val.Split(s_separators, 3);

                    if (TryGetHexValue(slots, 0, out targetType) &&
                        TryGetHexValue(slots, 1, out targetId))
                    {
                        return new ActionEventArgs
                        {
                            TargetType = MapEventTarget(targetType),
                            TargetId = targetId,
                            ContactId = TryGetHexValue(slots, 2, out contactId) ? contactId : default(int?),
                            Salt = salt
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Converts a number to a target value saving backward compatibility.
        /// </summary>
        /// <param name="target">The number to convert.</param>
        /// <returns>
        /// A value of enumeration values that specifies an event target.
        /// </returns>
        private static ObjectType MapEventTarget(int target)
        {
            if (target == 9)
            {
                return ObjectType.Action;
            }
            return (ObjectType)target;
        }

        /// <summary>
        /// Computes a CRC value for the specified URL arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="salt">The salt value.</param>
        /// <returns>A CRC checksum value.</returns>
        private static uint CompCrc32(string args, string salt = null)
        {
            if (salt == null)
            {
                return Crc32.Compute(args + InternalSalt);
            }
            return Crc32.Compute(args + salt + InternalSalt);
        }

        /// <summary>
        /// Tries to convert the string representation of a number to a hexadecimal value.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="str">A string that represents the number to convert.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if it was converted successfully; otherwise <c>false</c>.
        /// </returns>
        private static bool TryGetHexValue(string str, out uint value)
        {
            if (str == null)
            {
                value = 0;
                return false;
            }
            return uint.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Tries to convert the string representation of a number to a hexadecimal value.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="arr">A collection of strings.</param>
        /// <param name="index">The string at the specified index represents the number to convert.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if it was converted successfully; otherwise <c>false</c>.
        /// </returns>
        private static bool TryGetHexValue(string[] arr, int index, out int value)
        {
            if (arr == null || index >= arr.Length)
            {
                value = 0;
                return false;
            }
            return int.TryParse(arr[index], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }
    }
}
