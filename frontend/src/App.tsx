import {Routes, Route, Navigate, useNavigate, useMatch} from "react-router-dom";
import { useCallback, useState } from "react";
import Layout from "./Layout";
import CreatePastePage from "./pages/CreatePastePage";
import ViewPastePage from "./pages/ViewPastePage";
import { createPaste } from "./api/pasteService";

type CopyAction = (() => Promise<void>) | null;

export default function App() {
    const navigate = useNavigate();

    const [copyAction, setCopyAction] = useState<CopyAction>(null);
    const [copied, setCopied] = useState(false);

    const setHeaderCopyAction = useCallback((action: CopyAction) => {
        setCopyAction(action === null ? null : () => action);
    }, []);

    const handleCopy = useCallback(async () => {
        if (!copyAction) return;

        await copyAction();
        setCopied(true);
        window.setTimeout(() => setCopied(false), 3000);
    }, [copyAction]);

    function renderCreatePasteRoute() {
        return (<Route path="/" element={
                    <CreatePastePage
                        onCreatePaste={async (plaintext) => {
                            const [pasteId, keyBase64Url] = await createPaste(plaintext);
                            navigate(`/${pasteId}#${keyBase64Url}`, { replace: true });
                        }}
                    />
                }
            />
        );
    }

    function renderViewPasteRoute() {
        return (<Route path="/:pasteId" element={<ViewPastePage setHeaderCopyAction={setHeaderCopyAction}/>}/>);
    }

    function renderFallbackRoute() {
        return <Route path="*" element={<Navigate to="/" replace />} />;
    }

    const viewingPaste = Boolean(useMatch("/:pasteId"))
    const copyEnabled = copyAction !== null;

    return (
        <Layout viewingPaste={viewingPaste} isCopyEnabled={copyEnabled} copied={copied} onCopy={handleCopy}>
            <Routes>
                {renderCreatePasteRoute()}
                {renderViewPasteRoute()}
                {renderFallbackRoute()}
            </Routes>
        </Layout>
    );
}
