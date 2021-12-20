(function () {
    // Note: Replace with your own key pair before deploying
    const applicationServerPublicKey = "BByg5Mu5Zqii6F8mbISwyKjscdBBbGTwWtKWKf_jtEp8k9RWynN9ILXIbrCiTE90Abp4us9DGAnvCnvNfV8-siY";

    window.blazorPushNotifications = {
        requestSubscription: async () => {
            console.log("blazorPushNotifications.requestSubscription");
            const worker = await navigator.serviceWorker.getRegistration();
            const existingSubscription = await worker.pushManager.getSubscription();
            if (!existingSubscription) {
                const newSubscription = await subscribe(worker);
                if (newSubscription) {
                    return {
                        url: newSubscription.endpoint,
                        p256dh: arrayBufferToBase64(newSubscription.getKey("p256dh")),
                        auth: arrayBufferToBase64(newSubscription.getKey("auth"))
                    };
                }
            }
        }
    };

    async function subscribe(worker) {
        try {
            return await worker.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: applicationServerPublicKey
            });
        } catch (error) {
            if (error.name === "NotAllowedError") {
                return null;
            }
            throw error;
        }
    }

    function arrayBufferToBase64(buffer) {
        // https://stackoverflow.com/a/9458996
        var binary = "";
        const bytes = new Uint8Array(buffer);
        const len = bytes.byteLength;
        for (let i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }
})();
