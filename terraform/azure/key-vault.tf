resource "azurerm_key_vault" "default" {
  name                        = "${local.service_name}-key-vault"
  location                    = local.azure_location
  resource_group_name         = module.fatp_azure_web_app_services_hosting.azurerm_resource_group_default.name
  sku_name                    = "standard"
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = true
  enabled_for_disk_encryption = true

  dynamic "access_policy" {
    for_each = data.azuread_user.key_vault_access

    content {
      tenant_id = data.azurerm_client_config.current.tenant_id
      object_id = data.azurerm_client_config.current.object_id

      key_permissions = [
        "Create",
        "Get",
      ]

      secret_permissions = [
        "Set",
        "Get",
        "Delete",
        "Purge",
        "Recover",
        "List",
      ]
    }
  }

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = module.fatp_azure_web_app_services_hosting.azurerm_linux_web_app_default[0].identity[0].principal_id

    key_permissions = [
      "Get",
      "List"
    ]

    secret_permissions = [
      "Get",
      "List"
    ]
  }

  tags = local.tags

  depends_on = [module.fatp_azure_web_app_services_hosting]
}

resource "azurerm_key_vault_secret" "fatpdbconnectionstring" {
  name            = "ConnectionStrings--FatpDatabase"
  value           = "Server=${azurerm_postgresql_flexible_server.default.name}.postgres.database.azure.com;Database=${azurerm_postgresql_flexible_server_database.default.name};Port=5432;User Id=${azurerm_postgresql_flexible_server.default.administrator_login};Password=${azurerm_postgresql_flexible_server.default.administrator_password};Ssl Mode=Require;TrustServerCertificate=True;"
  key_vault_id    = azurerm_key_vault.default.id
  depends_on = [azurerm_postgresql_flexible_server_database.default]
}

resource "azurerm_key_vault_secret" "fatpredisconnectionstring" {
  name            = "ConnectionStrings--FatpRedis"
  value           = azurerm_redis_cache.default.primary_connection_string
  key_vault_id    = azurerm_key_vault.default.id
  depends_on = [azurerm_redis_cache.default]
}