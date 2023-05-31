resource "azurerm_postgresql_flexible_server" "default" {
  name                   = "${local.service_name}-psqlflexibleserver"
  resource_group_name    = module.fatp_azure_web_app_services_hosting.azurerm_resource_group_default.name
  location               = local.azure_location
  version                = local.postgresql_database_version
  administrator_login    = local.postgresql_server_admin_username
  administrator_password = local.postgresql_server_admin_password

  storage_mb = local.postgresql_storage_mb

  sku_name = local.postgresql_sku_name

  backup_retention_days = var.postgresql_backup_retention_days
  tags                  = local.tags

  depends_on = [module.fatp_azure_web_app_services_hosting]
}

resource "azurerm_postgresql_flexible_server_configuration" "default" {
  name      = "azure.extensions"
  server_id = azurerm_postgresql_flexible_server.default.id
  value     = "CITEXT,UUID-OSSP"
}

resource "azurerm_postgresql_flexible_server_database" "default" {
  name      = "${local.service_name}-db"
  server_id = azurerm_postgresql_flexible_server.default.id
  collation = "en_US.utf8"
  charset   = "utf8"
}