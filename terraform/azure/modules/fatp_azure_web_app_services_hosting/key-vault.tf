resource "azurerm_key_vault" "default" {
  name                        = "${replace(local.resource_prefix, "-", "")}-kv"
  location                    = local.azure_location
  resource_group_name         = local.resource_group.name
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
    object_id = azurerm_linux_web_app.default[0].identity[0].principal_id

    key_permissions = [
      "Get",
      "List"
    ]

    secret_permissions = [
      "Get",
      "List"
    ]
  }

  network_acls {
    bypass                     = "AzureServices"
    default_action             = "Deny"
    ip_rules                   = [azurerm_public_ip.nat_gateway[0].ip_address]
    virtual_network_subnet_ids = [azurerm_subnet.keyvault_subnet[0].id]
  }

  tags = local.tags

  lifecycle {
    ignore_changes = [
      access_policy,
    ]
  }
}

resource "azurerm_key_vault_secret" "fatpdbconnectionstring" {
  depends_on      = [azurerm_postgresql_flexible_server_database.default]
  name            = "ConnectionStrings--FatpDatabase"
  value           = "Server=${azurerm_postgresql_flexible_server.default.name}.postgres.database.azure.com;Database=${azurerm_postgresql_flexible_server_database.default.name};Port=5432;User Id=${azurerm_postgresql_flexible_server.default.administrator_login};Password=${azurerm_postgresql_flexible_server.default.administrator_password};Ssl Mode=Require;TrustServerCertificate=True;"
  key_vault_id    = azurerm_key_vault.default.id
  expiration_date = local.key_vault_year_from_now

  lifecycle {
    ignore_changes = [
      value,
    ]
  }
}

resource "azurerm_key_vault_secret" "fatpredisconnectionstring" {
  depends_on      = [azurerm_redis_cache.default]
  name            = "ConnectionStrings--FatpRedis"
  value           = azurerm_redis_cache.default.primary_connection_string
  key_vault_id    = azurerm_key_vault.default.id
  expiration_date = local.key_vault_year_from_now

  lifecycle {
    ignore_changes = [
      value,
    ]
  }
}

resource "azurerm_key_vault_secret" "govuknotifyapikey" {
  name            = "GovUkNotify--ApiKey"
  value           = var.govuk_notify_apikey
  key_vault_id    = azurerm_key_vault.default.id
  expiration_date = local.key_vault_year_from_now

  lifecycle {
    ignore_changes = [
      value,
    ]
  }
}

resource "azurerm_key_vault_secret" "blobstorageclientsecret" {
  name            = "BlobStorage--ClientSecret"
  value           = var.blob_storage_client_secret
  key_vault_id    = azurerm_key_vault.default.id
  expiration_date = local.key_vault_year_from_now

  lifecycle {
    ignore_changes = [
      value,
    ]
  }
}

resource "azurerm_monitor_diagnostic_setting" "default_key_vault" {
  count = local.enable_monitoring ? 1 : 0

  name               = "${local.resource_prefix}-default-kv-diag"
  target_resource_id = azurerm_key_vault.default.id

  log_analytics_workspace_id     = azurerm_log_analytics_workspace.web_app_service.id
  log_analytics_destination_type = "Dedicated"


  enabled_log {
    category = "AuditEvent"

    retention_policy {
      enabled = true
      days    = 7
    }
  }

  metric {
    category = "AllMetrics"

    retention_policy {
      enabled = true
      days    = 7
    }
  }
}