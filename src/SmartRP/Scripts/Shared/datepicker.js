jQuery(function ($) {
	// format date
	var currentTime = new Date();
	//currentTime = currentTime.setMonth(date.getMonth() - 1);
	var currentMonth = currentTime.getMonth() - 1;
	var currentDate = currentTime.getDate();
	var currentYear = currentTime.getFullYear();

	//bootstrap datetime picker
	$('div.datetimepicker').datetimepicker({
		format: 'DD/MM/YYYY'

	});


	$.validator.methods.date = function (value, element) {
		return this.optional(element) || moment(value, "DD/MM/YYYY", true).isValid();
	}


	//for jQuery datepicker
	//$('input[type=datetime]').datepicker({
	//	dateFormat: "dd/mm/yy",
	//	changeMonth: true,
	//	changeYear: true,
	//	showStatus: true,
	//	showWeeks: true,
	//	currentText: 'Now',
	//	autoSize: true,
	//	gotoCurrent: true,
	//	minDate: new Date(currentYear, currentMonth, currentDate)
	//});

});