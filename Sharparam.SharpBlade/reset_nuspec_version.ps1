$nuspec_path = 'Sharparam.SharpBlade.nuspec'
(Get-Content $nuspec_path) -replace '<version>[\d\.]+</version>', '<version>$version$</version>' | Out-File $nuspec_path
