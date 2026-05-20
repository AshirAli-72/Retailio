// ── E-Invoice System Service Worker v4 ─────────────────────────────────────
// Changes: Network-First for HTML/Navigation, Cache-First for static assets.

const CACHE_NAME = 'sata-pos-v5';

const STATIC_ASSETS = [
    '/Account/Login',
    '/css/dashboard.css',
    '/css/customer.css',
    '/css/sale.css',
    '/css/product.css',
    '/css/settings.css',
    '/css/login.css',
    '/lib/inter-font/inter.css',
    '/lib/phosphor/icons.css',
    '/js/site.js',
    '/js/app-main.js',
    '/images/sata-logo.png',
    '/favicon.ico'
];

// ── Install: pre-cache static assets ───────────────────────────────────────
self.addEventListener('install', event => {
    self.skipWaiting();
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache =>
            Promise.allSettled(
                STATIC_ASSETS.map(url =>
                    cache.add(url).catch(() => { /* asset may not exist yet */ })
                )
            )
        )
    );
});

// ── Activate: delete old caches ─────────────────────────────────────────────
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys()
            .then(keys => Promise.all(
                keys.filter(k => k !== CACHE_NAME).map(k => caches.delete(k))
            ))
            .then(() => self.clients.claim())
    );
});

// ── Helpers ──────────────────────────────────────────────────────────────────
const isSkipped = url => {
    const p = new URL(url).pathname;
    return p.startsWith('/_blazor') ||
           p.startsWith('/api/') ||
           p.includes('blazor.server.js') ||
           p.includes('__');
};

// ── Fetch Strategy ───────────────────────────────────────────────────────────
self.addEventListener('fetch', event => {
    const { request } = event;
    const url = request.url;

    // Only handle same-origin GETs
    if (request.method !== 'GET' ||
        new URL(url).origin !== self.location.origin ||
        isSkipped(url)) return;

    // ── Navigation Requests: Network-First ──
    if (request.mode === 'navigate') {
        event.respondWith(
            fetch(request)
                .then(response => {
                    if (response && response.status === 200) {
                        const copy = response.clone();
                        caches.open(CACHE_NAME).then(cache => cache.put(request, copy));
                    }
                    return response;
                })
                .catch(() => {
                    // Offline fallback: try to serve from cache, otherwise serve Login page
                    return caches.match(request).then(cached => cached || caches.match('/Account/Login'));
                })
        );
        return;
    }

    // ── Static Assets: Cache-First with Stale-While-Revalidate ──
    event.respondWith(
        caches.open(CACHE_NAME).then(cache =>
            cache.match(request).then(cached => {
                const networkFetch = fetch(request)
                    .then(response => {
                        if (response && response.status === 200) {
                            cache.put(request, response.clone());
                        }
                        return response;
                    })
                    .catch(() => null);

                return cached || networkFetch;
            })
        )
    );
});