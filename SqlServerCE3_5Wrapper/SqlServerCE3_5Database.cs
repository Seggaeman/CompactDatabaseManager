using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Reflection;
using System.Data;
using Debug = System.Diagnostics.Debug;
using DataGrid = System.Windows.Controls.DataGrid;

namespace CompactDatabaseManager
{
    public class SqlServerCE3_5Database : SqlServerCEBase
    {
        private SqlServerCE3_5Database(String connectionString) : base(connectionString)
        {

        }
    }
}