// Public Site JS - Modernized
// Handles: navbar scroll state, mobile dropdown toggle, form validation, password min-length helper.

(function() {
    'use strict';

    document.addEventListener('DOMContentLoaded', function() {
        // Navbar (new class .navbar-app)
        const nav = document.querySelector('.navbar-app');

        function applyNavScroll() { if (!nav) return; if (window.scrollY > 10) { nav.classList.add('scrolled'); } else { nav.classList.remove('scrolled'); } }
        applyNavScroll();
        window.addEventListener('scroll', applyNavScroll, { passive: true });

        // Mobile dropdown (improved for small screens)
        document.querySelectorAll('.dropdown-toggle').forEach(trigger => {
            trigger.addEventListener('click', function(e) {
                if (window.innerWidth < 992) { // below lg
                    const submenu = this.nextElementSibling;
                    if (submenu && submenu.classList.contains('dropdown-menu')) {
                        e.preventDefault();
                        submenu.classList.toggle('show');
                    }
                }
            });
        });

        // Simple form validation enhancement
        document.querySelectorAll('form.needs-validation').forEach(form => {
            form.addEventListener('submit', function(e) {
                if (!form.checkValidity()) {
                    e.preventDefault();
                    e.stopPropagation();
                }
                form.classList.add('was-validated');
            });
        });

        // Password min length guidance (keep lightweight; strength handled elsewhere when needed)
        document.querySelectorAll('input[type="password"][data-minlength]').forEach(input => {
            const min = parseInt(input.getAttribute('data-minlength'), 10) || 8;
            input.addEventListener('input', function() {
                if (this.value.length && this.value.length < min) {
                    this.setCustomValidity(`En az ${min} karakter olmalı`);
                } else {
                    this.setCustomValidity('');
                }
            });
        });
    });
})();