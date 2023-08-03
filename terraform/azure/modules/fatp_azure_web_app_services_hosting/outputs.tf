output "environment" {
  value = local.environment
}

output "azurerm_resource_group_default" {
  value       = local.existing_resource_group == "" ? azurerm_resource_group.default[0] : null
  description = "Default Azure Resource Group"
}

output "azurerm_log_analytics_workspace_web_app_service" {
  value       = azurerm_log_analytics_workspace.web_app_service
  description = "Web App Service Log Analytics Workspace"
}

output "azurerm_storage_account_logs" {
  value       = local.enable_service_logs ? azurerm_storage_account.logs[0] : null
  description = "Logs Storage Account"
}

output "azurerm_linux_web_app_default" {
  value       = azurerm_linux_web_app.default[0]
  description = "Default Azure Linux Web App"
}

output "azurerm_cdn_frontdoor_endpoint_endpoint" {
  value       = azurerm_cdn_frontdoor_endpoint.endpoint[0]
  description = "Front Door (standard/premium) Endpoint"
}

output "azurerm_public_ip_address" {
  value       = azurerm_public_ip.nat_gateway[0].ip_address
  description = "Get the public static IP"
}

output "client_ip" {
  value       = local.client_ip
  description = "The client IP address"
}

output "application_insights_instrumentation_key" {
  value       = azurerm_application_insights.web_app_service.instrumentation_key
  description = "App insights instrumentation key"
  sensitive   = true
}


