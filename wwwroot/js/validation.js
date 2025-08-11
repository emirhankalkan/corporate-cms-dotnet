/**
 * Form validation.js
 * Form doğrulama işlemleri için özel JavaScript dosyası
 */

document.addEventListener('DOMContentLoaded', function() {
    'use strict';

    // Tüm form validation için gerekli işlemler
    const forms = document.querySelectorAll('.needs-validation');

    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }

            form.classList.add('was-validated');
        }, false);

        // Input validation on blur
        const inputs = form.querySelectorAll('input, textarea, select');
        inputs.forEach(input => {
            input.addEventListener('blur', function() {
                if (this.checkValidity()) {
                    this.classList.add('is-valid');
                    this.classList.remove('is-invalid');
                } else {
                    this.classList.add('is-invalid');
                    this.classList.remove('is-valid');
                }
            });
        });
    });

    // Şifre doğrulama kuralları
    const passwordInputs = document.querySelectorAll('input[type="password"]');
    passwordInputs.forEach(input => {
        input.addEventListener('input', function() {
            validatePassword(this);
        });
    });

    // Şifre doğrulama yardımcı fonksiyonu
    function validatePassword(passwordInput) {
        const minLength = 8;
        const hasUpperCase = /[A-Z]/.test(passwordInput.value);
        const hasLowerCase = /[a-z]/.test(passwordInput.value);
        const hasNumbers = /\d/.test(passwordInput.value);

        // Minimum uzunluk kontrolü
        if (passwordInput.value.length < minLength) {
            passwordInput.setCustomValidity(`Şifre en az ${minLength} karakter olmalıdır`);
            return;
        }

        // Karmaşıklık kontrolü
        if (!(hasUpperCase && hasLowerCase && hasNumbers)) {
            passwordInput.setCustomValidity('Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir');
            return;
        }

        // Tüm kontroller geçildi
        passwordInput.setCustomValidity('');
    }

    // Şifre eşleşme kontrolü
    const passwordConfirmations = document.querySelectorAll('[data-match-password]');
    passwordConfirmations.forEach(confirmation => {
        const originalId = confirmation.getAttribute('data-match-password');
        const originalInput = document.getElementById(originalId);

        if (originalInput) {
            confirmation.addEventListener('input', function() {
                if (this.value !== originalInput.value) {
                    this.setCustomValidity('Şifreler eşleşmiyor');
                } else {
                    this.setCustomValidity('');
                }
            });

            originalInput.addEventListener('input', function() {
                if (confirmation.value !== this.value && confirmation.value !== '') {
                    confirmation.setCustomValidity('Şifreler eşleşmiyor');
                } else {
                    confirmation.setCustomValidity('');
                }
            });
        }
    });

    // Email doğrulama
    const emailInputs = document.querySelectorAll('input[type="email"]');
    emailInputs.forEach(input => {
        input.addEventListener('blur', function() {
            const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

            if (!emailRegex.test(this.value) && this.value !== '') {
                this.setCustomValidity('Geçerli bir e-posta adresi giriniz');
            } else {
                this.setCustomValidity('');
            }
        });
    });

    // Tarih doğrulama
    const dateInputs = document.querySelectorAll('input[type="date"]');
    dateInputs.forEach(input => {
        const minDate = input.getAttribute('min');
        const maxDate = input.getAttribute('max');

        input.addEventListener('change', function() {
            const selectedDate = new Date(this.value);

            if (minDate && new Date(minDate) > selectedDate) {
                this.setCustomValidity(`Tarih ${minDate} tarihinden sonra olmalıdır`);
            } else if (maxDate && new Date(maxDate) < selectedDate) {
                this.setCustomValidity(`Tarih ${maxDate} tarihinden önce olmalıdır`);
            } else {
                this.setCustomValidity('');
            }
        });
    });

});