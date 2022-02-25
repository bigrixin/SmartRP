jQuery.noConflict()(function ($) {
	$(document).ready(function () {
		$('#uploadfile').fileupload({
			dataType: 'json',
			url: $('#uploadURL').prop('value'),
			autoUpload: true,

			add: function (e, data) {

				var uploadErrors = [];
				var fileType = data.files[0].name.split('.').pop(), allowdtypes = 'doc, docx, pdf, ppt,pptx, jpg,png,DOC,DOCX, PPT, PPTX, PDF,JPG,PNG';
				if (allowdtypes.indexOf(fileType) < 0) {
					uploadErrors.push('Invalid file type, aborted');
				}
				//5000000 --> 5M
				if (data.originalFiles[0]['size'] > 5000000) {
					uploadErrors.push('File size is too big');
				}
				if (uploadErrors.length > 0) {
					alert(uploadErrors.join('\n'));
				} else {
					data.submit();
				}
			},
			start: function (e, data) {
				$('#uploadfileBar').css('width', '5%');
				$('#uploadfileBar').text('5% complete');
			},
			progress: function (e, data) {
				var progress = parseInt(data.loaded / data.total * 100, 10);

				if (progress < 80) {
					$('#uploadfileBar').css('width', progress + '%');
					$('#uploadfileBar').text(progress + '% complete');
				}
			},
			fail: function (event, data) {
				if (data.files[0].error) {
					var thisId = $(this).attr('id');
					var resultId = 'result' + thisId[thisId.length - 1];
					$('#' + resultId).html(data.files[0].error);
				}
			},
			done: function (e, data) {
				var thisId = $(this).attr('id');
				var resultId = 'result' + thisId[thisId.length - 1];
				var cancelId = 'cancelUpload' + thisId[thisId.length - 1];
				var cleanId = 'cleanUpload' + thisId[thisId.length - 1];
				var barId = 'bar' + thisId[thisId.length - 1];
				if (data.result.isUploaded) {

					$('#uploadfileBar').css('width', '100%');
					$('#uploadfileBar').text('100% complete');

					$('#uploadfile').attr("style", "display:none");

					$('#cancelUploadfile').attr("style", "display:block");
					$('#cleanUploadfile').attr("style", "display:block");
					$('#preview').attr("style", "display:block");
					//write in EditorFor
					var fileRecordId = 'fileRecord' + thisId[thisId.length - 1];

					$('#fileRecord').attr('value', data.result.file);
					setUrl(data.result.file);  // set url for preview

					$('#' + thisId).prop('disabled', true);

					$('#fileFileId').attr('src', data.result.file);
				}
				else {
					$('#uploadfileBar').css('width', '0%');
					$('#uploadfileBar').text('0% complete');
				}
			}
		});

		//check upload file status
		var fileName = $('#fileRecord').val();
		if (fileName === null || fileName.length === 0) {
			//display upload button
			$('#uploadfile').attr("style", "display:block");

			$('#uploadfileBar').css('width', '0%');
			$('#uploadfileBar').text('0% complete');
		}
		else {
			//display cancel and preview button
			$('#cancelUploadfile').attr("style", "display:block");
			$('#cleanUploadfile').attr("style", "display:block");
			$('#preview').attr("style", "display:block");
			//set file url
			setUrl($('#fileRecord').val());
			$('#uploadfileBar').css('width', '100%');
			$('#uploadfileBar').text('100% complete');
		}
	});

	//set preview url
	function setUrl(fileName) {
		var previewName = window.location.origin
				? window.location.origin + '/'
				: window.location.protocol + '/' + window.location.host;
		$('#preview').attr('href', previewName + fileName.substring(1));
	};

	//preview file 
	function readURL(input) {
		if (input.files && input.files[0]) {
			var reader = new FileReader();

			reader.onload = function (e) {
				$('#fileFileId').attr('src', e.target.result);
			};

			reader.readAsDataURL(input.files[0]);
		}
	}

	//clean file name and delete file
	$('#cancelUploadfile').click(function () {
		var fileName = $('#fileRecord').val();
		// delete file from server
		$.ajax({
			dataType: 'json',
			url: $('#deleteURL').prop('value'),
			type: 'DELETE',
			data: { 'fileName': fileName },
			success: function (result) {
				///	alert(result.message);
				$('#uploadfileBar').css('width', '0%');
				$('#uploadfileBar').text('');
				//clear file name
				$('#fileRecord').attr('value', '');

				//display upload button
				$('#uploadfile').attr("style", "display:block");
				$('#uploadfile').prop('disabled', false);

				//hiden cancel button
				$('#cancelUploadfile').attr("style", "display:none");
				$('#preview').attr("style", "display:none");

				$('#fileFileId').attr('src', '');
			},
			error: function () {
				alert('error');
			}
		});

	});

	//clean file name only
	$('#cleanUploadfile').click(function () {
		var fileName = $('#fileRecord').val();

		///	alert(result.message);
		$('#uploadfileBar').css('width', '0%');
		$('#uploadfileBar').text('');
		//clear file name
		$('#fileRecord').attr('value', '');

		//display upload button
		$('#uploadfile').attr("style", "display:block");
		$('#uploadfile').prop('disabled', false);

		//hiden cancel button
		$('#cleanUploadfile').attr("style", "display:none");
		$('#preview').attr("style", "display:none");

		$('#fileFileId').attr('src', '');

	});

});

//file type
function isImage(filename) {
	var ext = getExtension(filename);
	switch (ext.toLowerCase()) {
		case 'doc':
		case 'docx':
		case 'pdf':
			//etc
			return true;
	}
	return false;
}

function getExtension(filename) {
	var parts = filename.split('.');
	return parts[parts.length - 1];
}