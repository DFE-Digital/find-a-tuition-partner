resource "azurerm_log_analytics_workspace" "web_app_service" {
  name                = "${local.resource_prefix}-webappservice"
  resource_group_name = local.resource_group.name
  location            = local.resource_group.location
  sku                 = "PerGB2018"
  retention_in_days   = 30
  tags                = local.tags
}

resource "azurerm_log_analytics_workspace" "function_app_service" {
  name                = "${local.resource_prefix}-functionappservice"
  resource_group_name = local.resource_group.name
  location            = local.resource_group.location
  sku                 = "PerGB2018"
  retention_in_days   = 30
  tags                = local.tags
}
