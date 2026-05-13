// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
(function () {
    const originalFetch = window.fetch;
    window.fetch = async (...args) => {
        const response = await originalFetch(...args);

        if (response.redirected && response.url.includes('Login')) {
            window.location.href = response.url;
            return;
        }

        if (response.status === 401) {
            const loginPath = "/Accounts/Login?ReturnUrl=" + encodeURIComponent(window.location.pathname);
            window.location.href = loginPath;
            return;
        }

        return response;
    };
})();