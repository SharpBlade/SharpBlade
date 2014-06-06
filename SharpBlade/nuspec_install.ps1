param($installPath, $toolsPath, $package, $project)

$projectItems = $project.ProjectItems
$tpBlank = $projectItems.Item("Default\Images\tp_blank.png")
$tpBlank.Properties.Item("BuildAction").Value = 0 # BuildAction = None
$tpBlank.Properties.Item("CopyToOutputDirectory").Value = 2 # CopyToOutputDirectory = Copy if newer
$dkBlank = $projectItems.Item("Default\Images\dk_blank.png")
$dkBlank.Properties.Item("BuildAction").Value = 0 # BuildAction = None
$dkBlank.Properties.Item("CopyToOutputDirectory").Value = 2 # CopyToOutputDirectory = Copy if newer
