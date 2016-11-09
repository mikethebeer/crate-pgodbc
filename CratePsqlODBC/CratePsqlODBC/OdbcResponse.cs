using System;
using System.Collections.Generic;

namespace CratePsqlODBC
{
    public class OdbcResponse
    {
        private readonly long _rowCount;

        /// <summary>
        /// Creates the Odbc Response 
        /// </summary>
        /// <param name="cols">the list of column names in the response</param>
        /// <param name="rows">the response of the statement that got executed</param>
        /// <param name="rowCount">the number of rows that have been fetched</param>
        public OdbcResponse(string[] cols, object[][] rows, List<Type> types)
        {
            Rows = rows;
            Types = types;
            if (rows != null)
                _rowCount = rows.GetLength(0);
            else
                _rowCount = 0;
        }

        /// <summary>
        /// Returns the number of rows that have been fetched. 
        /// It returns -1 if the table is unknown.
        /// </summary>
        public long RowCount
        {
            get { return _rowCount; }
        }

        /// <summary>
        /// The fetched response
        /// </summary>
        public object[][] Rows { get; set; }

        /// <summary>
        /// The fetched SQL Data Types
        /// </summary>
        public List<Type> Types { get; set; }
    }
}
