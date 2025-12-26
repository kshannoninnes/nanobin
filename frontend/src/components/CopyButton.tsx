import styles from "./CopyButton.module.css";

type TextButtonProps = {
    enabled: boolean;
    copied: boolean;
    onCopy: () => void;
};

export function CopyButton({ enabled, copied, onCopy }: TextButtonProps) {
    return (
        <button
            type="button"
            className={`${styles.copyButton} ${copied ? styles.copied : ""}`}
            onClick={onCopy}
            disabled={!enabled}
            aria-disabled={!enabled}
        >
          <span className={`${styles.label} ${styles.copyLabel} ${copied ? styles.hidden : styles.visible}`}>
            copy
          </span>
          <span className={`${styles.label} ${copied ? styles.visible : styles.hidden}`}>
            copied!
          </span>
        </button>
    );
}
