/* Russian initialisation for the timepicker plugin */
/* Written by Lowie Hulzinga. */
jQuery(function($){
    $.timepicker.regional['ru'] = {
                hourText: 'Часы',
                minuteText: 'Минуты',
                amPmText: ['AM', 'PM'] ,
                closeButtonText: 'Закрыть',
                nowButtonText: 'Тек время',
                deselectButtonText: 'Отмена' }
    $.timepicker.setDefaults($.timepicker.regional['ru']);
});
