set mypath=%cd%

dotnet script %mypath%\PocosGenerator.csx -- output:CRMMobile.cs namespace:com.apthai.CRMMobile.Model.CRMMobile config:..\appsettings.json connectionstring:ConnectionStrings:DefaultConnection dapper:true
