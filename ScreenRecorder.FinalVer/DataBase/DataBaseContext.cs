using System.Data.Entity;

namespace ScreenRecorder.FinalVer.DataBase
{
    class DataBaseContext : DbContext
    {
        public DataBaseContext()
            : base("DbConnection")
        { }

        public DbSet<Account> Accounts { get; set; }

    }
}
