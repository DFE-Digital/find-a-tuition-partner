data "azurerm_virtual_network" "existing_virtual_network" {
  count = local.existing_virtual_network == "" ? 0 : 1

  name                = local.existing_virtual_network
  resource_group_name = local.existing_resource_group
}

data "azurerm_resource_group" "existing_resource_group" {
  count = local.existing_resource_group == "" ? 0 : 1

  name = local.existing_resource_group
}

data "dns_a_record_set" "web_app_service_ip_address" {
  host = local.service_app.default_hostname
}

data "azurerm_client_config" "current" {}

data "http" "clientip" {
  url = "https://ifconfig.me/ip"
}

data "azurerm_storage_account_blob_container_sas" "logs" {
  for_each = local.enable_service_logs ? local.service_log_types : []

  connection_string = azurerm_storage_account.logs[0].primary_connection_string
  container_name    = azurerm_storage_container.logs[each.value].name
  https_only        = true

  start  = local.service_log_storage_sas_start != "" ? local.service_log_storage_sas_start : formatdate("YYYY-MM-DD'T'hh:mm:ssZ", timestamp())
  expiry = local.service_log_storage_sas_expiry != "" ? local.service_log_storage_sas_expiry : formatdate("YYYY-MM-DD'T'hh:mm:ssZ", timeadd(timestamp(), "+8760h")) # +12 months

  permissions {
    read   = true
    add    = true
    create = true
    write  = true
    delete = true
    list   = true
  }
}

data "external" "check_existing_pipeline_service_account" {
  program = ["./scripts/check_access_policy_exists.sh"]

  query = {
    keyvault_name  = azurerm_key_vault.default.name
    principal_name = data.azurerm_client_config.current.object_id
  }
}

data "external" "check_existing_fatp_web_app" {
  program = ["./scripts/check_access_policy_exists.sh"]

  query = {
    keyvault_name  = azurerm_key_vault.default.name
    principal_name = azurerm_linux_web_app.default[0].identity[0].principal_id
  }
}

