
		function SuggestedKeywordCheckbox() {
			var checkBox = document.getElementById("inputKeyword");
			var keywords = document.getElementById("suggested-keyword");
			if (checkBox.checked == true) {
				keywords.style.display = "block";
			} else {
				keywords.style.display = "none";
			}
		}
