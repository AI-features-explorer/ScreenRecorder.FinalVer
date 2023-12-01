using System.Collections.Generic;
using System.Data.Entity;

namespace ScreenRecorder.FinalVer.DataBase
{
    interface IDataBaseAgent
    {
        List<IDataTable> LoadColumnData(DbSet dbSet);
    }
}
