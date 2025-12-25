export {};

declare global {
    interface Window {
        hljs?: {
            highlightElement: (el: HTMLElement) => void;
        };
    }
}
