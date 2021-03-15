// Copyright (c) János Janka. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Server;

namespace Partnerinfo
{
    internal static class DbParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="System.Data.SqlClient.SqlParameter" /> class.
        /// If the parameter is null, this function returns with a DBNull value avoiding invalid parameter values.
        /// </summary>
        /// <param name="parameterName">The name of the stored procedure parameter.</param>
        /// <param name="value">An <see cref="System.Object" /> that is the value of the <see cref="System.Data.SqlClient.SqlParameter" />.</param>
        /// <param name="sqlDbType">Type of the SQL database.</param>
        /// <param name="size">The size.</param>
        /// <param name="direction">One of the <see cref="System.Data.ParameterDirection" /> values.</param>
        /// <returns>
        /// The created <see cref="SqlParameter" /> instance.
        /// </returns>
        public static SqlParameter Value(string parameterName, object value, SqlDbType sqlDbType, int size, ParameterDirection direction = ParameterDirection.Input)
        {
            return new SqlParameter(parameterName, value ?? DBNull.Value) { SqlDbType = sqlDbType, Size = size, Direction = direction };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="System.Data.SqlClient.SqlParameter" /> class.
        /// If the parameter is null, this function returns with a DBNull value avoiding invalid parameter values.
        /// </summary>
        /// <param name="parameterName">The name of the stored procedure parameter.</param>
        /// <param name="value">An <see cref="System.Object" /> that is the value of the <see cref="System.Data.SqlClient.SqlParameter" />.</param>
        /// <param name="sqlDbType">Type of the SQL database.</param>
        /// <param name="direction">One of the <see cref="System.Data.ParameterDirection" /> values.</param>
        /// <returns>
        /// The created <see cref="SqlParameter" /> instance.
        /// </returns>
        public static SqlParameter Value(string parameterName, object value, SqlDbType sqlDbType, ParameterDirection direction = ParameterDirection.Input)
        {
            return new SqlParameter(parameterName, value ?? DBNull.Value) { SqlDbType = sqlDbType, Direction = direction };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="System.Data.SqlClient.SqlParameter" /> class.
        /// If the parameter is null, this function returns with a DBNull value avoiding invalid parameter values.
        /// </summary>
        /// <param name="parameterName">The name of the stored procedure parameter.</param>
        /// <param name="value">An <see cref="System.Object" /> that is the value of the <see cref="System.Data.SqlClient.SqlParameter" />.</param>
        /// <param name="direction">One of the <see cref="System.Data.ParameterDirection" /> values.</param>
        /// <returns>
        /// The created <see cref="SqlParameter" /> instance.
        /// </returns>
        public static SqlParameter Value(string parameterName, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            return new SqlParameter(parameterName, value ?? DBNull.Value) { Direction = direction };
        }

        /// <summary>
        /// Initializes a new instance of the dbo.IdListType table value type.
        /// </summary>
        /// <param name="parameterName">The name of the stored procedure parameter.</param>
        /// <param name="idList">A collection of unique identifiers.</param>
        /// <returns>
        /// The created <see cref="SqlParameter" /> instance.
        /// </returns>
        public static SqlParameter IdList(string parameterName, IEnumerable<int> idList)
        {
            return new SqlParameter(parameterName, SqlDbType.Structured)
            {
                TypeName = "dbo.IdListType",
                Value = CreateDataRecords(idList, "Id"),
                IsNullable = true
            };
        }

        /// <summary>
        /// Initializes a new instance of the dbo.ClientListType table value type.
        /// </summary>
        /// <param name="paramName">The name of the stored procedure parameter.</param>
        /// <param name="clientList">A collection of unique identifiers.</param>
        /// <returns>
        /// The created <see cref="SqlParameter" /> instance.
        /// </returns>
        public static SqlParameter ClientList(string paramName, IEnumerable<string> clientList)
        {
            return new SqlParameter(paramName, SqlDbType.Structured)
            {
                TypeName = "dbo.ClientListType",
                Value = CreateDataRecords(clientList, "Id", SqlDbType.VarChar, 64),
                IsNullable = true
            };
        }

        /// <summary>
        /// Initializes a new instance of the dbo.EmailTableType table value type.
        /// </summary>
        /// <param name="paramName">The name of the stored procedure parameter.</param>
        /// <param name="emailList">A collection of unique identifiers.</param>
        /// <returns>
        /// The created <see cref="SqlParameter" /> instance.
        /// </returns>
        public static SqlParameter EmailList(string paramName, IEnumerable<string> emailList)
        {
            return new SqlParameter(paramName, SqlDbType.Structured)
            {
                TypeName = "dbo.EmailListType",
                Value = CreateDataRecords(emailList, "Id", SqlDbType.NVarChar, 256),
                IsNullable = true
            };
        }

        /// <summary>
        /// Initializes a new instance of the dbo.UriTable value type.
        /// </summary>
        /// <param name="paramName">The name of the stored procedure parameter.</param>
        /// <param name="uri">A collection of unique identifiers.</param>
        /// <returns>
        /// The created <see cref="SqlParameter" /> instance.
        /// </returns>
        public static SqlParameter UriValue(string paramName, string uri)
        {
            return new SqlParameter(paramName, SqlDbType.VarChar, 64) { Value = uri };
        }

        /// <summary>
        /// Initializes a new instance of the dbo.UriTable value type.
        /// </summary>
        /// <param name="paramName">The name of the stored procedure parameter.</param>
        /// <param name="uriList">A collection of unique identifiers.</param>
        /// <returns>
        /// The created <see cref="SqlParameter" /> instance.
        /// </returns>
        public static SqlParameter UriTable(string paramName, IEnumerable<string> uriList)
        {
            return new SqlParameter(paramName, SqlDbType.Structured)
            {
                TypeName = "dbo.UriListType",
                Value = CreateDataRecords(uriList, "Uri", SqlDbType.VarChar, 64),
                IsNullable = true
            };
        }

        /// <summary>
        /// Creates a collection of unique integer indentifiers.
        /// </summary>
        /// <param name="list">A collection of unique integer indentifiers.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The <see cref="IEnumerable{SqlDataRecord}" /> collection.
        /// </returns>
        private static IEnumerable<SqlDataRecord> CreateDataRecords(IEnumerable<int> list, string name)
        {
            if (list == null)
            {
                return null;
            }
            var dataRecords = new Queue<SqlDataRecord>();
            foreach (int id in list.Distinct())
            {
                var dataRecord = new SqlDataRecord(new SqlMetaData(name, SqlDbType.Int));
                dataRecord.SetInt32(0, id);
                dataRecords.Enqueue(dataRecord);
            }
            return dataRecords.Count > 0 ? dataRecords : null;
        }

        /// <summary>
        /// Creates a collection of unique string indentifiers.
        /// </summary>
        /// <param name="list">A collection of unique string indentifiers.</param>
        /// <param name="name">The name.</param>
        /// <param name="dbType">Type of the database.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>
        /// The <see cref="IEnumerable{SqlDataRecord}" /> collection.
        /// </returns>
        private static IEnumerable<SqlDataRecord> CreateDataRecords(IEnumerable<string> list, string name, SqlDbType dbType, int maxLength)
        {
            if (list == null)
            {
                return null;
            }
            var dataRecords = new Queue<SqlDataRecord>();
            foreach (string id in list.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (id != null)
                {
                    var dataRecord = new SqlDataRecord(new SqlMetaData(name, dbType, maxLength));
                    dataRecord.SetString(0, id);
                    dataRecords.Enqueue(dataRecord);
                }
            }
            return dataRecords.Count > 0 ? dataRecords : null;
        }
    }
}
