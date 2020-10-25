using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Data;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace CompactDatabaseManager
{
    class MainWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _windowTitle = "CompactDatabaseManager";
        private string _databasePath;
        private IDatabase _iDatabase;
        private DataTable _tableContent;
        private List<InformationSchemaTV> _infoSchemaTVCollection;


        // CallerMemberName attribute not available on .NET 4.0
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public String WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                if (_windowTitle != value)
                {
                    _windowTitle = value;
                    NotifyPropertyChanged("WindowTitle");
                }
            }
        }

        private void UpdateInfoSchemaTV()
        {
            DataTableReader reader = _iDatabase.InformationSchema.TABLES.CreateDataReader();
            // Regenerate the data for the tree view
            InformationSchemaTV updatedTV = new InformationSchemaTV()
            {
                Name = String.Format("{0} ({1})", Path.GetFileName(_databasePath), this.ServerVersionFromFile),
            };
            while (reader.Read())
            {
                TableTV tableTV = new TableTV()
                {
                    Name = reader.GetString(reader.GetOrdinal("TABLE_NAME"))
                };
                updatedTV.TablesCollection.Add(tableTV);
            }
            _infoSchemaTVCollection = new List<InformationSchemaTV>();
            _infoSchemaTVCollection.Add(updatedTV);
            NotifyPropertyChanged("InfoSchemaTVCollection");
        }

        public List<InformationSchemaTV> InfoSchemaTVCollection
        {
            get { return _infoSchemaTVCollection; }
            set
            {
                if (_infoSchemaTVCollection != value)
                {
                    _infoSchemaTVCollection = value;
                    NotifyPropertyChanged("InfoSchemaTVCollection");
                }
            }
        }

        public DataTable TableContent
        {
            get { return _tableContent; }
            set
            {
                if (_tableContent != value)
                {
                    _tableContent = value;
                    NotifyPropertyChanged("TableContent");
                }
            }
        }

        public IDatabase IDatabaseInstance
        {
            get { return _iDatabase; }
            set
            {
                if (_iDatabase != value)
                {
                    _iDatabase = value;
                }
            }
        }

        public ServerVersionNumber ServerVersionFromFile { get; set; }

        public bool OpenDatabase(String databasePath, String password)
        {
            CloseDatabase();
            bool result = false;
            _databasePath = databasePath;
            ServerVersionFromFile = Utilities.RetrieveServerVersionFromFile(databasePath);
            switch (ServerVersionFromFile)
            {
                case ServerVersionNumber.VERSION_3_0:
                    IDatabaseInstance = SqlServerCE3_1Database.CreateInstance(databasePath, password);
                    break;
                case ServerVersionNumber.VERSION_3_5:
                    IDatabaseInstance = SqlServerCE3_5Database.CreateInstance(databasePath, password);
                    break;
                case ServerVersionNumber.VERSION_4_0:
                    IDatabaseInstance = SqlServerCE4_0Database.CreateInstance(databasePath, password);
                    break;
                default:
                    break;
            }

            try
            {
                IDatabaseInstance.OpenConnection();
                if (IDatabaseInstance.ConnectionState == ConnectionState.Open)
                {
                    UpdateInfoSchemaTV();
                    result = true;
                }
                else
                {
                    CloseDatabase();
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
            return result;
        }

        public void CloseDatabase()
        {
            if (_iDatabase != null)
            {
                InfoSchemaTVCollection = null;
                TableContent = null;
                _iDatabase.CloseConnection();
                _iDatabase = null;
            }
        }

        public void UpdateDataGrid(String tableName)
        {
            String queryString = String.Format("SELECT * FROM {0}", tableName);
            TableContent = _iDatabase.ExecuteQuery(queryString);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _iDatabase.CloseConnection();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MainWindowViewModel() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
