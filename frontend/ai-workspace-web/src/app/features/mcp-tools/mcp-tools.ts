import { Component } from '@angular/core';

interface McpTool {
  name: string;
  description: string;
  category: string;
  method: string;
}

@Component({
  selector: 'app-mcp-tools',
  standalone: true,
  templateUrl: './mcp-tools.html',
})
export class McpTools {
  readonly tools: McpTool[] = [
    {
      name: 'get_banking_capabilities',
      description:
        'Returns the banking operations supported by the workspace.',
      category: 'Banking Capabilities',
      method: 'MCP Tool',
    },
  ];

  readonly serverInformation = [
    {
      label: 'Protocol',
      value: 'Model Context Protocol',
    },
    {
      label: 'Server Project',
      value: 'AiWorkspace.McpServer',
    },
    {
      label: 'Development Port',
      value: '5083',
    },
    {
      label: 'Tool Discovery',
      value: 'tools/list',
    },
  ];
}