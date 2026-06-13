/**
 * Attachment Compression Utility
 * Handles client-side compression for various file types to save storage space.
 */

const AttachmentCompression = {
    /**
     * Compresses a file based on its type.
     * @param {File} file - The original file object.
     * @param {Object} options - Compression options.
     * @returns {Promise<File>} - The compressed file object.
     */
    async compress(file, options = {}) {
        if (!file) return null;

        const originalSize = file.size;
        console.log(`Original size: ${(originalSize / 1024).toFixed(2)} KB (${file.type})`);

        let compressedFile = file;

        if (file.type.startsWith('image/')) {
            compressedFile = await this.compressImage(file, options.image);
        } else {
            compressedFile = await this.compressGeneric(file);
        }

        // If compression actually made it larger (rare but possible with ZIP/GZIP on small pre-compressed files)
        // or if it failed to do anything, we keep the original.
        if (compressedFile.size >= originalSize && compressedFile !== file) {
            console.log("Compression did not save space. Reverting to original file.");
            return file;
        }

        const finalSize = compressedFile.size;
        const ratio = ((1 - (finalSize / originalSize)) * 100).toFixed(2);
        console.log(`Final size: ${(finalSize / 1024).toFixed(2)} KB. Space saved: ${ratio}%`);

        return compressedFile;
    },

    /**
     * Compresses an image using aggressive lossy compression.
     */
    async compressImage(file, options = {}) {
        const defaultOptions = {
            maxSizeMB: 0.2,          // Aggressive: Target 200KB
            maxWidthOrHeight: 1280,  // Reduce resolution
            useWebWorker: true,
            initialQuality: 0.6,     // Lower initial quality
            alwaysKeepResolution: false
        };

        const config = { ...defaultOptions, ...options };

        try {
            if (typeof imageCompression === 'undefined') {
                console.warn('browser-image-compression library not found.');
                return file;
            }

            console.log(`Aggressively compressing image: ${file.name}`);
            const result = await imageCompression(file, config);
            return new File([result], file.name, { type: result.type });
        } catch (error) {
            console.error('Image compression failed:', error);
            return file;
        }
    },

    /**
     * Compresses any file using lossless GZIP compression (better for single files than ZIP).
     */
    async compressGeneric(file) {
        try {
            // Robust check for fflate
            const lib = window.fflate || (typeof fflate !== 'undefined' ? fflate : null);
            
            if (!lib || (!lib.gzipSync && !lib.zipSync)) {
                console.warn('fflate library not found or incomplete.');
                return file;
            }

            console.log(`Starting compression for: ${file.name}`);
            const fileData = await file.arrayBuffer();
            const uint8Data = new Uint8Array(fileData);
            
            let compressedData;
            let newFileName = file.name;
            let newType = file.type;

            if (lib.gzipSync) {
                // GZIP is more efficient for single files
                compressedData = lib.gzipSync(uint8Data, { level: 9 });
                newFileName = file.name + '.gz';
                newType = 'application/gzip';
            } else {
                // Fallback to ZIP if GZIP isn't available
                compressedData = lib.zipSync({ [file.name]: uint8Data }, { level: 9 });
                newFileName = file.name + '.zip';
                newType = 'application/zip';
            }
            
            const compressedFile = new File([compressedData], newFileName, { type: newType });
            return compressedFile;
        } catch (error) {
            console.error('Generic compression failed:', error);
            return file;
        }
    }
};

window.AttachmentCompression = AttachmentCompression;
