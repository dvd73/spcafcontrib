// Ensure that current page ribbon set to Browse tab
$(document).ready(function () {
	var InitialTabId = $.query.get('InitialTabId');
	if (!InitialTabId || InitialTabId != "Ribbon.Read")
		$(location).attr('href', $.query.set("InitialTabId", "Ribbon.Read").toString());
});

	