// localization - more like translation, these are used within the CultureSelector
function setLanguageCookie(language) {
    document.cookie = `UserLanguage=${language};path=/;`;
}

function getLanguageCookie() {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; UserLanguage=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}
// --------------------------------------