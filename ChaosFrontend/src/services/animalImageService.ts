const limpiarSvg = (svg: string) =>
    svg.replace(/<\?xml.*?\?>/, '').replace(/<!--.*?-->/g, '').trim();

/**
 * Resolves the image source for an animal from its DB field value.
 * Supports both legacy inline SVG string content and static URL path values.
 */
export function getAnimalImageUrl(svgBd: string | null | undefined): string {
    if (!svgBd || svgBd.trim() === "") {
        return "";
    }

    const trimmed = svgBd.trim();
    const isRawSvg = trimmed.startsWith("<") || trimmed.startsWith("<?xml");

    if (isRawSvg) {
        try {
            const svgLimpio = limpiarSvg(trimmed);
            const svgBase64 = btoa(unescape(encodeURIComponent(svgLimpio)));
            return `data:image/svg+xml;base64,${svgBase64}`;
        } catch (e) {
            console.error("Error base64 encoding SVG:", e);
            return "";
        }
    } else {
        if (trimmed.startsWith("http://") || trimmed.startsWith("https://") || trimmed.startsWith("data:")) {
            return trimmed;
        }

        const baseUrl = import.meta.env.VITE_BASE_URL || 'https://localhost:7101';
        return `${baseUrl.replace(/\/$/, '')}/${trimmed.replace(/^\//, '')}`;
    }
}
