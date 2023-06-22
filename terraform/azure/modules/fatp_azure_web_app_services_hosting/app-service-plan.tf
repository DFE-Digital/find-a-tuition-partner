resource "azurerm_service_plan" "default" {
  name                = local.resource_prefix
  resource_group_name = local.resource_group.name
  location            = local.resource_group.location
  os_type             = local.service_plan_os
  sku_name            = local.service_plan_sku

  tags = local.tags
}
