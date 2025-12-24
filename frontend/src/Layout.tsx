import type { PropsWithChildren } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./Layout.module.css";

export default function Layout({ children }: PropsWithChildren) {
    const navigate = useNavigate();

    function handleNavigateHome() {
        navigate("/");
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
