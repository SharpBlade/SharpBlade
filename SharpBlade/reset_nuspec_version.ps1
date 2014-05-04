$nuspec_path = 'SharpBlade.nuspec'
(Get-Content $nuspec_path) -replace '<version>[\w\d\.\-]+</version>', '<version>$version$</version>' | Out-File $nuspec_path
