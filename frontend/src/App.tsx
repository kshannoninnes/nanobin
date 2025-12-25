import {Routes, Route, Navigate, useNavigate, useLocation} from "react-router-dom";
import Layout from "./Layout";
import CreatePastePage from "./CreatePastePage";
import ViewPastePage from "./ViewPastePage";
import { createPaste } from "./services/pasteService";

function CreatePasteRoute() {
    const navigate = useNavigate();

    return (
        <CreatePastePage
            onCreatePaste={async (plaintext) => {
                const [pasteId, keyBase64Url] = await createPaste(plaintext);
                navigate(`/${pasteId}#${keyBase64Url}`, { replace: true });
            }}
        />
    );
}

export default function App() {
    const location = useLocation();

    return (
        <Layout>
            <Routes>
                <Route path="/" element={<CreatePasteRoute />} />
                <Route path="/:pasteId" element={<ViewPastePage key={location.pathname + location.hash} />}/>
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </Layout>
    );
}
