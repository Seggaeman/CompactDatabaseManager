using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompactDatabaseManager
{
    public class SqlServerCE4_0Database : SqlServerCEBase
    {
        private SqlServerCE4_0Database(String connectionString) : base(connectionString)
        {

        }
    }
}