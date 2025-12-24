import { useEffect, useMemo, useState } from "react";
import { useParams } from "react-router-dom";
import styles from "./ViewPastePage.module.css";
import { getPaste } from "./services/pasteService";

type ViewPasteError =
    | { code: "missingPasteId" }
    | { code: "missingKey" }
    | { code: "notFound" }
    | { code: "decryptionFailed" }
    | { code: "networkError" };

type ViewState =
    | { kind: "pending" }
    | { kind: "success"; plaintext: string }
    | { kind: "error"; error: ViewPasteError };

type ErrorKind = ViewPasteError["code"];

function readKeyFromHashOnce(): string | null {
    const hashValue = window.location.hash;
    if (!hashValue) return null;

    const keyFragment = hashValue.startsWith("#") ? hashValue.slice(1) : hashValue;
    return keyFragment.length > 0 ? keyFragment : null;
}

function normalizeErrorKind(error: unknown): Exclude<ErrorKind, "missingPasteId" | "missingKey"> {
    const message = error instanceof Error ? error.message : "";

    if (message.toLowerCase().includes("decryption failed")) return "decryptionFailed";
    if (message.includes("404") || message.toLowerCase().includes("not found")) return "notFound";
    return "networkError";
}

function errorMessage(error: ViewPasteError): string {
    switch (error.code) {
        case "missingPasteId":
            return "Invalid paste URL.";

        case "missingKey":
            return (
                "Missing decryption key in URL fragment. " +
                "The key must appear after '#' and is never sent to the server."
            );

        case "notFound":
            return "Paste not found (or expired).";

        case "decryptionFailed":
            return "Decryption failed (bad key or corrupted paste).";

        case "networkError":
            return "Error retrieving paste.";
    }
}

export default function ViewPastePage() {
    const { pasteId } = useParams<{ pasteId: string }>();
    const keyFragment = useMemo(() => readKeyFromHashOnce(), []);

    const [viewState, setViewState] = useState<ViewState>(() => {
        if (!pasteId) return { kind: "error", error: { code: "missingPasteId" } };
        if (!keyFragment) return { kind: "error", error: { code: "missingKey" } };
        return { kind: "pending" };
    });

    useEffect(() => {
        let cancelled = false;

        async function loadAndDecrypt() {
            if (!pasteId) return;
            if (!keyFragment) return;

            try {
                const plaintext = await getPaste(pasteId, keyFragment);
                if (cancelled) return;

                setViewState({ kind: "success", plaintext });
            } catch (error: unknown) {
                if (cancelled) return;

                setViewState({ kind: "error", error: { code: normalizeErrorKind(error) } });
            }
        }

        // Only attempt fetch/decrypt if we are not in a pre-validation error state
        if (pasteId && keyFragment) {
            loadAndDecrypt();
        }

        return () => {
            cancelled = true;
        };
    }, [pasteId, keyFragment]);

    // Still loading, nothing to show yet
    if (viewState.kind === "pending") return null;

    // Error occurred, show the error message
    if (viewState.kind === "error") {
        return (
            <div className={styles.viewPaste}>
                <div className={styles.error}>{errorMessage(viewState.error)}</div>
            </div>
        );
    }

    // Success, show decrypted content
    return (
        <div className={styles.viewPaste}>
            <div className={styles.pasteContainer}>
                <pre className={styles.pre}>
                  <code className={styles.decryptedContent}>{viewState.plaintext}</code>
                </pre>
            </div>
        </div>
    );
}
