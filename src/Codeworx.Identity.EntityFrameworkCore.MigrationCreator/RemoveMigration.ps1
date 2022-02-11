function RemoveMigration([string]$ProjectPath,
                      [string]$DbType) {

echo "Removing Migration..."
dotnet ef migrations remove --project $ProjectPath --startup-project $MigrationsCreatorPath --configuration $DbType
}

$MigrationsCreatorPath = "$PSScriptRoot\Codeworx.Identity.EntityFrameworkCore.MigrationCreator.csproj"

#Extend here if new migrations are added.
RemoveMigration "$PSScriptRoot\..\Codeworx.Identity.EntityFrameworkCore.Migrations.SqlServer\Codeworx.Identity.EntityFrameworkCore.Migrations.SqlServer.csproj" 'MsSql'
RemoveMigration "$PSScriptRoot\..\Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite\Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite.csproj" 'Sqlite'

Read-Host -Prompt 'Press any key to close...'