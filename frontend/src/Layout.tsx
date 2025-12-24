import type { PropsWithChildren } from "react";
import styles from "./Layout.module.css";

export default function Layout({ children }: PropsWithChildren) {
    function handleNavigateHome() {
        // For now: same behavior as navigating to "/"
        window.location.href = "/";
    }

    return (
        <div className={styles.container}>
            <header className={styles.siteHeader}>
                <h1 className={styles.title} onClick={handleNavigateHome}>
                    Nanobin
                </h1>
                <span className={styles.subtitle}>A minimalistic pastebin service</span>
            </header>

            <main className={styles.mainContent}>{children}</main>
        </div>
    );
}
