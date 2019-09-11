Import-Module -Name "./Build-Versioning.psm1"


$projects = "..\src\Codeworx.Identity.Primitives\Codeworx.Identity.Primitives.csproj",
            "..\src\Codeworx.Identity.AspNetcore\Codeworx.Identity.AspNetcore.csproj", 
            "..\src\Codeworx.Identity.Cryptography\Codeworx.Identity.Cryptography.csproj", 
            "..\src\Codeworx.Identity.EntityFrameworkCore\Codeworx.Identity.EntityFrameworkCore.csproj",
            "..\src\Codeworx.Identity\Codeworx.Identity.csproj"

New-NugetPackages `
    -Projects $projects `
    -NugetServerUrl "https://www.myget.org/F/codeworx/api/v2" `
    -VersionPackage "Codeworx.Identity" `
    -VersionFilePath "..\version.json" `
    -OutputPath "..\dist\nuget\" `
    -MsBuildParams "SignAssembly=true;AssemblyOriginatorKeyFile=..\..\private\identity_signkey.snk"