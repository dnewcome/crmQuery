:: Usage:
:: build 1.0.2

:: setting point version on the commandline determines the name of the output folder.
set msbuild=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe
set PointVersion=1.0.1

set outpathprop=/property:OutputPath=..\release\%PointVersion%\3.5
%msbuild% /property:Configuration=Debug /property:TargetFrameworkVersion=v3.5 /t:rebuild %outpathprop%\CRM4\Debug CrmQuery\CrmQuery.csproj
%msbuild% /property:Configuration=Release /property:TargetFrameworkVersion=v3.5 /t:rebuild %outpathprop%\CRM4\Release CrmQuery\CrmQuery.csproj

:: note that we can't build 2011 against the 3.5 framework.
:: %msbuild% /property:Configuration=Debug /property:TargetFrameworkVersion=v3.5 %outpathprop%\CRM2011\Debug CrmQuery\CrmQuery2011.csproj
:: %msbuild% /property:Configuration=Release /property:TargetFrameworkVersion=v3.5 %outpathprop%\CRM2011\Release CrmQuery\CrmQuery2011.csproj

set outpathprop=/property:OutputPath=..\release\%PointVersion%\4.0
%msbuild% /property:Configuration=Debug /property:TargetFrameworkVersion=v4.0 /t:rebuild %outpathprop%\CRM4\Debug CrmQuery\CrmQuery.csproj
%msbuild% /property:Configuration=Release /property:TargetFrameworkVersion=v4.0 /t:rebuild %outpathprop%\CRM4\Release CrmQuery\CrmQuery.csproj
%msbuild% /property:Configuration=Debug /property:TargetFrameworkVersion=v4.0 /t:rebuild %outpathprop%\CRM2011\Debug CrmQuery\CrmQuery2011.csproj
%msbuild% /property:Configuration=Release /property:TargetFrameworkVersion=v4.0 /t:rebuild %outpathprop%\CRM2011\Release CrmQuery\CrmQuery2011.csproj