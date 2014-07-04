// Ribbon будет скрыт, только если страница запущена в модальном диалоге, и только если она находится в режиме просмотра. В режиме редактирования лента будет отображаться.
	
	(function () {
		
		if (GetUrlKeyValue("IsDlg") == "1" && document.forms[MSOWebPartPageFormName].MSOSPWebPartManager_DisplayModeName.value != "Edit")
		{
			var notificationArea = $get('notificationArea');
			var ribbonRow = $get('s4-ribbonrow');
			ribbonRow.removeChild(notificationArea);
			ribbonRow.parentNode.insertBefore(notificationArea, ribbonRow);
			ribbonRow.style.display = 'none';
		}

	})();
