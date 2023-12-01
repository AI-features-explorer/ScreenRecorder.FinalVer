using System;

namespace ScreenRecorder.FinalVer.DataBase
{
    class Account : IDataTable
    {
        public Guid ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
