using DataMigration.Models;

namespace DataMigration
{
    public interface IMigration
    {
        void Start();
        void MigrateAssociation();
        ConfigModel GetConfig();

        void TestEF();
    }
}
