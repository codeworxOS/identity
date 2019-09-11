nuget push ..\dist\nuget\*.snupkg %MYGETAPIKEY% -Source https://www.myget.org/F/codeworx/api/v2/package

del ..\dist\nuget\*.snupkg

nuget push ..\dist\nuget\*.nupkg %NUGETAPIKEY% -Source https://www.nuget.org/api/v2/package

del ..\dist\nuget\*.nupkg