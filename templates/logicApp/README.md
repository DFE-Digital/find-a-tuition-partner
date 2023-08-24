# Logic App Template Explanation

```markdown
# Logic App Template Explanation

This Markdown document explains the logic app template provided, which contains placeholders denoted by `{{variable}}` that need to be replaced with the correct values.

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "workflows_find_a_tp_enquiriesdatacsv_workflow_name": {
      "defaultValue": "find-a-tp-{{Environment}}-enquiriesdatacsv-workflow",
      "type": "String"
    },
    "connections_azureblob_externalid": {
      "defaultValue": "/subscriptions/{{subscriptionId}}/resourceGroups/{{resourceGroupName}}/providers/Microsoft.Web/connections/{{connectionName}}",
      "type": "String"
    },
    "connections_office365_externalid": {
      "defaultValue": "/subscriptions/{{subscriptionId}}/resourceGroups/{{resourceGroupName}}/providers/Microsoft.Web/connections/{{connectionName}}",
      "type": "String"
    },
    "connections_sharepointonline_externalid": {
      "defaultValue": "/subscriptions/{{subscriptionId}}/resourceGroups/{{resourceGroupName}}/providers/Microsoft.Web/connections/{{connectionName}}",
      "type": "String"
    }
  },
  "variables": {},
  "resources": [
    {
      // Resource details...
    }
  ]
}
```

## Explanation

- The template is written in JSON format and follows the Azure ARM template schema.
- The template includes parameter definitions for various placeholders that need to be replaced.

### Parameter Definitions Example

1. `workflows_find_a_tp_enquiriesdatacsv_workflow_name`: Represents the name of the Logic App workflow. It includes a placeholder `{{Environment}}` that should be replaced with the actual environment value.

2. `connections_azureblob_externalid`, `connections_office365_externalid`, and `connections_sharepointonline_externalid`: These parameters define connection strings for different services. They include placeholders for `{{subscriptionId}}`, `{{resourceGroupName}}`, and `{{connectionName}}` that need to be replaced with actual values.

---

This document provides an overview of the logic app template and the placeholders that need replacement. Ensure that you replace the placeholders with the appropriate values before deploying the logic app template.

Remember that this document serves as an explanation, and you should refer to the official Azure documentation or other relevant resources for complete instructions on deploying logic apps and working with ARM templates.
```