using System;
using System.Data;
using System.Data.SqlClient;
using Numaka.Common.Contracts;

namespace EventStore.Repository
{
    public class SqlDbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _dbConnectionString;

        public SqlDbConnectionFactory(string dbConnectionString)
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
                throw new ArgumentNullException(nameof(dbConnectionString));

            _dbConnectionString = dbConnectionString;
        }

        public IDbConnection Create() => new SqlConnection(_dbConnectionString);
    }
}