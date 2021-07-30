function AddMigration([string]$ProjectPath,
                      [string]$DbType) {

echo "Creating Migration..."
dotnet ef migrations add $MigrationName --project $ProjectPath --startup-project $MigrationsCreatorPath --configuration $DbType
}

$MigrationName = Read-Host -Prompt 'Migration Name'
$MigrationsCreatorPath = "$PSScriptRoot\Codeworx.Identity.EntityFrameworkCore.MigrationCreator.csproj"

#Extend here if new migrations are added.
AddMigration "$PSScriptRoot\..\Codeworx.Identity.EntityFrameworkCore.Migrations.SqlServer\Codeworx.Identity.EntityFrameworkCore.Migrations.SqlServer.csproj" 'MsSql'
AddMigration "$PSScriptRoot\..\Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite\Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite.csproj" 'Sqlite'

Read-Host -Prompt 'Press any key to close...'