
// ── E-Invoice System Service Worker v3 ─────────────────────────────────────
// Changes: login page cached, stale-while-revalidate for HTML,
//          smarter offline fallback, bumped cache name.

const CACHE_NAME = 'sata-pos-v4';

const STATIC_ASSETS = [
    '/',
    '/Account/Login',
    '/customer',
    '/product',
    '/sale',
    '/Invoice',
    '/settings',
    '/css/dashboard.css',
    '/css/customer.css',
    '/css/invoice.css',
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
const isStaticAsset = url =>
    /\.(css|js|woff2?|ttf|eot|png|jpg|jpeg|gif|svg|ico|webp)$/i.test(new URL(url).pathname);

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

    // ── Offline-First (Stale-While-Revalidate) for both assets and HTML pages ──
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

                // Return cached version immediately if available, otherwise wait for network
                return cached || networkFetch;
            })
        ).catch(() => {
            // Ultimate fallback for HTML pages
            if (request.mode === 'navigate') {
                return caches.match('/Account/Login');
            }
        })
    );
});