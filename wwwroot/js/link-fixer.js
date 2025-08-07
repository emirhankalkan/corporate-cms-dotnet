// Link Düzeltici - Admin Area için
(function() {
    'use strict';

    // Sayfadaki tüm linkleri düzelt
    function fixLinks() {
        const currentPath = window.location.pathname;

        // Admin area'da mıyız kontrol et
        if (currentPath.includes('/Admin/')) {
            const allLinks = document.querySelectorAll('a[href]');
            const buttons = document.querySelectorAll('.btn');

            // Tüm linkleri kontrol et
            allLinks.forEach(fixLink);

            // Butonları da kontrol et
            buttons.forEach(function(button) {
                const href = button.getAttribute('href');
                if (href) {
                    fixLink(button);
                }
            });
        }
    }

    // Link düzeltme fonksiyonu
    function fixLink(element) {
        const href = element.getAttribute('href');

        // Sadece göreceli linkleri düzelt
        if (href && !href.startsWith('/') && !href.startsWith('http') && !href.startsWith('#')) {
            // Admin area kontrolü
            if (!href.includes('/Admin/')) {
                // Controller/Action formatında mı?
                if (href.includes('/')) {
                    element.setAttribute('href', '/Admin' + href);
                    console.log('Düzeltildi:', href, ' => ', '/Admin' + href);
                } else {
                    element.setAttribute('href', '/Admin/' + href);
                    console.log('Düzeltildi:', href, ' => ', '/Admin/' + href);
                }
            }
        }
    }

    // Sayfa yüklenince çalıştır
    document.addEventListener('DOMContentLoaded', fixLinks);
})();