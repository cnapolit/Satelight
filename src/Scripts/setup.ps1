$folders = "Functions", "Classes", "Models", "Extensions"
$dir = (Get-Location).Path
$basePath = "$dir\Common\Utility"
function Create-Link($folderName) {
    $path = "$basePath.Core\$folderName"
    $target = "$basePath\" + $folderName
    if (Test-Path $path) {
        return
    }
    New-Item -ItemType SymbolicLink -Path $path -Target $target
}

foreach($folder in $folders) {
    Create-Link $folder
}