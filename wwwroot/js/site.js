// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Debug: Bootstrap ve jQuery yüklenip yüklenmediğini kontrol et
document.addEventListener('DOMContentLoaded', function() {
    console.log('DOM yüklendi');
    console.log('jQuery:', typeof $ !== 'undefined' ? 'Yüklendi' : 'Yüklenmedi');
    console.log('Bootstrap:', typeof bootstrap !== 'undefined' ? 'Yüklendi' : 'Yüklenmedi');

    // Buton click eventlerini kontrol et
    document.querySelectorAll('.btn').forEach(function(btn) {
        btn.addEventListener('click', function(e) {
            console.log('Buton tıklandı:', this);
            console.log('href:', this.getAttribute('href'));
            console.log('asp-action:', this.getAttribute('asp-action'));
        });
    });
});