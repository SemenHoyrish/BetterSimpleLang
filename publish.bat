dotnet publish -c Release -r win10-x64 --self-contained true /p:PublishSingleFile=true
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true