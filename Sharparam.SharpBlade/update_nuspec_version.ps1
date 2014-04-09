$assemblyinfo_path = 'Properties/AssemblyInfo.cs'
$nuspec_path = 'Sharparam.SharpBlade.nuspec'
$version_regex = 'AssemblyVersion\("(\d+\.\d+\.\d+)"\)'
$nuspec_regex = '<version>\$version\$</version>'
$version_attribute = (Get-Content $assemblyinfo_path) -match $version_regex
$version_attribute[0] -match $version_regex
$version = $matches[1]
(Get-Content $nuspec_path) -replace $nuspec_regex, "<version>$($version).%env.BUILD_NUMBER%</version>" | Out-File $nuspec_path
