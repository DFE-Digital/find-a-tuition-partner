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

resource "azurerm_storage_account_network_rules" "logs" {
  count = local.enable_service_logs ? 1 : 0

  storage_account_id         = azurerm_storage_account.logs[0].id
  default_action             = "Deny"
  bypass                     = ["AzureServices"]
  virtual_network_subnet_ids = [azurerm_subnet.web_app_service_infra_subnet[0].id]
  ip_rules                   = [azurerm_public_ip.nat_gateway[0].ip_address]

  private_link_access {
    endpoint_resource_id = local.service_app.id
  }
}

resource "azurerm_monitor_diagnostic_setting" "web_app" {
  count = local.enable_service_logs ? 1 : 0

  name                           = "${local.resource_prefix}-storage-diag"
  target_resource_id             = azurerm_storage_account.logs[0].id
  log_analytics_workspace_id     = azurerm_log_analytics_workspace.web_app_service.id
  log_analytics_destination_type = "Dedicated"

  metric {
    category = "Transaction"

    retention_policy {
      enabled = false
    }
  }
}
