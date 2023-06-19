output "azurerm_resource_group_default" {
  value       = module.fatp_azure_web_app_services_hosting.azurerm_resource_group_default
  description = "Default Azure Resource Group"
}

output "azurerm_linux_web_app_default" {
  value       = module.fatp_azure_web_app_services_hosting.azurerm_linux_web_app_default
  description = "Default Azure Linux Web App"
}

output "azurerm_cdn_frontdoor_endpoint_endpoint" {
  value       = module.fatp_azure_web_app_services_hosting.azurerm_cdn_frontdoor_endpoint_endpoint
  description = "Front Door (standard/premium) Endpoint"
}