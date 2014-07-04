# The script copies the project DLL to the SPCAF Installation folder, if any

# There are a few return codes:
# 42 - can't copy one of the *.dll to the SPCAF folders

# More details might be found here:
# - PowerShell Script in PostBuild" 
#	http://stackoverflow.com/questions/813003/powershell-script-in-postbuild

param($SolutionDir, $ProjectDir, $ConfigurationName, $TargetDir, $TargetFileName)

Write-Output "SolutionDir:[$SolutionDir]"
Write-Output "ProjectDir:[$ProjectDir]"

$targetPackages = @()
$targetPackages += [System.IO.Path]::Combine($ProjectDir, "bin\$ConfigurationName\$TargetFileName")

function DeployPackagesToFolder($targetFolder, $packages) {

	Write-Output "Perform module copy..."
	Write-Output "	Target folder:[$targetFolder]"

	if([System.IO.Directory]::Exists($targetFolder) -eq $true)
	{
		foreach($package in $packages) {
			Write-Output "		Source package:[$package]"
			try 
			{
				Copy-Item -path $package -dest $targetFolder –errorvariable $errors
			}
			catch 
			{
				[Environment]::Exit(-42)
			}
		}
	} 
}

function CheckUniqueCheckId($packages) {

	#	PS 3.0 is required, so need to cjeck under win7/8/2012

	foreach($package in $packages) {
		#$asm = [System.Reflection.Assembly]::LoadFile($package)
		#$types = $asm.GetTypes() 

		#foreach ($type in $types)
        #{
        #    if ($type.IsSubClassOf("Rule") -or $type.IsSubClassOf("Metric"))
        #    {
        #        Write $type.FullName
        #    }
        #}
	}
}

CheckUniqueCheckId $targetPackages

DeployPackagesToFolder "C:\Program Files (x86)\SPCAF" $targetPackages
DeployPackagesToFolder "C:\Program Files (x86)\SPCopCE" $targetPackages
DeployPackagesToFolder "C:\Temp\SPCAFDebug\" $targetPackages
DeployPackagesToFolder ([System.IO.Path]::Combine($SolutionDir, "..\SPCAFDebug\"))  $targetPackages

$targetRuleSets = @()

$targetRuleSets += [System.IO.Path]::Combine($SolutionDir, "SPCAFContrib.RuleSets\SPCAFContrib.Experimental.spruleset")
$targetRuleSets += [System.IO.Path]::Combine($SolutionDir, "SPCAFContrib.RuleSets\SPCAFContrib.spruleset")
$targetRuleSets += [System.IO.Path]::Combine($SolutionDir, "SPCAFContrib.RuleSets\Community.spruleset")

DeployPackagesToFolder "C:\Program Files (x86)\SPCAF\RuleSets" $targetRuleSets
DeployPackagesToFolder "C:\Program Files (x86)\SPCopCE\RuleSets" $targetRuleSets
DeployPackagesToFolder ([System.IO.Path]::Combine($SolutionDir, "..\SPCAFDebug\RuleSets")) $targetRuleSets

[Environment]::Exit(0)
