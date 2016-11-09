namespace CratePsqlODBC.Tests
{
    public abstract class SqlTransportExecutionTest
    {
        protected static OdbcTransportExecutor Executor = new OdbcTransportExecutor();
        protected static OdbcResponse Response;

        public OdbcResponse Execute(string stmt)
        {
            Response = Executor.Exec(stmt);
            return Response;
        }

        public void ExecuteBulk(string stmt, string[] parameters, object[][] bulkArgs)
        {
            Executor.ExecBulkArgs(stmt, parameters, bulkArgs);
        }
    }
}
