$("#seeAnotherField").change(function() {
			if ($(this).val() == "yes") {
				$('#otherFieldDiv').show();
				$('#otherField').attr('required','');
				$('#otherField').attr('data-error', 'This field is required.');
			} else {
				$('#otherFieldDiv').hide();
				$('#otherField').removeAttr('required');
				$('#otherField').removeAttr('data-error');				
			}
		});
		$("#seeAnotherField").trigger("change");
		
$("#seeAnotherFieldGroup").change(function() {
			if ($(this).val() == "yes") {
				$('#otherFieldGroupDiv').show();
				$('#otherField1').attr('required','');
				$('#otherField1').attr('data-error', 'This field is required.');
        $('#otherField2').attr('required','');
				$('#otherField2').attr('data-error', 'This field is required.');
			} else {
				$('#otherFieldGroupDiv').hide();
				$('#otherField1').removeAttr('required');
				$('#otherField1').removeAttr('data-error');
        $('#otherField2').removeAttr('required');
				$('#otherField2').removeAttr('data-error');	
			}
		});
		$("#seeAnotherFieldGroup").trigger("change")
	

  // other functions


  $("#tseeAnotherField").change(function() {
			if ($(this).val() == "yes") {
				$('#totherFieldDiv').show();
				$('#totherField').attr('required','');
				$('#totherField').attr('data-error', 'This field is required.');
			} else {
				$('#totherFieldDiv').hide();
				$('#totherField').removeAttr('required');
				$('#totherField').removeAttr('data-error');				
			}
		});
		$("#tseeAnotherField").trigger("change");
		
$("#tseeAnotherFieldGroup").change(function() {
			if ($(this).val() == "yes") {
				$('#totherFieldGroupDiv').show();
				$('#totherField1').attr('required','');
				$('#totherField1').attr('data-error', 'This field is required.');
        $('#totherField2').attr('required','');
				$('#totherField2').attr('data-error', 'This field is required.');
			} else {
				$('#totherFieldGroupDiv').hide();
				$('#totherField1').removeAttr('required');
				$('#totherField1').removeAttr('data-error');
        $('#totherField2').removeAttr('required');
				$('#totherField2').removeAttr('data-error');	
			}
		});
		$("#tseeAnotherFieldGroup").trigger("change");
		