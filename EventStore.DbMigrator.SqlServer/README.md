# Event Store DB Migrator - SQL Server

Apply SQL Server database migration scripts.
A database must exist and the user must be able to apply schema changes.

## Publish

```bash
dotnet publish -c Release -o ./publish --runtime alpine-x64 --self-contained true /p:PublishTrimmed=true /p:PublishSingleFile=true
```

## Build image

```bash
docker build -t <registry>/event-store-dbmigrator-sqlserver:<tag> .
```

## Run container

```bash
docker run --rm -e "DB_CONNECTION_STRING=<value>" <registry>/event-store-dbmigrator-sqlserver:<tag>
```