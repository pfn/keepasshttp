using KeePassLib;

namespace KeePassHttp
{
    class PwEntryDatabase
    {
        private PwEntry _entry;
        public PwEntry entry
        {
            get { return _entry; }
        }
        private PwDatabase _database;
        public PwDatabase database
        {
            get { return _database; }
        }

        //public PwEntryDatabase(ref PwEntry e, ref PwDatabase db)
        public PwEntryDatabase(PwEntry e, PwDatabase db)
        {
            _entry = e;
            _database = db;
        }
    }
}
