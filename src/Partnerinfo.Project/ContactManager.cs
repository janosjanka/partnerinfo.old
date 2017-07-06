// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Excel;

namespace Partnerinfo.Project
{
    public sealed class ContactManager
    {
        private readonly ProjectManager _projectManager;

        private readonly IDictionary<string, string> _columns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Email", "Email" },
            { "N\u00E9v", "Name" },
            { "Keresztn\u00E9v", "FirstName" },
            { "Vezet\u00E9kn\u00E9v", "LastName" },
            { "Neme", "Gender" },
            { "Sz\u00FClet\u00E9snap", "Birthday" }
        };

        public ContactManager(ProjectManager projectManager)
        {
            if (projectManager == null)
            {
                throw new ArgumentNullException("projectManager");
            }
            _projectManager = projectManager;
        }

        /// <summary>
        /// Imports contacts from Excel binary.
        /// </summary>
        /// <param name="fileStream">The file stream to import.</param>
        public async Task ImportFromXlsAsync(ProjectItem project, Stream fileStream, CancellationToken cancellationToken)
        {
            using (var reader = ExcelReaderFactory.CreateBinaryReader(fileStream))
            {
                await ImportFromExcelAsync(project, reader, cancellationToken);
            }
        }

        /// <summary>
        /// Imports contacts from Excel XML.
        /// </summary>
        /// <param name="fileStream">The file stream to import.</param>
        public async Task ImportFromXlsxAsync(ProjectItem project, Stream fileStream, CancellationToken cancellationToken)
        {
            using (var reader = ExcelReaderFactory.CreateOpenXmlReader(fileStream))
            {
                await ImportFromExcelAsync(project, reader, cancellationToken);
            }
        }

        private Task ImportFromExcelAsync(ProjectItem project, IExcelDataReader reader, CancellationToken cancellationToken)
        {
            reader.IsFirstRowAsColumnNames = true;

            var contacts = new List<ContactItem>();
            using (var dataSet = reader.AsDataSet())
            {
                DataTable table = dataSet.Tables[0];
                foreach (DataRow row in table.Rows)
                {
                    var contact = new ContactItem();
                    foreach (DataColumn col in table.Columns)
                    {
                        SetContactProperty(contact, _columns[col.ColumnName], row[col.ColumnName]);
                    }
                    contacts.Add(contact);
                }
            }

            return _projectManager.AddContactsAsync(project, contacts, cancellationToken);
        }

        private void SetContactProperty(ContactItem contact, string property, object value)
        {
            if (value != null)
            {
                string val = value.ToString();

                if (!string.IsNullOrEmpty(val))
                {
                    switch (property)
                    {
                        case "Email":
                            contact.Email = MailAddressItem.Create(val, contact.Email.Name);
                            break;
                        case "Name":
                            contact.Email = MailAddressItem.Create(contact.Email.Address, val);
                            break;
                        case "FirstName":
                            contact.FirstName = val;
                            break;
                        case "LastName":
                            contact.LastName = val;
                            break;
                        case "Gender":
                            contact.Gender = (PersonGender)Enum.Parse(typeof(PersonGender), val);
                            break;
                        case "Birthday":
                            contact.Birthday = Convert.ToDateTime(val);
                            break;
                    }
                }
            }
        }
    }
}
