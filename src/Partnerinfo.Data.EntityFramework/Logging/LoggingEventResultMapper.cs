// Copyright (c) János Janka. All rights reserved.

using System;
using System.Data;

namespace Partnerinfo.Logging.EntityFramework
{
    /// <summary>
    /// This is much faster than EF 6 materialization, but it also terrible.
    /// Fortunately, EF 7 mapping pipeline will support complex types on SP results. 
    /// </summary>
    internal struct LoggingEventResultMapper
    {
        private readonly IDataReader _reader;

        private readonly int _id;
        private readonly int _userId;
        private readonly int _userName;
        private readonly int _userEmail;
        private readonly int _categoryId;
        private readonly int _categoryName;
        private readonly int _categoryColor;
        private readonly int _objectType;
        private readonly int _objectId;
        private readonly int _objectName;
        private readonly int _contactId;
        private readonly int _contactEmail;
        private readonly int _contactName;
        private readonly int _contactState;
        private readonly int _projectId;
        private readonly int _projectName;
        private readonly int _correlationId;
        private readonly int _correlationStartDate;
        private readonly int _startDate;
        private readonly int _finishDate;
        private readonly int _browserBrand;
        private readonly int _browserVersion;
        private readonly int _mobileDevice;
        private readonly int _clientId;
        private readonly int _customUri;
        private readonly int _referrerUrl;
        private readonly int _message;

        public LoggingEventResultMapper(IDataReader reader)
        {
            _reader = reader;
            _id = reader.GetOrdinal("Id");
            _userId = reader.GetOrdinal("UserId");
            _userName = reader.GetOrdinal("UserName");
            _userEmail = reader.GetOrdinal("UserEmail");
            _categoryId = reader.GetOrdinal("CategoryId");
            _categoryName = reader.GetOrdinal("CategoryName");
            _categoryColor = reader.GetOrdinal("CategoryColor");
            _objectType = reader.GetOrdinal("ObjectType");
            _objectId = reader.GetOrdinal("ObjectId");
            _objectName = reader.GetOrdinal("ObjectName");
            _contactId = reader.GetOrdinal("ContactId");
            _contactEmail = reader.GetOrdinal("ContactEmail");
            _contactName = reader.GetOrdinal("ContactName");
            _contactState = reader.GetOrdinal("ContactState");
            _projectId = reader.GetOrdinal("ProjectId");
            _projectName = reader.GetOrdinal("ProjectName");
            _correlationId = reader.GetOrdinal("CorrelationId");
            _correlationStartDate = reader.GetOrdinal("CorrelationStartDate");
            _startDate = reader.GetOrdinal("StartDate");
            _finishDate = reader.GetOrdinal("FinishDate");
            _browserBrand = reader.GetOrdinal("BrowserBrand");
            _browserVersion = reader.GetOrdinal("BrowserVersion");
            _mobileDevice = reader.GetOrdinal("MobileDevice");
            _clientId = reader.GetOrdinal("ClientId");
            _customUri = reader.GetOrdinal("CustomUri");
            _referrerUrl = reader.GetOrdinal("ReferrerUrl");
            _message = reader.GetOrdinal("Message");
        }

        /// <summary>
        /// Gets the total number of records before paging is applied.
        /// </summary>
        /// <returns>
        /// The total number of records before paging is applied.
        /// </returns>
        public int GetTotal()
        {
            return _reader.GetInt32(_reader.GetOrdinal("TotalItemCount"));
        }

        /// <summary>
        /// Translates the current record in the <see cref="IDataReader"/> to a <see cref="EventResult"/> object.
        /// </summary>
        /// <returns>
        /// The <see cref="EventResult"/> object.
        /// </returns>
        public EventResult Translate()
        {
            return new EventResult
            {
                Id = _reader.GetInt32(_id),
                User = _reader.IsDBNull(_userId) ? null : new AccountItem
                {
                    Id = _reader.GetInt32(_userId),
                    Email = GetMailAddress(_userEmail, _userName)
                },
                Category = _reader.IsDBNull(_categoryId) ? null : new CategoryItem
                {
                    Id = _reader.GetInt32(_categoryId),
                    Name = GetStringOrNull(_categoryName),
                    Color = new ColorInfo(_reader.GetInt32(_categoryColor))
                },
                ObjectType = (ObjectType)_reader.GetByte(_objectType),
                Object = _reader.IsDBNull(_objectId) ? null : new UniqueItem
                {
                    Id = _reader.GetInt32(_objectId),
                    Name = GetStringOrNull(_objectName)
                },
                Contact = _reader.IsDBNull(_contactId) ? null : new AccountItem
                {
                    Id = _reader.GetInt32(_contactId),
                    Email = GetMailAddress(_contactEmail, _contactName)
                },
                ContactState = (ObjectState)_reader.GetByte(_contactState),
                Project = _reader.IsDBNull(_projectId) ? null : new UniqueItem
                {
                    Id = _reader.GetInt32(_projectId),
                    Name = GetStringOrNull(_projectName)
                },
                Correlation = _reader.IsDBNull(_correlationId) ? null : new EventResultBase
                {
                    Id = _reader.GetInt32(_correlationId),
                    StartDate = _reader.GetDateTime(_correlationStartDate)
                },
                StartDate = _reader.GetDateTime(_startDate),
                FinishDate = _reader.IsDBNull(_finishDate) ? default(DateTime?) : _reader.GetDateTime(_finishDate),
                BrowserBrand = (BrowserBrand)_reader.GetByte(_browserBrand),
                BrowserVersion = _reader.GetInt16(_browserVersion),
                MobileDevice = (MobileDevice)_reader.GetByte(_mobileDevice),
                ClientId = GetStringOrNull(_clientId),
                CustomUri = GetStringOrNull(_customUri),
                ReferrerUrl = GetStringOrNull(_referrerUrl),
                Message = GetStringOrNull(_message)
            };
        }

        private MailAddressItem GetMailAddress(int address, int name)
        {
            return MailAddressItem.Create(GetStringOrNull(address), GetStringOrNull(name));
        }

        private string GetStringOrNull(int ordinal)
        {
            return _reader.IsDBNull(ordinal) ? null : _reader.GetString(ordinal);
        }
    }
}
