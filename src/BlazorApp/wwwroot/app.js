"use strict";

const setTheme = () => {
    document.documentElement.setAttribute(
        "data-bs-theme",
        window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light");
}

setTheme();

window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", () => {
    setTheme();
});

Blazor.addEventListener("enhancedload", () => {
    setTheme();
    window.scrollTo({ top: 0, left: 0, behavior: "instant" });
});
