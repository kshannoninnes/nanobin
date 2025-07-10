// Intercept Ctrl-A and apply it only to the paste text segment
export function handlePasteSelection() {
    document.addEventListener('keydown', function(e) {
        if ((e.ctrlKey || e.metaKey) && e.key === 'a') {
            const codeElement = document.querySelector('.highlighted-code');
            if (codeElement) {
                e.preventDefault();

                const range = document.createRange();
                range.selectNodeContents(codeElement);

                const selection = window.getSelection();
                selection.removeAllRanges();
                selection.addRange(range);
            }
        }
    });
}
