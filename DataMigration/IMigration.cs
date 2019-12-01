using DataMigration.Models;

namespace DataMigration
{
    public interface IMigration
    {
        void MigrateUsers();
        void MigrateAssociation();
        void MigrateUnits();
        ConfigModel GetConfig();

        void TestEF();
    }
}
