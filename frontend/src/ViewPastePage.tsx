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
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    const codeRef = useRef<HTMLElement | null>(null);

    const canLoad = pasteId && keyFragment;

    useEffect(() => {
        if (!canLoad) return;

        const controller = new AbortController();

        getPaste(pasteId, keyFragment, controller.signal)
            .then(setPlaintext)
            .catch((err) => {
                if (err?.name !== "AbortError") setErrorMessage("Unable to load paste.");
            });

        return () => controller.abort();
    }, [canLoad, pasteId, keyFragment]);

    // Highlight once plaintext is present
    useEffect(() => {
        const element = codeRef.current;
        if (!plaintext || !element) return;

        element.textContent = plaintext;
        hljs.highlightElement(element);
    }, [plaintext]);

    const error = errorMessage ?? (!pasteId || !keyFragment ? "Unable to load content." : null);
    if (error) {
        return (
            <div className={styles.viewPaste}>
                <div className={styles.error}>{error}</div>
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
