resource "azurerm_key_vault" "default" {
  name                        = "${local.resource_prefix}-kv"
  location                    = local.azure_location
  resource_group_name         = local.resource_group.name
  sku_name                    = "standard"
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = true
  enabled_for_disk_encryption = true

  network_acls {
    bypass                     = "AzureServices"
    default_action             = "Deny"
    ip_rules                   = [azurerm_public_ip.nat_gateway[0].ip_address, local.client_ip, ]
    virtual_network_subnet_ids = [azurerm_subnet.keyvault_subnet.id, ]
  }

  tags = local.tags

  lifecycle {
    ignore_changes = [
      access_policy,
      network_acls[0].ip_rules,
    ]
  }

}

// We have already created the access policy for the pipeline_service_account as parts of the azurerm_key_vault resource access_policy block.
// We must make sure that this is only added if we don't already have it because we have segregated it from the main azurerm_key_vault resource;
// otherwise, an error will be raised.
resource "azurerm_key_vault_access_policy" "pipeline_service_account" {
  count = data.external.check_existing_pipeline_service_account.result.exists == "false" ? 1 : 0

  key_vault_id = azurerm_key_vault.default.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azurerm_client_config.current.object_id

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

// We have already created the access policy for the fatp_web_app as parts of the azurerm_key_vault resource access_policy block.
// We must make sure that this is only added if we don't already have it because we have segregated it from the main azurerm_key_vault resource;
// otherwise, an error will be raised.
resource "azurerm_key_vault_access_policy" "fatp_web_app" {
  count = data.external.check_existing_fatp_web_app.result.exists == "false" ? 1 : 0

  key_vault_id = azurerm_key_vault.default.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_linux_web_app.default[0].identity[0].principal_id

  key_permissions = [
    "Get",
    "List",
  ]

  secret_permissions = [
    "Get",
    "List",
  ]
}

resource "azurerm_key_vault_access_policy" "fatp_function_app" {
  key_vault_id = azurerm_key_vault.default.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_linux_function_app.default.identity[0].principal_id

  key_permissions = [
    "Get",
    "List",
  ]

  secret_permissions = [
    "Get",
    "List",
  ]
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
      expiration_date,
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
      expiration_date,
    ]
  }

}

resource "azurerm_key_vault_secret" "govuknotifyapikey" {
  depends_on      = [azurerm_postgresql_flexible_server_database.default]
  name            = "GovUkNotify--ApiKey"
  value           = var.govuk_notify_apikey
  key_vault_id    = azurerm_key_vault.default.id
  expiration_date = local.key_vault_year_from_now

  lifecycle {
    ignore_changes = [
      value,
      expiration_date,
    ]
  }
}

resource "azurerm_key_vault_secret" "blobstorageclientsecret" {
  depends_on      = [azurerm_postgresql_flexible_server_database.default]
  name            = "BlobStorage--ClientSecret"
  value           = var.blob_storage_client_secret
  key_vault_id    = azurerm_key_vault.default.id
  expiration_date = local.key_vault_year_from_now

  lifecycle {
    ignore_changes = [
      value,
      expiration_date,
    ]
  }
}

resource "azurerm_key_vault_secret" "blobstorageenquiriesdataclientsecret" {
  depends_on      = [azurerm_postgresql_flexible_server_database.default]
  name            = "BlobStorageEnquiriesData--ClientSecret"
  value           = var.blob_storage_enquiries_data_client_secret
  key_vault_id    = azurerm_key_vault.default.id
  expiration_date = local.key_vault_year_from_now

  lifecycle {
    ignore_changes = [
      value,
      expiration_date,
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
    }
  }

  metric {
    category = "AllMetrics"

    retention_policy {
      enabled = true
    }
  }
}