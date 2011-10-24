:: Usage:
:: build 1.0.2

:: setting point version on the commandline determines the name of the output folder.
set msbuild=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe
set PointVersion=1.0.0

set outpathprop=/property:OutputPath=..\release\%PointVersion%\3.5
%msbuild% /property:Configuration=Debug /property:TargetFrameworkVersion=v3.5 %outpathprop%\CRM4\Debug /p:DefineConstants="CRM4" CrmQuery\CrmQuery.csproj
%msbuild% /property:Configuration=Release /property:TargetFrameworkVersion=v3.5 %outpathprop%\CRM4\Release /p:DefineConstants="CRM4" CrmQuery\CrmQuery.csproj

:: note that we can't build 2011 against the 3.5 framework.
:: %msbuild% /property:Configuration=Debug /property:TargetFrameworkVersion=v3.5 %outpathprop%\CRM2011\Debug CrmQuery\CrmQuery2011.csproj
:: %msbuild% /property:Configuration=Release /property:TargetFrameworkVersion=v3.5 %outpathprop%\CRM2011\Release CrmQuery\CrmQuery2011.csproj

set outpathprop=/property:OutputPath=..\release\%PointVersion%\4.0
%msbuild% /property:Configuration=Debug /property:TargetFrameworkVersion=v4.0 %outpathprop%\CRM4\Debug /p:DefineConstants="CRM4" CrmQuery\CrmQuery.csproj
%msbuild% /property:Configuration=Release /property:TargetFrameworkVersion=v4.0 %outpathprop%\CRM4\Release /p:DefineConstants="CRM4" CrmQuery\CrmQuery.csproj
%msbuild% /property:Configuration=Debug /property:TargetFrameworkVersion=v4.0 %outpathprop%\CRM2011\Debug CrmQuery\CrmQuery2011.csproj
%msbuild% /property:Configuration=Release /property:TargetFrameworkVersion=v4.0 %outpathprop%\CRM2011\Release CrmQuery\CrmQuery2011.csproj