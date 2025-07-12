/**
 * Decrypts and displays the paste content
 * @param content
 * @returns {Promise<void>}
 */
export async function displayContent(content) {
    handlePasteSelection();
    document.getElementById('decrypted-content').textContent = await decryptContent(content);
}

/**
 * Performs syntax highlighting on the decrypted content
 * @returns {void}
 */
export function applySyntaxHighlighting() {
    hljs.highlightAll();
}

/**
 * TODO Remove this and implement a raw view instead
 * Intercepts Ctrl-A keypresses to select all the paste text only
 * @returns {void}
 */
function handlePasteSelection() {
    document.addEventListener('keydown', function(e) {
        if ((e.ctrlKey || e.metaKey) && e.key === 'a') {
            const codeElement = document.getElementById('decrypted-content');
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

/**
 * Decrypts a string using the key from the URL fragment
 * @param {string} combinedData - The combined IV and encrypted data as a single base64 string
 * @returns {Promise<string>} - The decrypted text
 */
async function decryptContent(combinedData) {
    // Get the key from the URL fragment
    const keyBase64 = window.location.hash.substring(1); // Remove the '#' character
    if (!keyBase64) {
        throw new Error('No decryption key found in URL fragment');
    }

    // Import the key
    const keyData = base64ToArrayBuffer(keyBase64);
    const key = await window.crypto.subtle.importKey(
        'raw',
        keyData,
        { name: 'AES-GCM', length: 256 },
        false,
        ['decrypt']
    );

    // Decode the combined data
    const decodedData = base64ToArrayBuffer(combinedData);

    // Extract IV (first 12 bytes) and encrypted data
    const iv = new Uint8Array(decodedData.slice(0, 12));
    const encryptedData = new Uint8Array(decodedData.slice(12));

    // Decrypt the data
    const decryptedData = await window.crypto.subtle.decrypt(
        {
            name: 'AES-GCM',
            iv: iv
        },
        key,
        encryptedData
    );

    // Decode and render the decrypted data
    return new TextDecoder().decode(decryptedData);
}

/**
 * Converts a Base64 string to an ArrayBuffer
 * @param {string} base64 - The Base64 string to convert
 * @returns {ArrayBuffer} - The converted array buffer
 */
function base64ToArrayBuffer(base64) {
    const binaryString = atob(base64);
    const bytes = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes.buffer;
}
