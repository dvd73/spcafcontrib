
# public override void Visit(FeatureDefinition target, NotificationCollection notifications)
function Visit($__this, $target, $notifications) {

	if ([string]::IsNullOrEmpty($target.ImageUrl) -eq $true)
    {
	    $message = [string]::Format("PS Host Rule: Feature '{0}' should specify the ImageUrl attribute and reference a custom feature image.", $target.FeatureName);
		$__this.PsNotify($target, $message, $notifications);
    }
}