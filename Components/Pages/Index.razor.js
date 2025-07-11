/**
 * Encrypts a string and stores the key in the URL fragment
 * @param {string} text - The text to encrypt
 * @returns {Promise<string>} - Combined IV and encrypted data as a single base64 string
 */
export async function encryptContent(text) {
    // Generate a random encryption key
    const key = await generateEncryptionKey();

    // Export the key to raw format (to store in URL)
    const exportedKey = await window.crypto.subtle.exportKey('raw', key);

    // Store the key in URL fragment
    sessionStorage.setItem('key', arrayBufferToBase64(exportedKey));

    // Encrypt the text
    const iv = window.crypto.getRandomValues(new Uint8Array(12)); // 12 bytes IV for AES-GCM
    const encodedText = new TextEncoder().encode(text);

    const encryptedData = await window.crypto.subtle.encrypt(
        {
            name: 'AES-GCM',
            iv: iv
        },
        key,
        encodedText
    );

    // Combine IV and encrypted data
    const combinedData = new Uint8Array(iv.length + encryptedData.byteLength);
    combinedData.set(iv);
    combinedData.set(new Uint8Array(encryptedData), iv.length);

    // Convert to base64 for easier handling
    return arrayBufferToBase64(combinedData);
}

export function navigateToNewPaste(pasteID){
    const key = sessionStorage.getItem('key');
    sessionStorage.removeItem('key');
    
    if(!key){
        console.error('Encryption key not found in session storage');
        window.location.href = '/';
    }
    
    window.location.href = `/${pasteID}#${key}`;
}

/**
 * Generates a random AES-GCM encryption key
 * @returns {Promise<CryptoKey>} - The generated encryption key
 */
async function generateEncryptionKey() {
    return await window.crypto.subtle.generateKey(
        {
            name: 'AES-GCM',
            length: 256
        },
        true, // extractable
        ['encrypt', 'decrypt']
    );
}

/**
 * Converts an ArrayBuffer to a Base64 string
 * @param {ArrayBuffer} buffer - The array buffer to convert
 * @returns {string} - Base64 string
 */
function arrayBufferToBase64(buffer) {
    const bytes = new Uint8Array(buffer);
    let binary = '';
    for (let i = 0; i < bytes.length; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return btoa(binary);
}