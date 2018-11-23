param($rootPath, $toolsPath, $package, $project)

 
# Find MvcVersion
$MvcVersion = ""
$project.Object.References | Where-Object { $_.Name -eq 'System.Web.Mvc' } | ForEach-Object { $MvcVersion = $_.Version}
Write-Host "MVC version: " $MvcVersion
 

# Remove unnecessary version assemblies from project references
if ($MvcVersion.StartsWith("2.")) {
    $project.Object.References | Where-Object { ($_.Name.StartsWith("CLK.Web.Mvc")) -and !($_.Name.StartsWith("CLK.Web.Mvc2")) } | ForEach-Object { 
        $_.Remove()
    }
}
elseif ($MvcVersion.StartsWith("3.")) {
    $project.Object.References | Where-Object { ($_.Name.StartsWith("CLK.Web.Mvc")) -and !($_.Name.StartsWith("CLK.Web.Mvc3")) } | ForEach-Object {
        $_.Remove()
    }
}
elseif ($MvcVersion.StartsWith("4.")) {
    $project.Object.References | Where-Object { ($_.Name.StartsWith("CLK.Web.Mvc")) -and !($_.Name.StartsWith("CLK.Web.Mvc4")) } | ForEach-Object { 
        $_.Remove()
    }
}
elseif ($MvcVersion.StartsWith("5.")) {
    $project.Object.References | Where-Object { ($_.Name.StartsWith("CLK.Web.Mvc")) -and !($_.Name.StartsWith("CLK.Web.Mvc5")) } | ForEach-Object { 
        $_.Remove()
    }
}
elseif ($MvcVersion.StartsWith("6.")) {
    $project.Object.References | Where-Object { ($_.Name.StartsWith("CLK.Web.Mvc")) -and !($_.Name.StartsWith("CLK.Web.Mvc6")) } | ForEach-Object { 
        $_.Remove()
    }
}