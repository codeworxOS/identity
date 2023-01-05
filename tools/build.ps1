Import-Module -Name "./Build-Versioning.psm1"


$projects = "..\src\Codeworx.Identity.Primitives\Codeworx.Identity.Primitives.csproj",
            "..\src\Codeworx.Identity.AspNetCore\Codeworx.Identity.AspNetCore.csproj", 
            "..\src\Codeworx.Identity.Cryptography\Codeworx.Identity.Cryptography.csproj", 
            "..\src\Codeworx.Identity.Configuration\Codeworx.Identity.Configuration.csproj",
            "..\src\Codeworx.Identity\Codeworx.Identity.csproj",
            "..\src\Codeworx.Identity.Mfa.Totp\Codeworx.Identity.Mfa.Totp.csproj"

$coreVersion = New-NugetPackages `
    -Projects $projects `
    -NugetServerUrl "https://www.myget.org/F/codeworx/api/v2" `
    -VersionPackage "Codeworx.Identity" `
    -VersionFilePath "..\version.json" `
    -OutputPath "..\dist\nuget\" `
    -MsBuildParams "SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\identity_signkey.snk"

$projects = "..\src\Codeworx.Identity.EntityFrameworkCore\Codeworx.Identity.EntityFrameworkCore.csproj",
"..\src\Codeworx.Identity.EntityFrameworkCore.Migrations.SqlServer\Codeworx.Identity.EntityFrameworkCore.Migrations.SqlServer.csproj",
"..\src\Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite\Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite.csproj",
"..\src\Codeworx.Identity.EntityFrameworkCore.Api\Codeworx.Identity.EntityFrameworkCore.Api.csproj",
"..\src\Codeworx.Identity.EntityFrameworkCore.Api.NSwag\Codeworx.Identity.EntityFrameworkCore.Api.NSwag.csproj"


    New-NugetPackages `
    -Projects $projects `
    -NugetServerUrl "https://www.myget.org/F/codeworx/api/v2" `
    -VersionPackage "Codeworx.Identity.EntityFrameworkCore" `
    -VersionFilePath "..\version_ef6.json" `
    -DoNotCleanOutput `
    -OutputPath "..\dist\nuget" `
    -MsBuildParams "SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\identity_signkey.snk;EfVersion=6;IdentityCoreVersion=$($coreVersion.NugetVersion)"

    New-NugetPackages `
    -Projects $projects `
    -NugetServerUrl "https://www.myget.org/F/codeworx/api/v2" `
    -VersionPackage "Codeworx.Identity.EntityFrameworkCore" `
    -VersionFilePath "..\version_ef7.json" `
    -DoNotCleanOutput `
    -OutputPath "..\dist\nuget" `
    -MsBuildParams "SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\identity_signkey.snk;EfVersion=7;IdentityCoreVersion=$($coreVersion.NugetVersion)"



Write-Host "##vso[build.updatebuildnumber]$($coreVersion.NugetVersion)"