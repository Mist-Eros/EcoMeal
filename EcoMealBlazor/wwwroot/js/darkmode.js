window.applyTheme = (isDark) => {
    document.documentElement.setAttribute('data-theme', isDark ? 'dark' : 'light');
};

window.getCurrentTheme = () => {
    return document.documentElement.getAttribute('data-theme') || 'light';
};