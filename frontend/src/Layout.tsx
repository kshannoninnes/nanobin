import type { PropsWithChildren } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./Layout.module.css";
import {CopyButton} from "./components/CopyButton.tsx";

type LayoutProps = PropsWithChildren<{
    viewingPaste: boolean;
    isCopyEnabled: boolean;
    copied: boolean;
    onCopy: () => Promise<void>;
}>;

export default function Layout({ children, viewingPaste, isCopyEnabled, copied, onCopy }: LayoutProps) {
    const navigate = useNavigate();

    function handleNavigateHome() {
        navigate("/");
    }

    return (
        <div className={styles.container}>
            <header className={styles.siteHeader}>
                <h1 className={styles.title} onClick={handleNavigateHome}>
                    nanobin
                </h1>

                <span className={styles.subtitle}>A minimalistic pastebin service</span>

                <div className={styles.tools}>
                    {viewingPaste && (<CopyButton enabled={isCopyEnabled} copied={copied} onCopy={onCopy}/>)}
                </div>
            </header>

            <main className={styles.mainContent}>{children}</main>
        </div>
    );
}
