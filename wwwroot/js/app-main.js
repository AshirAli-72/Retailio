/**
 * app-main.js — E-Invoice System Core Logic
 * Professional SPA-feel navigation: progress bar, prefetch, instant active state,
 * page entry animation, sidebar toggle, toast notifications, online/offline.
 */

// ─── Progress Bar ─────────────────────────────────────────────────────────────
const NavProgress = (() => {
    let bar, timer, currentWidth = 0;

    function create() {
        if (document.getElementById('nav-progress-bar')) return;
        bar = document.createElement('div');
        bar.id = 'nav-progress-bar';
        Object.assign(bar.style, {
            position: 'fixed', top: '0', left: '0', height: '3px',
            width: '0%', background: 'var(--primary)',
            zIndex: '99999', transition: 'width 0.25s ease, opacity 0.4s ease',
            boxShadow: '0 0 8px var(--primary)', opacity: '1', borderRadius: '0 2px 2px 0'
        });
        document.body.appendChild(bar);
    }

    function start() {
        create();
        clearInterval(timer);
        currentWidth = 0;
        bar.style.opacity = '1';
        bar.style.width = '0%';
        requestAnimationFrame(() => {
            bar.style.width = '30%';
            timer = setInterval(() => {
                if (currentWidth < 85) {
                    currentWidth += (85 - currentWidth) * 0.08;
                    bar.style.width = currentWidth + '%';
                }
            }, 200);
        });
    }

    function finish() {
        if (!bar) return;
        clearInterval(timer);
        bar.style.width = '100%';
        setTimeout(() => { 
            if (bar) bar.style.opacity = '0'; 
            setTimeout(() => { if (bar) bar.style.width = '0%'; }, 200); 
        }, 100);
    }

    return { start, finish };
})();

// ─── Prefetch Cache ────────────────────────────────────────────────────────────
const EXCLUDED_PATHS = [
    '/Account/Logout', 
    '/Account/Login', 
    '/sale', 
    '/reports', 
    '/customer', 
    '/product', 
    '/inventory', 
    '/employee', 
    '/settings',
    '/recovery'
];

function isPathExcluded(url) {
    if (!url) return true;
    try {
        const path = new URL(url, window.location.origin).pathname.toLowerCase();
        return EXCLUDED_PATHS.some(p => path.startsWith(p.toLowerCase()));
    } catch (e) { return true; }
}

const prefetched = new Map();
async function prefetchPage(url) {
    if (prefetched.has(url)) return prefetched.get(url);
    if (isPathExcluded(url)) return null; 
    
    // Store the promise itself to handle concurrent requests
    const fetchPromise = (async () => {
        try {
            const res = await fetch(url, { priority: 'low' });
            if (res.ok) return await res.text();
        } catch (err) {
            console.warn('Prefetch failed:', url, err);
        }
        return null;
    })();
    
    prefetched.set(url, fetchPromise);
    return fetchPromise;
}

// ─── Fast Navigation (SPA Feel) ─────────────────────────────────────────────
async function fastNavigate(url, pushState = true) {
    if (!url || url.startsWith('#') || url.includes('/Account/Logout') || isPathExcluded(url)) {
        if (pushState) window.location.href = url;
        return false;
    }

    NavProgress.start();
    try {
        let html = null;
        if (prefetched.has(url)) {
            html = await prefetched.get(url);
        }
        
        if (!html) {
            const res = await fetch(url, { cache: 'no-cache' });
            if (!res.ok) { window.location.href = url; return; }
            html = await res.text();
        }

        const parser = new DOMParser();
        const doc = parser.parseFromString(html, 'text/html');
        const newMain = doc.querySelector('.main-content');
        const oldMain = document.querySelector('.main-content');

        if (newMain && oldMain) {
            if (pushState) window.history.pushState({}, '', url);
            
            // OPTIMIZED: Delay 'loading' dimming to avoid flicker on fast pages
            let showLoading = true;
            const loadingTimer = setTimeout(() => { if (showLoading) oldMain.classList.add('loading'); }, 50);

            setTimeout(() => {
                showLoading = false; // Page is ready, don't show dimming if not already shown
                clearTimeout(loadingTimer);

                // ── Sync Head (CSS & Styles) ──
                const newHead = doc.head;
                const currentHead = document.head;
                
                // Sync Stylesheets with robust selector
                newHead.querySelectorAll('link[rel="stylesheet"]').forEach(newLink => {
                    const href = newLink.getAttribute('href');
                    if (!href) return;
                    // Check for existence ignoring protocol/host if possible, or just exact match
                    let exists = false;
                    currentHead.querySelectorAll('link[rel="stylesheet"]').forEach(l => {
                        if (l.getAttribute('href') === href) exists = true;
                    });
                    
                    if (!exists) {
                        const link = document.createElement('link');
                        link.rel = 'stylesheet';
                        link.href = href;
                        // Use append version if present in attributes
                        Array.from(newLink.attributes).forEach(attr => {
                            if (attr.name !== 'href' && attr.name !== 'rel') link.setAttribute(attr.name, attr.value);
                        });
                        currentHead.appendChild(link);
                    }
                });

                // Sync Inline Styles
                newHead.querySelectorAll('style').forEach(newStyle => {
                    currentHead.appendChild(newStyle.cloneNode(true));
                });

                // Swap Title
                document.title = doc.title;

                // Swap Main Content
                oldMain.innerHTML = newMain.innerHTML;

                // Extract and execute scripts
                const scripts = oldMain.querySelectorAll('script');
                scripts.forEach(oldScript => {
                    const newScript = document.createElement('script');
                    Array.from(oldScript.attributes).forEach(attr => newScript.setAttribute(attr.name, attr.value));
                    newScript.appendChild(document.createTextNode(oldScript.innerHTML));
                    oldScript.parentNode.replaceChild(newScript, oldScript);
                });

                // Re-run animations and UI logic
                requestAnimationFrame(() => {
                    oldMain.classList.remove('loading');
                    animatePageEntry();
                    updateActiveNav(window.location.pathname);
                    setupNavLinks(); 
                    
                    window.dispatchEvent(new CustomEvent('fastnav:success', { detail: { url } }));
                    
                    // --- CRITICAL: Re-initialize Blazor for swapped content ---
                    if (window.Blazor) {
                        console.log("Re-initializing Blazor...");
                        try {
                            if (typeof Blazor.disconnect === 'function') Blazor.disconnect();
                        } catch(e) {}
                        
                        setTimeout(() => {
                            try {
                                if (typeof Blazor.start === 'function') {
                                    Blazor.start().then(() => {
                                        console.log("Blazor re-started");
                                    }).catch(err => {
                                        // If already started, try reconnecting
                                        if (typeof Blazor.reconnect === 'function') {
                                            Blazor.reconnect().then(res => {
                                                if (!res) location.reload(); // Hard fallback if reconnection failed
                                            });
                                        }
                                    });
                                }
                            } catch (e) { console.warn("Blazor start error:", e); }
                        }, 150); // Increased delay for DOM stability
                    } else {
                        // Fallback: If Blazor script was in the new page but not in the old one
                        if (doc.querySelector('script[src*="blazor.server.js"]')) {
                            const bScript = document.createElement('script');
                            bScript.src = '_framework/blazor.server.js';
                            document.body.appendChild(bScript);
                        }
                    }

                    NavProgress.finish();
                });
            }, 50); 
            return true;
        } else {
            // If the layout is fundamentally different (missing .main-content), do a professional smooth reload
            if (pushState) {
                if (document.startViewTransition) {
                    document.startViewTransition(() => { window.location.href = url; });
                } else {
                    window.location.href = url;
                }
            }
            return false;
        }
    } catch (err) {
        console.error('Fast Navigation Error:', err);
        window.location.href = url;
    }
    return false;
}

// ─── Global Click Delegation (Navigation & UI) ────────────────────────────────
document.addEventListener('click', (e) => {
    // 1. Navigation Links (SPA Feel)
    const navLink = e.target.closest('a[href]:not([target="_blank"]):not([href^="http"]):not([href^="javascript"]):not([href^="#"]):not([href*="/Account/Logout"])');
    if (navLink && !e.metaKey && !e.ctrlKey && !e.shiftKey && !e.altKey && !navLink.hasAttribute('data-no-fast-nav')) {
        const href = navLink.getAttribute('href');
        
        // --- Professional SPA-Feedback for All Links ---
        // Instantly update active state in sidebar
        const sidebarNav = document.querySelector('.sidebar-nav');
        if (sidebarNav) {
            sidebarNav.querySelectorAll('.nav-item').forEach(n => n.classList.remove('active'));
            const navItem = navLink.closest('.nav-item');
            if (navItem) navItem.classList.add('active');
        }

        // Start progress bar even for full-page navigations
        NavProgress.start();

        // Close sidebar on mobile
        const sb = document.querySelector('.sidebar');
        if (window.innerWidth <= 768 && sb) sb.classList.remove('open');

        // Check if we should use Fast Navigation or Seamless Redirect
        if (isPathExcluded(href)) {
            // "Professional Redirect": Seamlessly navigate with a UI flash-guard
            const mainContent = document.querySelector('.main-content');
            if (mainContent) mainContent.classList.add('loading');
            
            // Use View Transition if supported for a high-end feel
            if (document.startViewTransition) {
                document.startViewTransition(() => {
                    window.location.href = href;
                });
            } else {
                window.location.href = href;
            }
            return;
        }

        e.preventDefault();
        fastNavigate(href);
        return;
    }

    // 2. Sidebar & UI Buttons
    const sidebarBtn = e.target.closest('#sidebarToggle');
    const closeBtn   = e.target.closest('#sidebarClose');
    const profileBtn = e.target.closest('#profileDropdownToggle');
    const profileMenu = document.getElementById('profileDropdown');
    const sb = document.querySelector('.sidebar');

    if (sidebarBtn) {
        if (window.innerWidth > 768) document.body.classList.toggle('sidebar-collapsed');
        else sb && sb.classList.add('open');
        return;
    }
    if (closeBtn) { sb && sb.classList.remove('open'); return; }
    if (profileBtn && profileMenu) { e.stopPropagation(); profileMenu.classList.toggle('show'); return; }
    if (profileMenu && profileMenu.classList.contains('show') && !profileMenu.contains(e.target)) {
        profileMenu.classList.remove('show');
    }
    if (window.innerWidth <= 768 && sb && sb.classList.contains('open')) {
        if (!sb.contains(e.target) && !e.target.closest('#sidebarToggle')) sb.classList.remove('open');
    }
});

// Expose cache clearing for Blazor/JS Interop
window.clearNavCache = async () => {
    prefetched.clear();
    if ('caches' in window) {
        try {
            const cache = await caches.open('sata-pos-v3');
            const keys = await cache.keys();
            // Clear all HTML pages from cache, keep assets (css, js, etc)
            for (const request of keys) {
                if (!request.url.match(/\.(css|js|png|jpg|jpeg|svg|ico|woff2?)$/)) {
                    await cache.delete(request);
                }
            }
            console.log('HTML Navigation cache cleared.');
        } catch (err) {
            console.warn('Cache clear failed:', err);
        }
    }
};

// ─── Form Interception (Fast Delete/Actions) ──────────────────────────────────
document.addEventListener('submit', async (e) => {
    const form = e.target;
    const btn = e.submitter || form.querySelector('button[type="submit"]');

    // CRITICAL: Skip Blazor-managed forms or forms without a real action
    // Blazor forms often have no action or action="" and are handled via WebSockets
    if (!form.action || form.action === window.location.href || form.action === window.location.href + '#') return;
    
    // Only intercept internal POST forms that aren't logout or external
    if (form.method.toLowerCase() !== 'post' || form.action.includes('/Account/Logout') || form.getAttribute('data-turbo') === 'false') return;

    // Skip if it looks like a Blazor form (contains builder helper or is in a blazor-managed area)
    if (form.querySelector('[name^="__"]')) return; 

    e.preventDefault();
    if (btn) btn.classList.add('processing');

    // Show progress bar
    NavProgress.start();
    
    try {
        const formData = new FormData(form);
        const response = await fetch(form.action, {
            method: 'POST',
            body: formData,
            headers: { 'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || '' }
        });

        if (response.ok) {
            // Success! Clear cache so subsequent navigations (like to Dashboard) show fresh data
            await window.clearNavCache();
            
            // Refresh the current page content
            await fastNavigate(window.location.pathname + window.location.search, false);
            showToast('Success', 'Action completed successfully.', 'success');
        } else {
            // If failed (e.g. validation error), fallback
            if (btn) btn.classList.remove('processing');
            form.submit();
        }
    } catch (err) {
        console.error('Form Submission Error:', err);
        if (btn) btn.classList.remove('processing');
        form.submit(); 
    } finally {
        NavProgress.finish();
    }
});

// ─── Toast Notifications (Premium Glassmorphism with Progress Bar) ──────────────────────────────
function showToast(title, message, type = 'info') {
    const container = document.getElementById('toast-container');
    if (!container) return;
    
    const icons = { info: 'ph-info', success: 'ph-check-circle', error: 'ph-warning-circle', warning: 'ph-warning' };
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    
    const progressColor = type === 'success' ? '#10b981' : type === 'error' ? '#ef4444' : '#BC1823';
    
    toast.innerHTML = `
        <div class="toast-icon"><i class="${icons[type] || icons.info}"></i></div>
        <div class="toast-content">
            <div class="toast-title">${title}</div>
            <div class="toast-message">${message}</div>
        </div>
        <button class="toast-close" onclick="this.parentElement.classList.remove('show'); setTimeout(() => this.parentElement.remove(), 400);">
            <i class="ph-x"></i>
        </button>
        <div class="toast-progress" style="background: ${progressColor};"></div>
    `;
    
    container.appendChild(toast);
    
    // Trigger animation
    requestAnimationFrame(() => {
        setTimeout(() => toast.classList.add('show'), 10);
    });
    
    // Auto-remove after 3 seconds
    setTimeout(() => {
        if (toast.parentNode) {
            toast.classList.remove('show');
            setTimeout(() => toast.remove(), 400);
        }
    }, 3000);
}

// ─── Page Entry Animation ──────────────────────────────────────────────────────
function animatePageEntry() {
    const main = document.querySelector('.main-content');
    if (!main) return;
    main.style.transform = 'translateY(6px)';
    main.style.transition = 'opacity 0.4s cubic-bezier(0.4, 0, 0.2, 1), transform 0.4s cubic-bezier(0.4, 0, 0.2, 1)';
    requestAnimationFrame(() => { main.style.opacity = '1'; main.style.transform = 'translateY(0)'; });
}

// ─── Active Nav ────────────────────────────────────────────────────────────────
function updateActiveNav(urlPath) {
    const sidebarNav = document.querySelector('.sidebar-nav');
    if (!sidebarNav) return;
    const navItems = Array.from(sidebarNav.querySelectorAll('.nav-item'));
    let bestMatch = null, maxLen = -1;
    navItems.forEach(el => {
        el.classList.remove('active');
        const href = el.getAttribute('href');
        if (!href) return;
        const hrefPath = new URL(href, window.location.origin).pathname.toLowerCase();
        const normUrl = urlPath.replace(/\/index$/, '') || '/';
        const normHref = hrefPath.replace(/\/index$/, '') || '/';
        if (normUrl === normHref || (normUrl.startsWith(normHref + '/') && normHref !== '/')) {
            if (normHref.length > maxLen) { maxLen = normHref.length; bestMatch = el; }
        }
    });
    if (bestMatch) bestMatch.classList.add('active');
}

// ─── Sidebar Nav: Prefetch on hover only ────────────
function setupNavLinks() {
    const container = document.querySelector('.main-content');
    const links = (container || document).querySelectorAll('a[href]:not([target="_blank"]):not([href^="http"]):not([href^="javascript"]):not([href^="#"]):not([href*="/Account/Logout"])');
    links.forEach(link => {
        link.addEventListener('mouseenter', () => prefetchPage(link.getAttribute('href')), { once: true, passive: true });
    });
}

// ─── Init ──────────────────────────────────────────────────────────────────────
window.addEventListener('DOMContentLoaded', () => {
    NavProgress.finish();
    updateActiveNav(window.location.pathname);
    setupNavLinks();
    animatePageEntry();

    window.addEventListener('popstate', () => {
        fastNavigate(window.location.pathname, false);
    });

    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register('/js/sw.js', { scope: '/' })
            .catch(err => console.warn('SW Register Error:', err));
    }
});

// ─── Report Utilities ──────────────────────────────────────────────────────────
function exportTableToCSV(tableId, filename) {
    const table = document.getElementById(tableId);
    if (!table) return;
    
    let csv = [];
    const rows = table.querySelectorAll("tr");
    
    for (let i = 0; i < rows.length; i++) {
        let row = [], cols = rows[i].querySelectorAll("td, th");
        
        for (let j = 0; j < cols.length; j++) {
            // Clean text and handle commas/quotes
            let data = cols[j].innerText.replace(/"/g, '""');
            row.push('"' + data + '"');
        }
        csv.push(row.join(","));
    }

    const csvFile = new Blob([csv.join("\n")], { type: "text/csv" });
    const downloadLink = document.createElement("a");
    downloadLink.download = filename;
    downloadLink.href = window.URL.createObjectURL(csvFile);
    downloadLink.style.display = "none";
    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
}

function escapeHtml(text) {
    if (text == null) return '';
    const d = document.createElement('div');
    d.textContent = String(text);
    return d.innerHTML;
}

/** Print report: opens system print dialog directly (no preview). Table + summary only. */
function printElement(elementId, reportTitle, storeInfo) {
    const element = document.getElementById(elementId);
    if (!element) return;

    const table = element.querySelector('table.premium-table, table.premium-table, .premium-table');
    if (!table || !table.querySelector('tbody tr')) {
        if (typeof showToast === 'function') {
            showToast('Print', 'Generate the report first — no data to print.', 'warning');
        }
        return;
    }

    const stats = [];
    element.querySelectorAll('.stat-card-premium').forEach((card) => {
        const label = card.querySelector('.label');
        const value = card.querySelector('.value');
        if (label && value) {
            stats.push({ label: label.innerText.trim(), value: value.innerText.trim() });
        }
    });

    const today = new Date().toLocaleString('en-GB', {
        day: '2-digit', month: 'short', year: 'numeric',
        hour: '2-digit', minute: '2-digit', hour12: false
    });

    const logoHtml = storeInfo?.logoUrl
        ? `<img src="${escapeHtml(storeInfo.logoUrl)}" alt="Logo" class="rp-logo" />`
        : '';

    let summaryHtml = '';
    if (stats.length) {
        summaryHtml = `
            <section class="rp-summary">
                <h3 class="rp-section-title">Summary</h3>
                <div class="rp-summary-grid">
                    ${stats.map((s) => `
                        <div class="rp-summary-item">
                            <span class="rp-sum-label">${escapeHtml(s.label)}</span>
                            <strong class="rp-sum-value">${escapeHtml(s.value)}</strong>
                        </div>`).join('')}
                </div>
            </section>`;
    }

    const existing = document.getElementById('report-print-root');
    if (existing) existing.remove();

    const printRoot = document.createElement('div');
    printRoot.id = 'report-print-root';
    printRoot.className = 'report-print-root';
    printRoot.innerHTML = `
        <article class="rp-document">
            <header class="rp-header">
                <!-- LEFT: Shop Info -->
                <div class="rp-shop-info">
                    <h1 class="rp-store">${escapeHtml(storeInfo?.shopName || 'E-Invoice System')}</h1>
                    ${storeInfo?.address ? `<p class="rp-meta-line"><span class="rp-meta-icon">📍</span> ${escapeHtml(storeInfo.address)}</p>` : ''}
                    ${storeInfo?.phone ? `<p class="rp-meta-line"><span class="rp-meta-icon">📞</span> ${escapeHtml(storeInfo.phone)}</p>` : ''}
                </div>
                <!-- CENTER: Logo -->
                <div class="rp-logo-center">
                    ${logoHtml}
                    <div class="rp-report-name">${escapeHtml(reportTitle)}</div>
                </div>
                <!-- RIGHT: Print Date -->
                <div class="rp-date-block">
                    <div class="rp-date-label">Printed On</div>
                    <div class="rp-date">${escapeHtml(today)}</div>
                </div>
            </header>
            ${summaryHtml}
            <section class="rp-table-section">
                <h3 class="rp-section-title">Report Details</h3>
                <div class="rp-table-wrap">${table.outerHTML}</div>
            </section>
            <footer class="rp-footer">E-Invoice System · Confidential business report</footer>
        </article>`;

    document.body.appendChild(printRoot);
    document.body.classList.add('printing-report');

    let finished = false;
    const cleanup = () => {
        if (finished) return;
        finished = true;
        window.removeEventListener('afterprint', cleanup);
        document.body.classList.remove('printing-report');
        if (printRoot.parentNode) printRoot.remove();
    };

    setTimeout(() => {
        window.addEventListener('afterprint', cleanup);
        window.print();
        setTimeout(cleanup, 3000);
    }, 250);
}

function renderReportChart(canvasId, data) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    // Cleanup existing chart
    const existingChart = Chart.getChart(ctx);
    if (existingChart) existingChart.destroy();

    const chartType = data.type || 'line';
    const isPie = chartType === 'pie' || chartType === 'doughnut';

    new Chart(ctx, {
        type: chartType,
        data: {
            labels: data.labels,
            datasets: [{
                label: data.label,
                data: data.values,
                borderColor: isPie ? '#fff' : '#BC1823',
                backgroundColor: isPie ? 
                    ['#BC1823', '#1e293b', '#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ec4899'] : 
                    (chartType === 'bar' ? '#BC1823' : 'rgba(188, 24, 35, 0.1)'),
                borderWidth: isPie ? 2 : 3,
                fill: !isPie && chartType !== 'bar',
                tension: 0.4,
                pointRadius: 4,
                pointHoverRadius: 6
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: isPie }
            },
            scales: isPie ? {} : {
                y: { beginAtZero: true, grid: { borderDash: [2, 4], color: '#f1f5f9' } },
                x: { grid: { display: false } }
            }
        }
    });
}

// Expose globally for Blazor JS Interop
window.showToast = showToast;
window.exportTableToCSV = exportTableToCSV;
window.printElement = printElement;
window.renderReportChart = renderReportChart;
