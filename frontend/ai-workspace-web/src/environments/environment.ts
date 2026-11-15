export const environment = {
  production: false,

  api: {
    baseUrl: '/api',
    agents: '/agents',
    chat: '/agents/chat',
    onboardingChat: '/agents/onboarding/chat'
  },

  mcp: {
    baseUrl: 'http://localhost:5083'
  },

  application: {
    name: 'AI Banking Workspace',
    version: '1.0.0'
  },

  features: {
    aiAssistant: true,
    bankingAccounts: true,
    transfers: true,
    agents: true,
    mcpTools: true
  }
};