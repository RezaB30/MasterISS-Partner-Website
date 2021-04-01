"use strict";

// Class definition
var KTWizard5 = function () {
	// Base elements
	var _wizardEl;
	var _formEl;
	var _wizardObj;
	var _validations = [];


	// Private functions
	var _initWizard = function () {
		// Initialize form wizard
		_wizardObj = new KTWizard(_wizardEl, {
			startStep: 1, // initial active step number
			clickableSteps: false  // allow step clicking
		});

		// Validation before going to next page
		_wizardObj.on('change', function (wizard) {
			if (wizard.getStep() > wizard.getNewStep()) {
				return; // Skip if stepped back
			}

			// Validate form before change wizard step
			var validator = _validations[wizard.getStep() - 1]; // get validator for currnt step

			if (validator) {
				validator.validate().then(function (status) {
					if (status == 'Valid') {
						wizard.goTo(wizard.getNewStep());

						KTUtil.scrollTop();
					} else {
						Swal.fire({
							text: "Hata Bulundu, Lütfen kontrol edin",
							icon: "error",
							buttonsStyling: false,
							confirmButtonText: "Tamam",
							customClass: {
								confirmButton: "btn font-weight-bold btn-light"
							}
						}).then(function () {
							KTUtil.scrollTop();
						});
					}
				});
			}

			return false;  // Do not change wizard step, further action will be handled by he validator
		});

		// Change event
		_wizardObj.on('changed', function (wizard) {
			KTUtil.scrollTop();
		});

		// Submit event
		_wizardObj.on('submit', function (wizard) {
			Swal.fire({
				text: "Herşey Tamam!, Başvurunuzu Onaylıyor musunuz?",
				icon: "success",
				showCancelButton: true,
				buttonsStyling: false,
				confirmButtonText: "Onaylıyorum",
				cancelButtonText: "İptal",
				customClass: {
					confirmButton: "btn font-weight-bold btn-primary",
					cancelButton: "btn font-weight-bold btn-default"
				}
			}).then(function (result) {
				if (result.value) {
					_formEl.submit(); // Submit form
				} else if (result.dismiss === 'cancel') {
					Swal.fire({
						text: "Form Gönderimi İptal Edildi",
						icon: "error",
						buttonsStyling: false,
						confirmButtonText: "Tamam",
						customClass: {
							confirmButton: "btn font-weight-bold btn-primary",
						}
					});
				}
			});
		});
	}

	var _initValidation = function () {
		// Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
		// Step 1
		_validations.push(FormValidation.formValidation(
			_formEl,
			{
				fields: {
					gsmno: {
						validators: {
							notEmpty: {
								message: 'GSM numaranızı yazınız'
							},
							
					}
				},
				      kvkk: {
						validators: {
							choice: {
								min:1,
								message: 'Onay veriniz'
								},
							
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					// Bootstrap Framework Integration
					bootstrap: new FormValidation.plugins.Bootstrap({
						//eleInvalidClass: '',
						eleValidClass: '',
					})
				}
			}
		));

		// Step 2
		_validations.push(FormValidation.formValidation(
			_formEl,
			{
				fields: {
					smscode: {
						validators: {
							notEmpty: {
								message: 'SMS Doğrulama Kodunuzu Yazınız'
							}
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					// Bootstrap Framework Integration
					bootstrap: new FormValidation.plugins.Bootstrap({
						//eleInvalidClass: '',
						eleValidClass: '',
					})
				}
			}
		));

		// Step 3
		_validations.push(FormValidation.formValidation(
			_formEl,
			{
				fields: {
					firstname: {
						validators: {
							notEmpty: {
								message: 'Adınızı Yazınız'
								
							}
						}
					},
					lastname: {
						validators: {
							notEmpty: {
								message: 'Soyadınızı Yazınız'
							}
						}
					},
					email: {
						validators: {
							notEmpty: {
								message: 'E-Posta Gerekli'
							},
							emailAddress: {
								message: 'Lütfen geçerli bir e-posta yazınız'
							}
						}
					
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					// Bootstrap Framework Integration
					bootstrap: new FormValidation.plugins.Bootstrap({
						//eleInvalidClass: '',
						eleValidClass: '',
					})
				}
			}
		));
						// Step 4
		_validations.push(FormValidation.formValidation(
			_formEl,
			{
				fields: {
					sehir: {
						validators: {
							notEmpty: {
								message: 'Şehir Seçiniz'
							}
						}
					},


					hizmetnosu: {
						validators: {
							stringLength: {
								min:10,
								max:10,
								message: 'Hizmet No 10 haneli olmalıdır'
							}
						}
					},

					ilce: {
						validators: {
							notEmpty: {
								message: 'İlçe Seçiniz'
							}
						}
					},
					semt: {
						validators: {
							notEmpty: {
								message: 'Semt Seçiniz'
							}
						}
					},
					mahalle: {
						validators: {
							notEmpty: {
								message: 'Mahalle Seçiniz'
							}
						}
					},
					sokak: {
						validators: {
							notEmpty: {
								message: 'Sokak veya Cadde Seçiniz'
							}
						}
					},
					kapino: {
						validators: {
							notEmpty: {
								message: 'Kapı No Seçiniz'
							}
						}
					},
					katno: {
						validators: {
							notEmpty: {
								message: 'Kat No Seçiniz'
							}
						}
					},
					postakodu: {
						validators: {
							notEmpty: {
								message: 'Posta Kodu Zorunludur'
							}
						}
					},
					aptbina: {
						validators: {
							notEmpty: {
								message: 'Bina No Seçiniz'
							}
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					// Bootstrap Framework Integration
					bootstrap: new FormValidation.plugins.Bootstrap({
						//eleInvalidClass: '',
						eleValidClass: '',
					})
				}
			}
		));

		// Step 3
		_validations.push(FormValidation.formValidation(
			_formEl,
			{
				fields: {
					xstatikip: {
						validators: {
							notEmpty: {
								message: 'Adınızı Yazınız'
								
							}
						}
					},
					xlastname: {
						validators: {
							notEmpty: {
								message: 'Soyadınızı Yazınız'
							}
						}
					},
					xemail: {
						validators: {
							notEmpty: {
								message: 'E-Posta Gerekli'
							},
							emailAddress: {
								message: 'Lütfen geçerli bir e-posta yazınız'
							}
						}
					},	
					xphone: {
						validators: {
							notEmpty: {
								message: 'Phone is required'
							}
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					// Bootstrap Framework Integration
					bootstrap: new FormValidation.plugins.Bootstrap({
						//eleInvalidClass: '',
						eleValidClass: '',
					})
				}
			}
		));

				// Step 4
		_validations.push(FormValidation.formValidation(
			_formEl,
			{
				fields: {
					tcno: {
						validators: {
							notEmpty: {
								message: 'T.C Zorunludur'
							}
						}
					
					},
					dogumyili: {
						validators: {
							notEmpty: {
								message: 'Doğum Yılı Zorunludur'
							}
						}
					
					},
					annekizliksoyad: {
						validators: {
							notEmpty: {
								message: 'Anne Kızlık Soyadı Zorunludur'
							}
						}
					
					},
					
					dogumyeri: {
						validators: {
							notEmpty: {
								message: 'Doğum Yeri Zorunludur'
							}
						}
					
					},
					
					cinsiyet: {
						validators: {
							notEmpty: {
								message: 'Cinsiyet Seçiniz'
							}
						}
					
					},
					
					babaadi: {
						validators: {
							notEmpty: {
								message: 'Baba Adı Zorunludur'
							}
						}
					
					},
					
					anneadi: {
						validators: {
							notEmpty: {
								message: 'Anne Adı Zorunludur'
							}
						}
					
					},
					
					dogumay: {
						validators: {
							notEmpty: {
								message: 'Doğum Ayı Zorunludur'
							}
						}
					
					},
					
					dogumgun: {
						validators: {
							notEmpty: {
								message: 'Doğum Günü Zorunludur'
							}
						}
					
					},
					

					xcountry: {
						validators: {
							notEmpty: {
								message: 'Country is required'
							}
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					// Bootstrap Framework Integration
					bootstrap: new FormValidation.plugins.Bootstrap({
						//eleInvalidClass: '',
						eleValidClass: '',
					})
				}
			}

		));

	}

	return {
		// public functions
		init: function () {
			_wizardEl = KTUtil.getById('kt_wizard');
			_formEl = KTUtil.getById('kt_form');

			_initWizard();
			_initValidation();
		}
	};
}();

jQuery(document).ready(function () {
	KTWizard5.init();
});
