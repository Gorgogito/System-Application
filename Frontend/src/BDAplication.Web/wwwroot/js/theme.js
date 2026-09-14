// Persistencia rápida del tema (cookie) — evita el parpadeo de tema por defecto
// en la primera carga, ya que la cookie viaja en el request HTTP inicial y puede
// leerse server-side antes de renderizar nada (a diferencia de localStorage, que
// solo está disponible tras conectar el circuito de Blazor Server).
window.appTheme = {
    setCookie: function (mode) {
        document.cookie = `theme_mode=${mode}; path=/; max-age=31536000; SameSite=Lax`;
    },
    prefersDark: function () {
        return !!(window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches);
    }
};
