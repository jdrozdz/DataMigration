# DataMigration

To use program properly you need create `database.json` file with structure like below
```json
{
  "sourceConfig": {
    "connectionString": "Host=<hostname>;Port=5432;Password=<password>;Username=<username>;Database=<db name>"
  },
  "hostConfig": {
    "connectionString": "Host=<hostname>;Port=5432;Password=<password>;Username=<username>;Database=<db name>"
  }
}
```
