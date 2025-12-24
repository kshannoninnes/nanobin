import { Base64 } from "js-base64";
import { encrypt, decrypt } from "./pasteCrypto.ts";
import { CreatePasteResponseSchema, GetPasteResponseSchema } from "./pasteAPISchema.ts";

export async function createPaste(plaintext: string): Promise<string> {
    if (plaintext.trim().length === 0) {
        throw new Error("Paste content is empty");
    }

    const plaintextBytes = utf8Encode(plaintext);
    const { ciphertextBytes, ivBytes, keyBytes } = await encrypt(plaintextBytes);
    const test = base64Encode(ivBytes);

    const response = await fetch("/api/pastes", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Accept: "application/json",
        },
        body: JSON.stringify({
            ciphertextBase64: base64Encode(ciphertextBytes),
            ivBase64: test,
        }),
    });

    if (!response.ok) {
        const errorText = await safeReadText(response);
        throw new Error(errorText || `Create failed (${response.status})`);
    }

    const parsed = CreatePasteResponseSchema.parse(await response.json());

    return buildShareUrl(parsed.id, base64UrlEncode(keyBytes));
}

export async function getPaste(pasteId: string, keyBase64Url: string): Promise<string> {
    if (pasteId.trim().length === 0) {
        throw new Error("Paste id is empty");
    }
    if (keyBase64Url.trim().length === 0) {
        throw new Error("Paste key is empty");
    }

    const response = await fetch(`/api/pastes/${encodeURIComponent(pasteId)}`, {
        method: "GET",
        headers: { Accept: "application/json" },
    });

    if (!response.ok) {
        const errorText = await safeReadText(response);
        throw new Error(errorText || `Fetch failed (${response.status})`);
    }

    const parsed = GetPasteResponseSchema.parse(await response.json());

    let plaintextBytes: Uint8Array;
    try {
        plaintextBytes = await decrypt({
            ciphertextBytes: base64Decode(parsed.ciphertextBase64),
            ivBytes: base64Decode(parsed.ivBase64),
            keyBytes: base64UrlDecode(keyBase64Url),
        });
    } catch {
        throw new Error("Decryption failed (bad key or corrupted paste)");
    }

    return utf8Decode(plaintextBytes);
}

function buildShareUrl(pasteId: string, keyBase64Url: string): string {
    return `${window.location.origin}/${pasteId}#${keyBase64Url}`;
}

async function safeReadText(response: Response): Promise<string> {
    try {
        return await response.text();
    } catch {
        return "";
    }
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
