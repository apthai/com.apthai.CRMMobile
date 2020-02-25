set mypath=%cd%

dotnet script %mypath%\PocosGenerator.csx -- output:DefectModels.cs namespace:com.apthai.CRMMobile.Model.DefectAPI config:..\appsettings.json connectionstring:ConnectionStrings:DefaultConnection dapper:true
