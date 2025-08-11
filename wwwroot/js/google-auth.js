// Google Authentication Enhanced Experience
document.addEventListener('DOMContentLoaded', () => {
    // Track authentication state
    const authState = {
        inProgress: false
    };

    // Initialize Google Auth buttons with enhanced UX
    initGoogleAuthButtons();

    // Get URL parameters (for handling auth callbacks)
    const urlParams = new URLSearchParams(window.location.search);
    const authError = urlParams.get('authError');

    // Handle any auth errors coming back from the server
    if (authError) {
        showAuthErrorNotification(authError);
    }

    /**
     * Enhanced Google Auth Button Initialization
     * - Adds loading state
     * - Improves accessibility
     * - Prevents double-clicks
     */
    function initGoogleAuthButtons() {
        const googleButtons = document.querySelectorAll('.google-auth-btn');

        googleButtons.forEach(button => {
            // Enhance accessibility
            button.setAttribute('role', 'button');
            button.setAttribute('aria-label', 'Google ile oturum aç');

            button.addEventListener('click', handleGoogleAuthClick);
        });
    }

    /**
     * Handle Google Authentication Button Click
     */
    function handleGoogleAuthClick(e) {
        e.preventDefault();

        // Prevent double-clicks
        if (authState.inProgress) {
            return;
        }

        // Get the button and update its state
        const button = e.currentTarget;

        // Set auth in progress
        authState.inProgress = true;

        // Show loading state
        button.innerHTML = '<i class="fas fa-circle-notch fa-spin text-primary"></i> Bağlanıyor...';
        button.classList.add('disabled');

        // Small delay to show the loading state (better UX)
        setTimeout(() => {
            // Redirect to the authentication endpoint
            window.location.href = '/Account/ExternalLogin?provider=Google&returnUrl=' +
                encodeURIComponent(window.location.pathname);
        }, 500);
    }

    /**
     * Show authentication error notification
     */
    function showAuthErrorNotification(error) {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = 'alert alert-danger auth-alert';
        notification.innerHTML = `
            <i class="fas fa-exclamation-circle me-2"></i> 
            <span>Google ile giriş sırasında bir hata oluştu: ${error}</span>
            <button type="button" class="btn-close" aria-label="Close"></button>
        `;

        // Add notification to the page
        const container = document.querySelector('.auth-container');
        container.insertBefore(notification, container.firstChild);

        // Handle close button
        const closeButton = notification.querySelector('.btn-close');
        closeButton.addEventListener('click', () => {
            notification.remove();
        });

        // Auto-dismiss after 8 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.classList.add('fade-out');
                setTimeout(() => notification.remove(), 500);
            }
        }, 8000);
    }
});

// Add CSS for the auth alert
const style = document.createElement('style');
style.textContent = `
.auth-alert {
    position: fixed;
    top: 20px;
    left: 50%;
    transform: translateX(-50%);
    z-index: 1000;
    min-width: 300px;
    box-shadow: 0 3px 10px rgba(0, 0, 0, 0.15);
    border-left: 5px solid #dc3545;
    animation: slideInDown 0.4s;
}

.fade-out {
    opacity: 0;
    transition: opacity 0.5s ease;
}

@keyframes slideInDown {
    from {
        transform: translate(-50%, -20px);
        opacity: 0;
    }
    to {
        transform: translate(-50%, 0);
        opacity: 1;
    }
}
`;
document.head.appendChild(style);