import Layout from "./Layout";
import CreatePastePage from "./CreatePastePage";

export default function App() {
    return (
        <Layout>
            <CreatePastePage
                onCreatePaste={(plaintext) => {
                    console.log("Create paste:", plaintext.slice(0, 40));
                }}
            />
        </Layout>
    );
}
