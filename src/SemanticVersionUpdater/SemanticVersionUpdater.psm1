[CmdletBinding(SupportsShouldProcess)]
Param(
	[Parameter]$Repository
	,[Parameter]$Project
	,[Parameter]$AssemblyInfo
	,[Parameter]$Major
	,[Parameter]$Minor
)

function Read-Version($file)
{
    $semverRegex = '';
    $regex = 'assemblyinformationalversion\(\s*"(?<version>(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+))"';
	$matches = [regex]::match((Get-Content $file),$regex, "IgnoreCase")
	$matches.Groups["major"].Value
	$matches.Groups["minor"].Value
	$matches.Groups["patch"].Value
}

function Get-NextVersion {
	if (!$Major) {
		if (!$AssemblyInfo){
			$AssemblyInfo = "${PSScriptRoot}\Properties\AssemblyInfo.cs"
		}
		$Major, $Minor, $patch = Read-Version $AssemblyInfo
	}

	$assembly = [Reflection.Assembly]::LoadFile("$PSScriptRoot\SemanticVersioning.Core.dll")
	$instance = New-Object -TypeName SemanticVersioning.VersionGenerator -ArgumentList $Repository

	$nextPatch = $instance.GetNextPatchAsync($Project, $Major, $Minor).GetAwaiter().GetResult()

	"$major.$minor.$nextPatch"
}