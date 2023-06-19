output "azurerm_resource_group_name" {
  value       = module.fatp_azure_web_app_services_hosting.azurerm_resource_group_default.name
  description = "Default Azure Resource Group"
}

output "azurerm_linux_web_app_name" {
  value       = module.fatp_azure_web_app_services_hosting.azurerm_linux_web_app_default.name
  description = "Default Azure Linux Web App"
}

output "azurerm_cdn_frontdoor_endpoint_host_name" {
  value       = module.fatp_azure_web_app_services_hosting.azurerm_cdn_frontdoor_endpoint_endpoint.host_name
  description = "Front Door (standard/premium) Endpoint"
}