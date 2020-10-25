using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CompactDatabaseManager
{
    public enum ServerVersionNumber
    {
        VERSION_UNKNOWN, VERSION_2_0, VERSION_3_0, VERSION_3_5, VERSION_4_0
    }

    public interface IDatabase
    {
        String ConnectionString { get; }
        void OpenConnection();
        void CloseConnection();
        System.Data.ConnectionState ConnectionState { get; }
        int ExecuteNonQuery(String queryString);
        DataTable ExecuteQuery(String queryString);
        String ServerVersion { get; }
        INFORMATION_SCHEMA InformationSchema { get; }
    }
}
