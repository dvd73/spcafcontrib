
# public override void Visit(AssemblyFileReference target, NotificationCollection notifications)
 function Visit($__this, $target, $notifications) {

	$compamyName = "SPCAFContrib";
	$assemblyName = $target.AssemblyDefinition.Name.Name;

	if($assemblyName.StartsWith($compamyName) -eq $false) {
		$__this.PsNotify($target, "PS Host Rule: Assembly should be named starting with the company name:[$compamyName]. Current assembly name:[$assemblyName]", $notifications);
	}
}