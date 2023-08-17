resource "azurerm_resource_group" "default" {
  count = local.existing_resource_group == "" ? 1 : 0

  name     = local.project_name
  location = local.azure_location
  tags     = local.tags
}

resource "azurerm_management_lock" "resource_group" {
  name       = "resource-group-level"
  scope      = azurerm_resource_group.default[0].id
  lock_level = "CanNotDelete"
  notes      = "The resource group is not allowed to delete"
}