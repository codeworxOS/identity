Import-Module -Name "./Invoke-MsBuild.psm1"
Add-Type -AssemblyName System.IO.Compression.FileSystem
Add-Type -AssemblyName System.Xml.Linq

$ErrorActionPreference = "Stop"

function Unzip {
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

function New-NugetPackages {
    <#
    .SYNOPSIS
    Build Nuget Packages
    
    .DESCRIPTION
    Build Nuget Packages for provided Projects
    
    .EXAMPLE
    Build-NugetPackages -NugetServerUrl "http://www.nuget.org/api/v2" -Projects "..\src\bla.csproj",".\src\bla2.csproj" -VersionPackage "EntityFramework"
    
    .NOTES
    General notes

    .PARAMETER Projects
    A list of MsBuild Project Files

    .PARAMETER NugetServerUrl
    The URL of the v2 oData Nuget Server endpoint

    .PARAMETER VersionPackage
    The Name of the Package, used to detect the next Version.

    .PARAMETER VersionFilePath
    Override the default version.json file

    .PARAMETER OutputPath
    override the default output path.
    
    .PARAMETER MsBuildParams
    additional msbuild parameters.

    .PARAMETER DoNotCleanOutput
    If set, the output folder is not cleand before build.

    #>
    [CmdletBinding()]
    param
    (
        [parameter(Position = 0, Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [Alias("Source", "S")]
        [string] $NugetServerUrl,

        [parameter(Position = 1, Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [Alias("P")]
        [string] $VersionPackage,

        [parameter(Position = 2, Mandatory = $true)]
        [string[]] $Projects,

        [parameter(Position = 3, Mandatory = $false)]
        [Alias("V")]
        [string] $VersionFilePath,
        
        [parameter(Position = 4, Mandatory = $false)]
        [Alias("O")]
        [string] $OutputPath,

        [parameter(Position = 5, Mandatory = $false)]
        [string] $MsBuildParams,
        
        [parameter(Position = 6, Mandatory = $false)]
        [switch] $DoNotCleanOutput
    )

    BEGIN { }
    END { }
    PROCESS {

        $projects = $Projects | % { [System.IO.FileInfo]::new($_).FullName }
        
        
        if ([string]::IsNullOrWhiteSpace($OutputPath) ) {
            $OutputPath = ".\nuget";
        }
        $outputDir = [System.Io.DirectoryInfo]::new($OutputPath)
        $output = $outputDir.FullName
       
        if ([string]::IsNullOrWhiteSpace($VersionFilePath)) {
            $VersionFilePath = "..\version.json"
        }
       
        $versionFile = [System.Io.FileInfo]::new($VersionFilePath)
       
        if (-Not $DoNotCleanOutput) {
            Write-Host "Cleaning output folder:"
            Remove-Item "$output\*.nupkg" -ErrorAction Ignore
        }
        
        $data = Get-Content $versionFile.FullName | ConvertFrom-Json

        if (Test-Path env:BUILD_PRERELEASE) {
            $data = $data | Add-Member -NotePropertyMembers @{prerelease=$env:BUILD_PRERELEASE} -PassThru
        }

        $nextVersion = Get-NugetVersionInfo -NugetServerUrl $NugetServerUrl -Package $VersionPackage -Major $data.major -Minor $data.minor -BuildNumberPrefix $data.buildNumberPrefix -Prerelease $data.prerelease    
        
        
        Write-Host "##vso[build.updatebuildnumber]$($nextVersion.NugetVersion)"
        
        $params = "Configuration=Release;Version=$($nextVersion.NugetVersion);AssemblyVersion=$($nextVersion.Major).0.0.0;FileVersion=$($nextVersion.Major).$($nextVersion.Minor).$($nextVersion.Build).$($nextVersion.Release)"
        
        if ( -not [string]::IsNullOrWhiteSpace($MsBuildParams)) {
            $params = "$params;$MsBuildParams"
        }
        
        $projects | foreach { 
            dotnet pack $_  -o "$output" -p:"$params" 
            if ($lastexitcode -ne 0) {
                throw ("Exec: " + $errorMessage)
            }
        }
    }
}

function Get-NugetVersionInfo {
    <#
	.SYNOPSIS
	Queries the Nuget Server for the currentVersion and returns the next Version to Use.

	.DESCRIPTION
	Queries the Nuget Server for the currentVersion and returns the next Version to Use.

	.PARAMETER NugetServerUrl
	The Url of the Nuget v2 oData Api.

	.PARAMETER Package
	The Id of the NuGet Package to query.

	.PARAMETER Major
    The Major Version Numer of the Package

    .PARAMETER Minor
    The Minor Version Numer of the Package

    .PARAMETER BuildNumberPrefix
    The Numeric Prefix of the Build Number. Must be between 0 and 65
    
	.PARAMETER Prerelease
    The Prerelease Information

    .OUTPUTS
    Major = The Major Version for the next build.
    Minor = The Minor Version for the next build.
    Build = The Build Version for the next build.
    Release = The Release Version for the next build.
    NugetVersion = The full NuGet Version String.
    
	.EXAMPLE
	$nextVerion = Get-NugetVersionInfo -NugetServer "http://www.nuget.org/api/v2" -Package EntityFramework -Major 1 -Minor 0 -BuildNumberPrefix 1 -Prerelease beta

    Write-Host "AssemblyFileVersion $($nextVersion.Major).$($nextVersion.Minor).$($nextVersion.Build).$($nextVersion.Release) NugetVersion $($nextVersion.NugetVersion)"
    
    .NOTES
	Name:   Get-NugetVersionInfo
	Author: Raphael Schwarz
	Version: 1.0.0
#>
    [CmdletBinding()]
    param
    (
        [parameter(Position = 0, Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [Alias("Source", "S")]
        [string] $NugetServerUrl,

        [parameter(Position = 1, Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [Alias("P")]
        [string] $Package,

        [parameter(Position = 2, Mandatory = $true)]
        [int] $Major,

        [parameter(Position = 3, Mandatory = $true)]
        [int] $Minor,

        [parameter(Position = 4, Mandatory = $true)]
        [int] $BuildNumberPrefix,

        [parameter(Position = 5, Mandatory = $false)]
        [Alias("Pre")]
        [string] $Prerelease
    )

    BEGIN { }
    END { }
    PROCESS {
       
        # Build our hash table that will be returned.
        $result = @{}
        $result.Major = $Major
        $result.Minor = $Minor
        $result.Build = $BuildNumberPrefix * 1000
        $result.Release = 0
        $result.NugetVersion = "$Major.$Minor.0"
        
        $lower = ""
        $upper = "$Major.$($Minor + 1).0"
        if ($Minor -eq 0) {
            $lower = "$($Major - 1).999999.0"
        }
        else {
            $lower = "$Major.$($Minor - 1).0"
        }
        
        $searchUrl = "Packages()/?`$filter=Id eq '$Package' and Version gt '$lower' and Version lt '$upper'&`$select=Version&`$orderby=Version desc"
        
        $packageResponse = Invoke-WebRequest -Uri "$NugetServerUrl/$searchUrl" -UseBasicParsing
        
        $xDoc = [System.Xml.Linq.XDocument]::Parse($packageResponse.Content);
        
        $ns = [System.Xml.Linq.XNamespace]::Get("http://www.w3.org/2005/Atom")
        $m = [System.Xml.Linq.XNamespace]::Get("http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
        $d = [System.Xml.Linq.XNamespace]::Get("http://schemas.microsoft.com/ado/2007/08/dataservices")
        $versions = $xDoc.Descendants($ns + "entry") | Select-Object @{Name = "Uri"; Expression = {$_.Element($ns + "content").Attribute("src").Value}}, @{Name = "Version"; Expression = {$_.Element($m + "properties").Element($d + "Version").Value}} | 
            Where-Object Version -Like "$Major.$Minor*"
        
        if ( (-Not $versions.Count -And $versions) -Or ($versions.Count -gt 0) ) {
        
            $tempFile = [System.IO.Path]::GetTempFileName();
            $tempPath = [System.IO.Path]::GetTempPath() + [System.Guid]::NewGuid().ToString("N");
        
            if (-Not $versions.Count) {
                $latest = $versions;
            }
            else {
                $latest = $versions[0];
            }
            Invoke-WebRequest -Uri $latest.Uri -OutFile "$tempFile.zip"
            Unzip "$tempFile.zip" "$tempPath"
            Remove-Item -Path "$tempFile.zip"
        
            $versionInfos = Get-ChildItem -Path "$tempPath\lib\" -Filter "$Package.dll" -Recurse | Select-Object -ExpandProperty VersionInfo -First 0 -Last 1
        
            $fileVersion = [version]$versionInfos.FileVersion
        
            $result.Build = $fileVersion.Build + 1
        
            $splitted = $latest.Version.Split(".")
            if ($splitted[2] -Like "*-*") {
                $result.Release = [int]::Parse($splitted[2].SubString(0, $splitted[2].IndexOf("-")))
            }
            else {
                $result.Release = [int]::Parse($splitted[2]) + 1
            }

            Get-ChildItem -Path $tempPath -Recurse | Remove-Item -force -recurse
            Remove-Item $tempPath -Force 
        }
        
        if ([string]::IsNullOrWhiteSpace($Prerelease)) {
            $result.NugetVersion = "$($result.Major).$($result.Minor).$($result.Release)";
        }
        else {
            $result.NugetVersion = "$($result.Major).$($result.Minor).$($result.Release)-$Prerelease-$($result.Build.ToString("D5"))"
        }

        # Return the results of the build.
        return $result
    }
}

Export-ModuleMember -Function Get-NugetVersionInfo
Export-ModuleMember -Function New-NugetPackages