const CACHE = 'ecomeal-v1';
const STATIC_ASSETS = [
  '/',
  '/EcoMealIcon.png',
  '/EcoMeal.png',
  '/favicon.png',
  '/manifest.json',
  '/js/darkmode.js',
  '/js/qrcode.min.js',
  '/js/qrcode-interop.js',
];

self.addEventListener('install', (event) => {
  event.waitUntil(
    caches.open(CACHE).then((cache) => {
      return cache.addAll(STATIC_ASSETS).catch(() => {});
    })
  );
  self.skipWaiting();
});

self.addEventListener('activate', (event) => {
  event.waitUntil(
    caches.keys().then((keys) => {
      return Promise.all(
        keys.filter((k) => k !== CACHE).map((k) => caches.delete(k))
      );
    })
  );
  self.clients.claim();
});

self.addEventListener('fetch', (event) => {
  const { request } = event;

  if (request.mode === 'navigate') {
    event.respondWith(
      fetch(request).catch(() => caches.match('/')));
    return;
  }

  if (request.url.includes('/api/')) {
    event.respondWith(
      fetch(request).catch(() => Response.error()));
    return;
  }

  event.respondWith(
    caches.match(request).then((cached) => {
      return cached || fetch(request).then((response) => {
        if (response && response.status === 200) {
          const clone = response.clone();
          caches.open(CACHE).then((cache) => {
            cache.put(request, clone);
          });
        }
        return response;
      }).catch(() => Response.error());
    })
  );
});
