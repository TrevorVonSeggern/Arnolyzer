﻿param($installPath, $toolsPath, $package, $project)

$analyzersPaths = Join-Path (Join-Path (Split-Path -Path $toolsPath -Parent) "analyzers\dotnet" ) * -Resolve

if($project.Type -eq "C#")
{
	foreach($analyzersPath in $analyzersPaths)
	{
		foreach ($analyzerFilePath in Get-ChildItem $analyzersPath -Filter *.dll)
		{
			if($project.Object.AnalyzerReferences)
			{
				$project.Object.AnalyzerReferences.Add($analyzerFilePath.FullName)
			}
		}
	}
}

