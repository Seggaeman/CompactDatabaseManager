using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Reflection;
using Debug = System.Diagnostics.Debug;
using DataGrid = System.Windows.Controls.DataGrid;

namespace CompactDatabaseManager
{
    public class SqlServerCE3_1Database : SqlServerCEBase
    {
        private SqlServerCE3_1Database(String connectionString) : base(connectionString)
        {

        }
    }
}
