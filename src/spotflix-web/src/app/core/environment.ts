export const environment = {
  production: false,
  apiUrl: '/api',
  auth: {
    accessTokenStorageKey: 'sfx.access',
    refreshTokenStorageKey: 'sfx.refresh',
    tokenRefreshBufferSeconds: 60,
  },
  http: {
    timeout: 30000,
    retryCount: 1,
    retryDelayMs: 500,
  },
  notifications: {
    autoDismissMs: 4200,
  },
};
