using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlServerCe;
using System.Reflection;
using Debug = System.Diagnostics.Debug;
using DataGrid = System.Windows.Controls.DataGrid;

namespace CompactDatabaseManager
{
    public class SqlServerCEBase : IDatabase
    {
        protected SqlCeConnection _sqlCeConnection = null;
        protected INFORMATION_SCHEMA _informationSchema = null;

        public string ConnectionString
        {
            get { return _sqlCeConnection.ConnectionString; }
        }

        public System.Data.ConnectionState ConnectionState
        {
            get { return _sqlCeConnection.State; }
        }

        public string ServerVersion
        {
            get { return _sqlCeConnection.ServerVersion; }
        }

        public INFORMATION_SCHEMA InformationSchema
        {
            get
            {
                if (_informationSchema == null && _sqlCeConnection.State == System.Data.ConnectionState.Open)
                    _informationSchema = RetrieveInformationSchema();

                return _informationSchema;
            }
        }

        public void CloseConnection()
        {
            if (_sqlCeConnection != null)
            {
                switch (_sqlCeConnection.State)
                {
                    case System.Data.ConnectionState.Open:
                    case System.Data.ConnectionState.Broken:
                        _sqlCeConnection.Close();
                        break;
                }
                _sqlCeConnection.Dispose();
                _sqlCeConnection = null;
            }
        }

        public void OpenConnection()
        {
            if (_sqlCeConnection != null)
            {
                switch (_sqlCeConnection.State)
                {
                    case System.Data.ConnectionState.Broken:
                        _sqlCeConnection.Close();
                        _sqlCeConnection.Open();
                        break;

                    case System.Data.ConnectionState.Closed:
                        _sqlCeConnection.Open();
                        break;

                    default:
                        break;
                }
            }
        }

        public int ExecuteNonQuery(string queryString)
        {
            SqlCeCommand sqlCeCommand = new SqlCeCommand(queryString, _sqlCeConnection);
            SqlCeTransaction sqlCeTransaction = _sqlCeConnection.BeginTransaction(System.Data.IsolationLevel.Serializable);
            int result = sqlCeCommand.ExecuteNonQuery();
            sqlCeTransaction.Commit(CommitMode.Immediate);
            return result;
        }

        protected SqlServerCEBase(String connectionString)
        {
            _sqlCeConnection = new SqlCeConnection(connectionString);
            Debug.WriteLine(String.Format("{0},{1}",_sqlCeConnection,_sqlCeConnection.DataSource));
        }

        public static IDatabase CreateInstance(String databaseName, String password)
        {
            // Create the connection string.
            String connectionString = password == null ? String.Format("datasource='{0}';", databaseName) : String.Format("datasource='{0}'; ssce:database password={1};", databaseName, password);
            // Check if the file exists.
            String fullDatabasePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), databaseName);
            if (System.IO.File.Exists(fullDatabasePath))
                Debug.WriteLine("File exists");
            else
            {
                // Create database.
                SqlCeEngine engine = new SqlCeEngine(connectionString);
                engine.CreateDatabase();
                engine.Dispose();
            }
            return new SqlServerCEBase(connectionString);
        }

        public DataTable ExecuteQuery(String queryString)
        {
            DataTable result_L = new DataTable();
            SqlCeCommand command_L = _sqlCeConnection.CreateCommand();
            command_L.CommandText = queryString;
            command_L.CommandType = System.Data.CommandType.Text;
            SqlCeTransaction sqlCeTransaction = _sqlCeConnection.BeginTransaction(System.Data.IsolationLevel.Serializable);
            SqlCeDataReader dataReader = command_L.ExecuteReader();
            for (int i = 0; i < dataReader.FieldCount; ++i)
                result_L.Columns.Add(dataReader.GetName(i));

            while (dataReader.Read())
            {
                Object[] rowData_L = new object[dataReader.FieldCount];
                dataReader.GetValues(rowData_L);
                result_L.Rows.Add(rowData_L);
            }
            sqlCeTransaction.Commit(CommitMode.Immediate);
            return result_L;
        }

        protected INFORMATION_SCHEMA RetrieveInformationSchema()
        {
            INFORMATION_SCHEMA result_L = new INFORMATION_SCHEMA();
            String informationSchemaTypeName_L = typeof(INFORMATION_SCHEMA).Name;
            PropertyInfo[] propertyInfoArray_L = typeof(INFORMATION_SCHEMA).GetProperties();
            String formatString_L = "SELECT * FROM {0}.{1}";

            foreach (PropertyInfo propertyInfo_L in propertyInfoArray_L)
            {
                String queryString_L = String.Format(formatString_L, informationSchemaTypeName_L, propertyInfo_L.Name);
                DataTable dataTable_L = ExecuteQuery(queryString_L);
                propertyInfo_L.SetValue(result_L, dataTable_L, null);
            }
            return result_L;
        }
    }
}