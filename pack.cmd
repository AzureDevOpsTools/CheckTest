md package
msbuild checktest.sln /p:Configuration=Release /p:Platform="Any CPU"
nuget pack CheckTest.nuspec -o package -NoPackageAnalysis
