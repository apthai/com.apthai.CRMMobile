set mypath=%cd%

dotnet script %mypath%\PocosGenerator.csx -- output:ModelsAuth.cs namespace:com.apthai.CRMMobile.Model.DefectAPISync config:..\appsettings.json connectionstring:ConnectionStrings:DefaultAuthorizeConnection dapper:true
