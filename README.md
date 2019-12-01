# DataMigration

To use program properlly you need create `database.json` file with structure like below
```
{
  "sourceConfig": {
    "connectionString": "Host=<hostname>;Port=5432;Password=<password>;Username=<username>;Database=<db name>"
  },
  "hostConfig": {
    "connectionString": "Host=<hostname>;Port=5432;Password=<password>;Username=<username>;Database=<db name>"
  }
}
```
