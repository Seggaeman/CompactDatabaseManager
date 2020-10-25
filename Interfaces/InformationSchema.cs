using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;

namespace CompactDatabaseManager
{
    public class INFORMATION_SCHEMA
    {
        public DataTable COLUMNS { get; set; }
        public DataTable INDEXES { get; set; }
        public DataTable KEY_COLUMN_USAGE { get; set; }
        public DataTable PROVIDER_TYPES { get; set; }
        public DataTable TABLES { get; set; }
        public DataTable TABLE_CONSTRAINTS { get; set; }
        public DataTable REFERENTIAL_CONSTRAINTS { get; set; }
    }

    public class TableTV
    {
        public string Name { get; set; }
    }

    public class InformationSchemaTV
    {
        public InformationSchemaTV()
        {
            this.TablesCollection = new List<TableTV>();
        }

        public string Name { get; set; }

        public List<TableTV> TablesCollection { get; set; }
    }

    public static class Utilities
    {
        public static ServerVersionNumber RetrieveServerVersionFromFile(String filePath)
        {
            FileStream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            fStream.Seek(16, SeekOrigin.Begin);
            byte[] versionHeaderBytes = new byte[4];
            fStream.Read(versionHeaderBytes, 0, versionHeaderBytes.Length);
            uint versionHeader = BitConverter.ToUInt32(versionHeaderBytes, 0);

            ServerVersionNumber result = ServerVersionNumber.VERSION_UNKNOWN;
            switch (versionHeader)
            {
                case 0x002dd714:
                    result = ServerVersionNumber.VERSION_3_0;
                    break;

                case 0x00357b9d:
                    result = ServerVersionNumber.VERSION_3_5;
                    break;

                default:
                    result = ServerVersionNumber.VERSION_UNKNOWN;
                    break;
            }
            fStream.Close();

            return result;
        }
    }
}
