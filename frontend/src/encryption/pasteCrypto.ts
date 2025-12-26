const AES_KEY_BITS = 256;
const AES_256_KEY_BYTES = AES_KEY_BITS / 8;
const GCM_IV_BYTES = 12;

/*
 * Encrypts plaintext using a freshly generated random AES-GCM key.
 * The key is returned alongside the ciphertext and IV to enable decryption.
 * The key must never leave the client (eg. only stored in URL fragments) to preserve E2EE guarantee.
 */
export async function encrypt(plaintextBytes: Uint8Array): Promise<{
    ciphertextBytes: Uint8Array;
    ivBytes: Uint8Array;
    keyBytes: Uint8Array;
}> {
    const cryptoKey = await crypto.subtle.generateKey(
        { name: "AES-GCM", length: AES_KEY_BITS },
        true,
        ["encrypt", "decrypt"],
    );

    const ivBytes = crypto.getRandomValues(new Uint8Array(GCM_IV_BYTES));

    const ciphertextBuffer = await crypto.subtle.encrypt(
        { name: "AES-GCM", iv: ivBytes },
        cryptoKey,
        plaintextBytes as BufferSource,
    );

    const keyBuffer = await crypto.subtle.exportKey("raw", cryptoKey);

    return {
        ciphertextBytes: new Uint8Array(ciphertextBuffer),
        ivBytes,
        keyBytes: new Uint8Array(keyBuffer),
    };
}

/*
 * Decrypts ciphertext using the given key and IV.
 * Throws if authentication fails (wrong key, IV, or ciphertext).
 */
export async function decrypt(params: {
    ciphertextBytes: Uint8Array;
    ivBytes: Uint8Array;
    keyBytes: Uint8Array;
}): Promise<Uint8Array> {
    if (params.ivBytes.byteLength !== GCM_IV_BYTES) {
        throw new Error(
            `Invalid IV length: expected ${GCM_IV_BYTES} bytes, got ${params.ivBytes.byteLength}`,
        );
    }

    if (params.keyBytes.byteLength !== AES_256_KEY_BYTES) {
        throw new Error(
            `Invalid key length: expected ${AES_256_KEY_BYTES} bytes, got ${params.keyBytes.byteLength}`,
        );
    }

    const cryptoKey = await crypto.subtle.importKey(
        "raw",
        params.keyBytes as BufferSource,
        { name: "AES-GCM" },
        false,
        ["decrypt"],
    );

    const plaintextBuffer = await crypto.subtle.decrypt(
        { name: "AES-GCM", iv: params.ivBytes as BufferSource },
        cryptoKey,
        params.ciphertextBytes as BufferSource,
    );


    return new Uint8Array(plaintextBuffer);
}