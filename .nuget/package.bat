rem Package the library for Nuget
copy ..\MaxFactry.Core-NF-2.0\bin\Release\*.dll lib\net20\
copy ..\MaxFactry.Core-NF-4.5.2\bin\Release\*.dll lib\net452\
copy ..\MaxFactry.Core-NF-4.7.2\bin\Release\*.dll lib\net472\
copy ..\MaxFactry.Core-NF-4.8\bin\Release\*.dll lib\net48\
copy ..\MaxFactry.Core-NC-2.1\bin\Release\netcoreapp2.1\*.dll lib\netcoreapp2.1\

c:\install\nuget\nuget.exe pack MaxFactry.Core.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 