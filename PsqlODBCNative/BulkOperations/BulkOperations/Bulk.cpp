#include <Windows.h>
#include <sql.h>
#include <sqltypes.h>
#include <sqlext.h>
#include <cstdio>
#include <cstdlib>
#include <iostream>
#include <cstring>

using namespace std;

static void PrintError(SQLSMALLINT siType, SQLHANDLE shHandle)
{
    SQLINTEGER siError;
    SQLSMALLINT siAvail;
    SQLCHAR szError[1024], szState[256];
    SQLGetDiagRec(siType, shHandle, 1, szState, &siError, szError, sizeof(szError), &siAvail);
    printf("ERROR: %s\n", szError);
}

int main(int argc, char* argv[])
{
    // Get the environment
    SQLHENV hdlEnv;
    SQLAllocHandle(SQL_HANDLE_ENV, SQL_NULL_HANDLE, &hdlEnv);
    SQLSetEnvAttr(hdlEnv, SQL_ATTR_ODBC_VERSION,
        (void*)SQL_OV_ODBC3, 0); // or SQL_OV_ODBC30

    // Set up a connection to the database
    SQLHDBC hdlDbc;
    SQLAllocHandle(SQL_HANDLE_DBC, hdlEnv, &hdlDbc);
    cout << "Connect to DB" << endl;
    SQLRETURN rc;

    // Connectionstring
    const char *dsnName = "PsqlODBC 32-bit";
    // const char *dsnName = "Crate ODBC Driver 32-bit";
    rc = SQLConnect(hdlDbc, (SQLCHAR*)dsnName, SQL_NTS, NULL, 0, NULL, 0);
    // If connection did not succeed, exit.
    if (rc != SQL_SUCCESS) return 1;

	// Turn off autocommit, so multiple batches can be loaded in a
	// transaction.
	/*
	cout << "Disable Autocommit." << endl;
	rc = SQLSetConnectAttr(hdlDbc, SQL_AUTOCOMMIT, SQL_AUTOCOMMIT_OFF, 0);
	if (rc != SQL_SUCCESS) printf("Failed to disable autocommit!\n");
	*/

    // Set up a statement handle
    SQLHSTMT hdlStmt;
    SQLAllocHandle(SQL_HANDLE_STMT, hdlDbc, &hdlStmt);

    // Create a table into which we can store data
    cout << "Create table." << endl;
	rc = SQLExecDirect(hdlStmt,
			(SQLCHAR*)"CREATE TABLE IF NOT EXISTS doc.customers (CustID int, CustName string, Phone_Number string)",
			SQL_NTS);
    if (rc != SQL_SUCCESS)
        PrintError(SQL_HANDLE_STMT, hdlStmt);

    // Create the prepared statement.
    rc = SQLPrepare(hdlStmt, (SQLTCHAR*)"INSERT INTO doc.customers (CustID, "
        "CustName,  Phone_Number) VALUES(?,?,?)", SQL_NTS);
    if (rc != SQL_SUCCESS)
        PrintError(SQL_HANDLE_STMT, hdlStmt);

    // The data
    char custNames[][50] = { "Allen, Anna", "Brown, Bill", "Chu, Cindy",
        "Dodd, Don" };
    SQLINTEGER custIDs[] = { 100, 101, 102, 103 };
    char phoneNums[][15] = { "1-617-555-1234", "1-781-555-1212",
        "1-508-555-4321", "1-617-555-4444" };

    // Bind the data arrays to the parameters in the prepared SQL statement
    rc = SQLBindParameter(hdlStmt, 1, SQL_PARAM_INPUT, SQL_C_LONG, SQL_INTEGER,
        0, 0, (SQLPOINTER)custIDs, sizeof(*custIDs), NULL);
    rc = SQLBindParameter(hdlStmt, 2, SQL_PARAM_INPUT, SQL_C_CHAR, SQL_VARCHAR,
        50, 0, (SQLPOINTER)custNames, sizeof(custNames[0]), NULL);
    rc = SQLBindParameter(hdlStmt, 3, SQL_PARAM_INPUT, SQL_C_CHAR, SQL_CHAR,
        15, 0, (SQLPOINTER)phoneNums, sizeof(phoneNums[0]), NULL);

    // Tell ODBC how many rows are in the array
    SQLSetStmtAttr(hdlStmt, SQL_ATTR_PARAMSET_SIZE, (SQLPOINTER)4, 0);
	
	// hold the nr. of accepted and rejected rows
    SQLINTEGER acc_rows = 0;

	// execute the prepared statement
    rc = SQLExecute(hdlStmt);
    if (rc != SQL_SUCCESS)
		PrintError(SQL_HANDLE_STMT, hdlStmt);
    SQLRowCount(hdlStmt, &acc_rows);
	printf("Rows affected: %d\n", (int)acc_rows);

	// Done with batches, commit the transaction
	cout << "Commit Transaction" << endl;
	rc = SQLEndTran(SQL_HANDLE_DBC, hdlDbc, SQL_COMMIT);
	if (rc != SQL_SUCCESS)
		printf("Failed to commit transaction.\n");

    // Get the accepted rows from the transaction.
    SQLRowCount(hdlStmt, &acc_rows);
    printf("Transaction affected %d rows.\n", (int)acc_rows);
    
	// tear-down
    rc = SQLExecDirect(hdlStmt, (SQLCHAR*)"DROP TABLE doc.customers",
        SQL_NTS);
    if (rc != SQL_SUCCESS)
        PrintError(SQL_HANDLE_STMT, hdlStmt);
	
    cout << "Free handles." << endl;
    SQLFreeHandle(SQL_HANDLE_STMT, hdlStmt);
    SQLFreeHandle(SQL_HANDLE_DBC, hdlDbc);
    SQLFreeHandle(SQL_HANDLE_ENV, hdlEnv);
	getchar();
    return 0;
}