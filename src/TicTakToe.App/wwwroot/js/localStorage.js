window.tttStorage = {
    getItem: (key) => localStorage.getItem(key),
    setItem: (key, value) => localStorage.setItem(key, value),
    removeItem: (key) => localStorage.removeItem(key),

    setTheme: function (theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('ttt_theme', theme);
    },

    toggleTheme: function () {
        const current = document.documentElement.getAttribute('data-theme') || 'light';
        tttStorage.setTheme(current === 'light' ? 'dark' : 'light');
    }
};

// Apply saved theme immediately on script load (before Blazor renders)
(function () {
    const saved = localStorage.getItem('ttt_theme');
    if (saved === 'light' || saved === 'dark') {
        document.documentElement.setAttribute('data-theme', saved);
    }
})();
