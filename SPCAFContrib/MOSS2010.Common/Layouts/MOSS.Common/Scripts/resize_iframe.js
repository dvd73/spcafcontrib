//SP.UI.ModalDialog.showModalDialog seems to set the size of the window late on in the client page's lifecycle.
//This means that if you try to set the dialog dimensions using the function above via a $(document).ready() for instance the resulting size will simply be overridden by SharePoint. 
//If you're using this code to respond to user events after the initial load process it should work fine.
function resizeCommonModalDialog(width, height) {

    // get jQuery objects for relevant parts of the dialog
    var dialogElements = new Array();
    var getDialogElement = function (elementArray, elementRef) {
        elementArray[elementRef] = $('.ms-dlg' + elementRef, window.parent.document);
    };
    getDialogElement(dialogElements, "Border");
    getDialogElement(dialogElements, "Title");
    getDialogElement(dialogElements, "TitleText");
    getDialogElement(dialogElements, "Content");
    getDialogElement(dialogElements, "Frame");

    // calculate width & height delta
    deltaWidth = width - dialogElements["Border"].width();
    deltaHeight = height - dialogElements["Border"].height();

    for (var key in dialogElements) {
        // set the width
        dialogElements[key].width(dialogElements[key].width() + deltaWidth);

        // set the height, excluding title elements
        if (key != "Title" && key != "TitleText") {
            dialogElements[key].height(dialogElements[key].height() + deltaHeight);
        }
    }
}