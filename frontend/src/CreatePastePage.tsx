import { useEffect, useMemo, useRef, useState } from "react";
import styles from "./CreatePastePage.module.css";

type CreatePastePageProps = {
    onCreatePaste?: (plaintext: string) => Promise<void> | void;
};

export default function CreatePastePage({ onCreatePaste }: CreatePastePageProps) {
    const textareaRef = useRef<HTMLTextAreaElement | null>(null);
    const [pasteContent, setPasteContent] = useState<string>("");

    const canSubmit = useMemo(() => pasteContent.trim().length > 0, [pasteContent]);

    useEffect(() => {
        textareaRef.current?.focus();
    }, []);

    async function handleSubmit() {
        if (!canSubmit) return;
        await onCreatePaste?.(pasteContent);
    }

    return (
        <div className={styles.createPaste}>
            <div className={styles.editorContainer}>
        <textarea
            ref={textareaRef}
            className={styles.pasteInput}
            placeholder="Enter your text here..."
            value={pasteContent}
            onChange={(event) => setPasteContent(event.target.value)}
        />
            </div>

            <button type="button" className={styles.createButton} onClick={() => void handleSubmit()}>
                Create Paste
            </button>
        </div>
    );
}
