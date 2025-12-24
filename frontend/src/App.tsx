import { Routes, Route, Navigate, useNavigate } from "react-router-dom";
import Layout from "./Layout";
import CreatePastePage from "./CreatePastePage";
import ViewPastePage from "./ViewPastePage";
import { createPaste } from "./services/pasteService";

function CreatePasteRoute() {
    const navigate = useNavigate();

    return (
        <CreatePastePage
            onCreatePaste={async (plaintext) => {
                // shareUrl: `${origin}/${pasteId}#${key}`
                const shareUrl = await createPaste(plaintext);
                const parsedUrl = new URL(shareUrl);
                navigate(parsedUrl.pathname + parsedUrl.hash, { replace: true });
            }}
        />
    );
}

export default function App() {
    return (
        <Layout>
            <Routes>
                <Route path="/" element={<CreatePasteRoute />} />
                <Route path="/:pasteId" element={<ViewPastePage />} />
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </Layout>
    );
}
