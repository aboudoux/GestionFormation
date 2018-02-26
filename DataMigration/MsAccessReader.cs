using System.Data;
using System.Data.OleDb;

namespace DataMigration
{
    public class MsAccessReader
    {
        private readonly string _mdbFilePath;

        public MsAccessReader(string mdbFilePath)
        {
            _mdbFilePath = mdbFilePath;
        }

        public DataRowCollection GetRows(string tableName)
        {
            var myDataSet = new DataSet();
            var myAccessConn = new OleDbConnection($"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={_mdbFilePath}");

            try
            {
                var myAccessCommand = new OleDbCommand($"SELECT * from [{tableName}]", myAccessConn);
                var myDataAdapter = new OleDbDataAdapter(myAccessCommand);

                myAccessConn.Open();
                myDataAdapter.Fill(myDataSet, tableName);
            }
            finally
            {
                myAccessConn.Close();
            }

            return myDataSet.Tables[tableName].Rows;
        }
    }
}