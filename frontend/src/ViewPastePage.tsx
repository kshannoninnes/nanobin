import { useEffect, useState } from "react";
import { useParams, useLocation } from "react-router-dom";
import styles from "./ViewPastePage.module.css";
import { getPaste } from "./services/pasteService";

export default function ViewPastePage() {
    const { pasteId } = useParams<{ pasteId: string }>();

    const { hash } = useLocation();
    const keyFragment = hash.startsWith("#") ? hash.slice(1) : "";

    const [plaintext, setPlaintext] = useState<string | null>(null);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    const canLoad = pasteId && keyFragment;

    // Get the current paste on page load
    useEffect(() => {
        if (!canLoad) return;

        const controller = new AbortController();

        getPaste(pasteId, keyFragment, controller.signal)
            .then(setPlaintext)
            .catch(err => {
                if (err.name !== "AbortError") {
                    setErrorMessage("Unable to load paste.");
                }
            });

        return () => controller.abort();
    }, [canLoad, pasteId, keyFragment]);

    // Don't render anything if we don't have any content yet (eg. just started loading the page)
    if (plaintext === null) return null;

    // Display an error message if any errors during page load/paste retrieval
    const error = errorMessage ?? (!pasteId || !keyFragment ? "Unable to load content." : null);
    if (error) {
        return (
            <div className={styles.viewPaste}>
                <div className={styles.error}>{error}</div>
            </div>
        );
    }

    // Display regular content if everything goes well
    return (
        <div className={styles.viewPaste}>
            <div className={styles.pasteContainer}>
                <pre className={styles.pre}>
                    <code className={styles.decryptedContent}>{plaintext}</code>
                </pre>
            </div>
        </div>
    );
}