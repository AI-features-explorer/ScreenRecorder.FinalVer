using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace ScreenRecorder.FinalVer.DataBase
{
    public class DataBaseAgent : IDataBaseAgent
    {

        public List<IDataTable> LoadColumnData(DbSet dbSet)
        {
            List<IDataTable> list = new List<IDataTable>();
            using (DataBaseContext db = new DataBaseContext())
            {
                var users = dbSet;
                foreach (IDataTable u in users) list.Add(u);
            }
            return list;
        }

        private bool ValidateLogin(string login)
        {
            DbSet set = new DataBaseContext().Accounts;
            foreach (var item in set)
            {
                if ((item as Account).Username == login)
                {
                    return false;
                }
            }
            return true;
        }

        public bool UploadAcconutData(string login, string password)
        {
            try
            {
                using (DataBaseContext db = new DataBaseContext())
                {
                    if (ValidateLogin(login))
                    {
                        Account account = new Account { ID = Guid.NewGuid(), Username = login, Password = password };
                        db.Accounts.Add(account);
                        db.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
