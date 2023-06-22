resource "azurerm_storage_account" "logs" {
  count = local.enable_service_logs ? 1 : 0

  name                      = "${replace(local.service_name, "-", "")}logs"
  resource_group_name       = azurerm_resource_group.default[0].name
  location                  = azurerm_resource_group.default[0].location
  account_tier              = "Standard"
  account_kind              = "StorageV2"
  account_replication_type  = "LRS"
  min_tls_version           = "TLS1_2"
  enable_https_traffic_only = true

  tags = local.tags
}

resource "azurerm_storage_container" "logs" {
  for_each = local.enable_service_logs ? local.service_log_types : []

  name                  = "${local.service_name}${each.value}logs"
  storage_account_name  = azurerm_storage_account.logs[0].name
  container_access_type = "private"
}

data "azurerm_storage_account_blob_container_sas" "logs" {
  for_each = local.enable_service_logs ? local.service_log_types : []

  connection_string = azurerm_storage_account.logs[0].primary_connection_string
  container_name    = azurerm_storage_container.logs[each.value].name
  https_only        = true

  start  = local.service_log_storage_sas_start
  expiry = local.service_log_storage_sas_expiry

  permissions {
    read   = true
    add    = false
    create = false
    write  = true
    delete = true
    list   = true
  }
}
