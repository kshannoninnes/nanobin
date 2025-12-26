import { Base64 } from "js-base64";
import { encrypt, decrypt } from "../encryption/pasteCrypto.ts";
import { CreatePasteResponseSchema, GetPasteResponseSchema } from "./pasteSchema.ts";
import type { CreatePasteResponse, GetPasteResponse } from "./pasteSchema.ts";

export async function createPaste(plaintext: string): Promise<[string, string]> {
    // Validate input
    if (plaintext.trim().length === 0) {
        throw new Error("Paste content is empty.");
    }

    // Encrypt content
    const plaintextBytes = utf8Encode(plaintext);
    const { ciphertextBytes, ivBytes, keyBytes } = await encrypt(plaintextBytes);

    // POST encrypted content
    let response: Response;
    try {
        response = await fetch("/api/pastes", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Accept: "application/json",
            },
            body: JSON.stringify({
                ciphertextBase64: base64Encode(ciphertextBytes),
                ivBase64: base64Encode(ivBytes),
            }),
        });
    } catch {
        throw new Error("Error creating paste.");
    }

    // Validate response
    if (!response.ok) {
        throw new Error("Error creating paste.");
    }

    // Parse JSON
    let json: unknown;
    let validated: CreatePasteResponse;
    try {
        json = await response.json();
        validated = CreatePasteResponseSchema.parse(json);
    } catch {
        throw new Error("Error creating paste.");
    }

    // Return paste ID and key for redirect
    return [validated.id, base64UrlEncode(keyBytes)];
}

export async function getPaste(pasteId: string, decryptionKey: string, signal: AbortSignal): Promise<string> {
    // Validate input
    if (pasteId.trim().length === 0) {
        throw new Error("Invalid paste URL.");
    }

    if (decryptionKey.trim().length === 0) {
        throw new Error("Missing decryption key in URL fragment. The key must appear after '#' and is never sent to the server.");
    }

    // Fetch ciphertext and iv
    let response: Response;
    try {
        response = await fetch(`/api/pastes/${encodeURIComponent(pasteId)}`, {
            method: "GET",
            headers: { Accept: "application/json" },
            signal,
        });
    } catch (err: unknown) {
        if (signal.aborted) throw err;
        throw new Error("Error retrieving paste.");
    }

    // Validate response
    if (response.status === 404) {
        throw new Error("Paste not found (or expired).");
    }

    if (!response.ok) {
        throw new Error("Error retrieving paste.");
    }

    // Parse JSON
    let json: unknown;
    let validated: GetPasteResponse;
    try {
        json = await response.json();
        validated = GetPasteResponseSchema.parse(json);
    } catch {
        throw new Error("Error retrieving paste.");
    }

    // Decrypt content
    let plaintextBytes: Uint8Array;
    try {
        plaintextBytes = await decrypt({
            ciphertextBytes: base64Decode(validated.ciphertextBase64),
            ivBytes: base64Decode(validated.ivBase64),
            keyBytes: base64UrlDecode(decryptionKey),
        });
    } catch {
        throw new Error("Decryption failed (bad key or corrupted paste).");
    }

    // Return decrypted content
    return utf8Decode(plaintextBytes);
}

/**
 * UTF8 Encoding/Decoding for bytes <-> plaintext conversion
 */
function utf8Encode(text: string): Uint8Array {
    return new TextEncoder().encode(text);
}

function utf8Decode(bytes: Uint8Array): string {
    return new TextDecoder().decode(bytes);
}

/**
 * Standard base64 (with '=' padding). Used for API payloads.
 */
function base64Encode(bytes: Uint8Array): string {
    return Base64.fromUint8Array(bytes);
}

function base64Decode(base64: string): Uint8Array {
    return Base64.toUint8Array(base64);
}

/**
 * Base64url (no padding). Used for the URL fragment.
 */
function base64UrlEncode(bytes: Uint8Array): string {
    return Base64.fromUint8Array(bytes, true);
}

function base64UrlDecode(base64Url: string): Uint8Array {
    return Base64.toUint8Array(base64Url);
}
