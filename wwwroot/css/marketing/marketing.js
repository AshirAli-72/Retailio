// ══════════════════════════════════════════
// RETAILIO Marketing Site — Script
// ══════════════════════════════════════════

// ── Theme Toggle ──
const themeToggle = document.getElementById('themeToggle');
const body = document.body;
let currentTheme = localStorage.getItem('mkt_theme') || 'light';
applyTheme(currentTheme);

themeToggle.addEventListener('click', () => {
    currentTheme = currentTheme === 'light' ? 'dark' : 'light';
    localStorage.setItem('mkt_theme', currentTheme);
    applyTheme(currentTheme);
});

function applyTheme(theme) {
    body.classList.toggle('dark-mode', theme === 'dark');
    const icon = themeToggle.querySelector('i');
    icon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
    themeToggle.title = theme === 'dark' ? 'Switch to Light Mode' : 'Switch to Dark Mode';
}

// ── Nav Toggle (mobile) ──
const navToggle = document.getElementById('navToggle');
const navMenu   = document.getElementById('navMenu');

navToggle.addEventListener('click', () => {
    navMenu.classList.toggle('active');
    const icon = navToggle.querySelector('i');
    icon.className = navMenu.classList.contains('active') ? 'fas fa-times' : 'fas fa-bars';
});

navMenu.querySelectorAll('a').forEach(link => {
    link.addEventListener('click', () => {
        navMenu.classList.remove('active');
        navToggle.querySelector('i').className = 'fas fa-bars';
    });
});

// ── Navbar scroll shadow ──
const navbar = document.getElementById('navbar');
window.addEventListener('scroll', () => {
    navbar.classList.toggle('scrolled', window.scrollY > 50);
});

// ── Active nav link ──
const sections = document.querySelectorAll('section[id]');
window.addEventListener('scroll', () => {
    let current = '';
    sections.forEach(sec => {
        if (window.pageYOffset >= sec.offsetTop - 100) current = sec.id;
    });
    navMenu.querySelectorAll('a').forEach(a => {
        a.classList.toggle('active', a.getAttribute('href') === '#' + current);
    });
});

// ── Animated counter ──
function animateCounter(el, target, duration = 2000) {
    const prefix = el.getAttribute('data-prefix') || '';
    const suffix = el.getAttribute('data-suffix') || '';
    let start = 0;
    const inc = target / (duration / 16);
    const timer = setInterval(() => {
        start += inc;
        if (start >= target) {
            el.textContent = prefix + target.toLocaleString() + suffix;
            clearInterval(timer);
        } else {
            el.textContent = prefix + Math.floor(start).toLocaleString() + suffix;
        }
    }, 16);
}

// ── Intersection Observer ──
const obs = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (!entry.isIntersecting) return;
        entry.target.classList.add('animate-in');
        if (entry.target.classList.contains('stat-number')) {
            animateCounter(entry.target, parseInt(entry.target.getAttribute('data-target') || '0'));
            obs.unobserve(entry.target);
        }
        if (entry.target.classList.contains('feature-card')) {
            entry.target.style.opacity = '1';
            entry.target.style.transform = 'translateY(0)';
        }
    });
}, { threshold: 0.2, rootMargin: '0px 0px -100px 0px' });

// ── Stats — try real API first ──
(async function loadStats() {
    document.querySelectorAll('.stat-number').forEach(el => el.textContent = '...');
    try {
        const res = await fetch('/api/stats', { signal: AbortSignal.timeout(3000) });
        if (res.ok) {
            const data = await res.json();
            document.querySelectorAll('.stat-number').forEach(el => {
                const key = el.getAttribute('data-key');
                if (key === 'users')        el.setAttribute('data-target', data.activeUsers   ?? el.getAttribute('data-target'));
                if (key === 'sales')        el.setAttribute('data-target', data.totalSales    ?? el.getAttribute('data-target'));
                if (key === 'satisfaction') el.setAttribute('data-target', data.satisfactionPct ?? el.getAttribute('data-target'));
            });
        }
    } catch (_) {}
    document.querySelectorAll('.stat-number').forEach(el => obs.observe(el));
})();

// ── Animate feature cards on scroll ──
document.querySelectorAll('.feature-card').forEach((card, i) => {
    card.style.opacity = '0';
    card.style.transform = 'translateY(30px)';
    card.style.transition = `all 0.6s ease ${i * 0.1}s`;
    obs.observe(card);
});

// ── Animate pricing cards ──
document.querySelectorAll('.pricing-card').forEach((card, i) => {
    card.style.opacity = '0';
    card.style.transform = 'translateY(30px)';
    const po = new IntersectionObserver(([entry]) => {
        if (!entry.isIntersecting) return;
        setTimeout(() => {
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
            card.style.transition = 'all 0.6s ease';
        }, i * 150);
        po.unobserve(card);
    }, { threshold: 0.2 });
    po.observe(card);
});

// ── Hero entrance animation ──
document.addEventListener('DOMContentLoaded', () => {
    const hc = document.querySelector('.hero-content');
    const hi = document.querySelector('.hero-image');
    if (hc) { hc.style.opacity='0'; hc.style.transform='translateY(30px)'; setTimeout(()=>{ hc.style.transition='all .8s ease'; hc.style.opacity='1'; hc.style.transform='translateY(0)'; },100); }
    if (hi) { hi.style.opacity='0'; hi.style.transform='translateX(30px)'; setTimeout(()=>{ hi.style.transition='all .8s ease .3s'; hi.style.opacity='1'; hi.style.transform='translateX(0)'; },100); }
});

// ── Contact Form ──
document.getElementById('contactForm')?.addEventListener('submit', e => {
    e.preventDefault();
    alert('Thank you for your message! We will get back to you soon.');
    e.target.reset();
});

// ── Smooth scroll ──
document.querySelectorAll('a[href^="#"]').forEach(a => {
    a.addEventListener('click', function(e) {
        const href = this.getAttribute('href');
        if (href === '#') return;
        const target = document.querySelector(href);
        if (target) { e.preventDefault(); window.scrollTo({ top: target.offsetTop - 80, behavior: 'smooth' }); }
    });
});

// ══════════════════════════════════════════
// FEATURE DETAIL MODAL
// ══════════════════════════════════════════
const featureData = {
    invoicing: {
        icon:'fas fa-receipt', title:'Smart Invoicing',
        tagline:'Create stunning professional invoices in seconds. Customizable templates, automated tax calculations, and instant PDF export.',
        highlights:[
            {icon:'fas fa-paint-brush', text:'Custom branded templates'},
            {icon:'fas fa-calculator',  text:'Auto tax & discount calc'},
            {icon:'fas fa-file-pdf',    text:'One-click PDF export'},
            {icon:'fas fa-paper-plane', text:'Email invoices directly'},
            {icon:'fas fa-history',     text:'Invoice history & search'},
            {icon:'fas fa-bell',        text:'Overdue payment alerts'},
        ]
    },
    inventory: {
        icon:'fas fa-boxes', title:'Inventory Management',
        tagline:'Stay on top of every item. Real-time stock tracking, barcode support, and automated reorder alerts.',
        highlights:[
            {icon:'fas fa-barcode',           text:'Barcode scanner support'},
            {icon:'fas fa-exclamation-triangle', text:'Low stock alerts'},
            {icon:'fas fa-layer-group',       text:'Multi-warehouse support'},
            {icon:'fas fa-tags',              text:'Product categories & tags'},
            {icon:'fas fa-sync-alt',          text:'Auto reorder triggers'},
            {icon:'fas fa-chart-bar',         text:'Stock movement reports'},
        ]
    },
    customers: {
        icon:'fas fa-users', title:'Customer Management',
        tagline:'Build lasting relationships with a complete 360° view of every customer — history, balances, notes, and contacts.',
        highlights:[
            {icon:'fas fa-address-card', text:'Complete customer profiles'},
            {icon:'fas fa-history',      text:'Full purchase history'},
            {icon:'fas fa-wallet',       text:'Credit balance tracking'},
            {icon:'fas fa-search',       text:'Advanced customer search'},
            {icon:'fas fa-star',         text:'Loyalty tier management'},
            {icon:'fas fa-file-export',  text:'Export customer lists'},
        ]
    },
    analytics: {
        icon:'fas fa-chart-line', title:'Real-time Analytics',
        tagline:'Make smarter decisions with live dashboards and beautiful reports — revenue, top products, team performance.',
        highlights:[
            {icon:'fas fa-tachometer-alt', text:'Live sales dashboard'},
            {icon:'fas fa-chart-pie',      text:'Revenue breakdown charts'},
            {icon:'fas fa-trophy',         text:'Top products & customers'},
            {icon:'fas fa-calendar-alt',   text:'Period-over-period trends'},
            {icon:'fas fa-user-clock',     text:'Employee performance'},
            {icon:'fas fa-download',       text:'Export to Excel / CSV'},
        ]
    },
    roles: {
        icon:'fas fa-user-shield', title:'User Roles & Permissions',
        tagline:'Grant exactly the right access. Admin, cashier, manager — define granular permissions with ease.',
        highlights:[
            {icon:'fas fa-key',            text:'Granular permission controls'},
            {icon:'fas fa-user-cog',       text:'Custom role creation'},
            {icon:'fas fa-eye-slash',      text:'Module-level visibility'},
            {icon:'fas fa-clipboard-list', text:'Full activity audit log'},
            {icon:'fas fa-lock',           text:'Secure login per user'},
            {icon:'fas fa-users-cog',      text:'Team management panel'},
        ]
    },
    credits: {
        icon:'fas fa-credit-card', title:'Credit & Refunds',
        tagline:'Handle returns, exchanges, and credits without the hassle. Keep customers happy while maintaining accurate records.',
        highlights:[
            {icon:'fas fa-undo',          text:'One-click refund processing'},
            {icon:'fas fa-exchange-alt',  text:'Product exchanges'},
            {icon:'fas fa-receipt',       text:'Credit note generation'},
            {icon:'fas fa-wallet',        text:'Store credit wallets'},
            {icon:'fas fa-history',       text:'Full refund audit trail'},
            {icon:'fas fa-percentage',    text:'Partial refund support'},
        ]
    },
    reports: {
        icon:'fas fa-file-alt', title:'Comprehensive Reports',
        tagline:'Every report you need — sales summaries, profit & loss, tax reports, employee productivity, and much more.',
        highlights:[
            {icon:'fas fa-file-invoice',   text:'Sales & revenue reports'},
            {icon:'fas fa-balance-scale',  text:'Profit & loss statement'},
            {icon:'fas fa-percentage',     text:'Tax summary & filing'},
            {icon:'fas fa-warehouse',      text:'Inventory valuation'},
            {icon:'fas fa-user-tie',       text:'Employee sales reports'},
            {icon:'fas fa-print',          text:'Scheduled auto-reports'},
        ]
    },
    currency: {
        icon:'fas fa-coins', title:'Multi-Currency Support',
        tagline:'Sell globally. Set exchange rates, display local currency prices, and reconcile across currencies easily.',
        highlights:[
            {icon:'fas fa-globe',                text:'Support for 150+ currencies'},
            {icon:'fas fa-sync-alt',             text:'Live exchange rate sync'},
            {icon:'fas fa-tag',                  text:'Per-customer currency'},
            {icon:'fas fa-file-invoice-dollar',  text:'Multi-currency invoices'},
            {icon:'fas fa-chart-bar',            text:'Converted reporting'},
            {icon:'fas fa-shield-alt',           text:'FX gain/loss tracking'},
        ]
    }
};

const featureModal    = document.getElementById('featureModal');
const closeFeatureBtn = document.getElementById('closeFeatureModal');
const pricingLink     = document.getElementById('featureModalPricingLink');

function openFeatureModal(key) {
    const d = featureData[key];
    if (!d) return;
    document.getElementById('featureModalIcon').innerHTML = `<i class="${d.icon}"></i>`;
    document.getElementById('featureModalTitle').textContent = d.title;
    document.getElementById('featureModalTagline').textContent = d.tagline;
    document.getElementById('featureModalHighlights').innerHTML = d.highlights.map(h =>
        `<div class="fmh-item"><i class="${h.icon}"></i><span>${h.text}</span></div>`
    ).join('');
    featureModal.classList.add('active');
    document.body.style.overflow = 'hidden';
}

document.querySelectorAll('.feature-card[data-feature]').forEach(card => {
    card.addEventListener('click', () => openFeatureModal(card.getAttribute('data-feature')));
});

closeFeatureBtn.addEventListener('click', () => {
    featureModal.classList.remove('active');
    document.body.style.overflow = '';
});

featureModal.addEventListener('click', e => {
    if (e.target === featureModal) { featureModal.classList.remove('active'); document.body.style.overflow = ''; }
});

pricingLink?.addEventListener('click', () => {
    featureModal.classList.remove('active');
    document.body.style.overflow = '';
});
