namespace CratePsqlODBC.Tests
{
    internal class Setup
    {
        private OdbcTransportExecutor executor;
        public Setup(OdbcTransportExecutor executor)
        {
            this.executor = executor;
        }

        public void SetupCrateTypes()
        {
            string stmt = "CREATE TABLE IF NOT EXISTS doc.crate_types (" +
                "id INT, " +
                "ean8 LONG, " +
                "ipv4 IP, " +
                "pin GEO_POINT, " +
                "yesno BOOLEAN, " +
                "number FLOAT, " +
                "str STRING, " +
                "random_digit DOUBLE, " +
                "unix_time TIMESTAMP" +
                ")" +
                "CLUSTERED INTO 4 SHARDS " +
                "WITH (number_of_replicas = 0)";

            executor.Exec(stmt);

            stmt = "INSERT INTO doc.crate_types (id, ean8, ipv4, pin, yesno, number, str, random_digit, unix_time) " +
                    "VALUES (?,?,?,?,?,?,?,?,?)";

            object[][] rows =
            {
                new object[]
                {
                    "1", "72159460", "94.15.205.243",
                    "POINT (170.13115159835087 -26.849426887361176)", false, 9058.98,
                    "TrgDQFsluQQKfBhLXiGx", 5.0, 295377940
                },
                new object[]
                {
                    "2", "295377940", "6.48.161.197",
                    "POINT (-34.113056930221035 63.42166140297837)", 1, -14.92426,
                    "hxAOUZQVpEGqpFmIDwHv", 3.0, 668452050
                }
            };

            executor.ExecBulkArgs(stmt, new[] { "id", "ean8", "ipv4", "pin", "yesno", "number", "str", "random_digit", "unix_time" }, rows);
            executor.Exec("refresh table doc.crate_types");
        }

        public void CleanCrateTypes()
        {
            executor.Exec("drop table doc.crate_types");
        }
    }
}
