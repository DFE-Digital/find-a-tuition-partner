resource "azurerm_key_vault" "default" {
  depends_on                  = [module.fatp_azure_web_app_services_hosting]
  name                        = "${local.service_name}-${local.environment}-kv"
  location                    = local.azure_location
  resource_group_name         = module.fatp_azure_web_app_services_hosting.azurerm_resource_group_default.name
  sku_name                    = "standard"
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = true
  enabled_for_disk_encryption = true

  access_policy {
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
}

resource "azurerm_key_vault_secret" "fatpdbconnectionstring" {
  depends_on   = [azurerm_postgresql_flexible_server_database.default]
  name         = "ConnectionStrings--FatpDatabase"
  value        = "Server=${azurerm_postgresql_flexible_server.default.name}.postgres.database.azure.com;Database=${azurerm_postgresql_flexible_server_database.default.name};Port=5432;User Id=${azurerm_postgresql_flexible_server.default.administrator_login};Password=${azurerm_postgresql_flexible_server.default.administrator_password};Ssl Mode=Require;TrustServerCertificate=True;"
  key_vault_id = azurerm_key_vault.default.id
}

resource "azurerm_key_vault_secret" "fatpredisconnectionstring" {
  depends_on   = [azurerm_redis_cache.default]
  name         = "ConnectionStrings--FatpRedis"
  value        = azurerm_redis_cache.default.primary_connection_string
  key_vault_id = azurerm_key_vault.default.id
}

resource "azurerm_key_vault_secret" "govuknotifyapikey" {
  name         = "GovUkNotify--ApiKey"
  value        = var.govuk_notify_apikey
  key_vault_id = azurerm_key_vault.default.id
}

resource "azurerm_key_vault_secret" "blobstorageclientsecret" {
  name         = "BlobStorage--ClientSecret"
  value        = var.blob_storage_client_secret
  key_vault_id = azurerm_key_vault.default.id
}