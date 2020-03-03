set mypath=%cd%

dotnet script %mypath%\PocosGenerator.csx -- output:CRMWeb.cs namespace:com.apthai.CRMMobile.Model.CRMWeb config:..\appsettings.json connectionstring:ConnectionStrings:DefaultConnection dapper:true
