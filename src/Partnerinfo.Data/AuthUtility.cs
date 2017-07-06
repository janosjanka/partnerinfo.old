// Copyright (c) János Janka. All rights reserved.

using System;
using System.Text;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using Partnerinfo.Project;

namespace Partnerinfo
{
    public static class AuthUtility
    {
        /// <summary>
        /// Encrypts the <paramref name="ticket" /> for transmission on the URL.
        /// </summary>
        /// <param name="ticket">The ticket to encrypt.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static string Protect(AuthTicket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            var claims = JsonConvert.SerializeObject(ticket, typeof(AuthTicket), Formatting.None, JsonNetUtility.Settings);
            var stream = Encoding.UTF8.GetBytes(claims);
            var encodedValue = MachineKey.Protect(stream, "project", "authentication");
            return HttpServerUtility.UrlTokenEncode(encodedValue);
        }

        /// <summary>
        /// Decrypts the specified <paramref name="ticket" />.
        /// </summary>
        /// <param name="ticket">The ticket to decrypt.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static AuthTicket Unprotect(string ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            var stream = HttpServerUtility.UrlTokenDecode(ticket);
            var decodedValue = MachineKey.Unprotect(stream, "project", "authentication");
            var decodedValueUtf8 = Encoding.UTF8.GetString(decodedValue);
            return JsonConvert.DeserializeObject<AuthTicket>(decodedValueUtf8, JsonNetUtility.Settings);
        }

        /// <summary>
        /// Creates a <see cref="AuthTicket" /> from the specified <see cref="ContactItem" />.
        /// </summary>
        /// <param name="contact">The contact to convert.</param>
        /// <returns>
        /// The <see cref="AuthTicket" />.
        /// </returns>
        public static AuthTicket Create(ContactItem contact)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }
            return new AuthTicket
            (
                contact.Id,
                contact.FacebookId,
                contact.Email,
                contact.FirstName,
                contact.LastName,
                contact.NickName,
                contact.Gender,
                contact.Birthday,
                contact.Phones
            );
        }

        /// <summary>
        /// Creates a <see cref="AuthTicket" /> from the specified <see cref="AccountItem" />.
        /// </summary>
        /// <param name="account">The contact to convert.</param>
        /// <returns>
        /// The <see cref="AuthTicket" />.
        /// </returns>
        public static AuthTicket Create(AccountItem account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }
            return new AuthTicket
            (
                account.Id,
                account.FacebookId,
                account.Email,
                account.FirstName,
                account.LastName,
                account.NickName,
                account.Gender,
                account.Birthday,
                null
            );
        }
    }
}