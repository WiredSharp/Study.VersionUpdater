$Version = "1.0"

Add-Type -Path "$PSScriptRoot\SemanticVersionUpdater.exe"

$semverRegex = '(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(?<release>(-|#)\w*|)'
$assemblyRegex = '\[assembly: assemblytitle\s*\(\s*"(?<assembly>[^"])"'
$informationVersionRegex = '(?<prefix>AssemblyInformationalVersion\(\s*")(?<version>' + $semverRegex +')(?<suffix>")'

function read-AssemblyFile([string]$assemblyFileContent) {

	if ($assemblyFileContent -match $informationVersionRegex) {
		$Matches.major
		$Matches.minor
	}
	else {
		"1","0"
	}

	if ($assemblyFileContent -match $assemblyRegex){
		$Matches.assembly
	}
	else {
		""
	}
}
<#
	.SUMMARY
	Update assembly informationalversion attribute in
#>
function Update-AssemblyFile {
	Param(
		[Parameter(Mandatory=$true)][string]$assemblyFile
		,[Parameter(Mandatory=$false)][string]$package
	)
	$content = Get-Content $assemblyFile
	$major, $minor, $assembly = read-AssemblyFile $content
	if (!$package) {
		$package = $assembly
	}
}