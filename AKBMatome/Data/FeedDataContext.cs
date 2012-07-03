using System.Data.Linq;
using Microsoft.Phone.Data.Linq;

namespace AKBMatome.Data
{
    public class FeedDataContext : DataContext
    {
        private const string DBConnectionString = "Data Source=isostore:/Feed.sdf";
        private const int DBSchemaVersion = 1;

        public FeedDataContext()
            : base(DBConnectionString)
        {
            if (DatabaseExists() == false)
            {
                System.Diagnostics.Debug.WriteLine("Create database");
                CreateDatabase();
                DatabaseSchemaUpdater updater = this.CreateDatabaseSchemaUpdater();
                int version = updater.DatabaseSchemaVersion;
                updater.DatabaseSchemaVersion = DBSchemaVersion;
                updater.Execute();
            }
            else
            {
                DatabaseSchemaUpdater updater = this.CreateDatabaseSchemaUpdater();
                int version = updater.DatabaseSchemaVersion;
                if (version == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Update database from " + version);
                    updater.AddColumn<FeedGroup>("Class");
                    updater.AddColumn<FeedGroup>("AccentColor");
                    updater.AddColumn<FeedGroup>("Status");
                    updater.AddColumn<FeedGroup>("Subscribe");
                    updater.AddColumn<FeedChannel>("Status");
                    updater.DatabaseSchemaVersion = DBSchemaVersion;
                    updater.Execute();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No database changes");
                }
            }
        }

        public Table<FeedGroup> FeedGroups;
        public Table<FeedChannel> FeedChannels;
    }
}
