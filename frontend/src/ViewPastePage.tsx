import { useEffect, useRef, useState } from "react";
import { useParams, useLocation } from "react-router-dom";
import styles from "./ViewPastePage.module.css";
import { getPaste } from "./services/pasteService";
import hljs from "highlight.js";
import "highlight.js/styles/github.css";

export default function ViewPastePage() {
    const { pasteId } = useParams<{ pasteId: string }>();

    const { hash } = useLocation();
    const keyFragment = hash.startsWith("#") ? hash.slice(1) : "";

    const [plaintext, setPlaintext] = useState<string | null>(null);
    const [failedPasteId, setFailedPasteId] = useState<string | null>(null);

    const codeRef = useRef<HTMLElement | null>(null);

    // Retrieve decrypted text
    useEffect(() => {
        if (!pasteId || !keyFragment) return;

        const controller = new AbortController();

        getPaste(pasteId, keyFragment, controller.signal)
            .then(setPlaintext)
            .catch((err) => {
                if (err?.name !== "AbortError") {
                    setFailedPasteId(pasteId);
                }
            });

        return () => controller.abort();
    }, [pasteId, keyFragment]);

    // Highlight once text is present
    useEffect(() => {
        const element = codeRef.current;
        if (plaintext === null || !element) return;

        element.textContent = plaintext;
        hljs.highlightElement(element);
    }, [plaintext]);

    if (!pasteId || !keyFragment || failedPasteId === pasteId) {
        return (
            <div className={styles.viewPaste}>
                <div className={styles.error}>Unable to load content.</div>
            </div>
        );
    }

    if (plaintext === null) return null;

    return (
        <div className={styles.viewPaste}>
            <div className={styles.pasteContainer}>
                <pre className={styles.pre}>
                    <code ref={codeRef} className={`${styles.decryptedContent} hljs`} />
                </pre>
            </div>
        </div>
    );
}
