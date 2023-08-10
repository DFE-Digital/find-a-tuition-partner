resource "azurerm_linux_function_app" "default" {
  name                = "${local.resource_prefix}-fa"
  resource_group_name = local.resource_group.name
  location            = local.resource_group.location

  storage_account_name       = azurerm_storage_account.functionapplogs[0].name
  storage_account_access_key = azurerm_storage_account.functionapplogs[0].primary_access_key
  service_plan_id            = azurerm_service_plan.default.id
  virtual_network_subnet_id  = azurerm_subnet.function_app_service_infra_subnet[0].id
  https_only                 = true

  app_settings = merge(
    local.function_app_insights_settings,
    local.function_app_settings
  )

  site_config {
    always_on              = true
    vnet_route_all_enabled = true
    http2_enabled          = true
    minimum_tls_version    = "1.2"
    worker_count           = local.service_worker_count
  }

  application_stack {
    dotnet_version              = local.service_stack == "dotnet" ? local.service_stack_version : null
    use_dotnet_isolated_runtime = true
  }

  dynamic "ip_restriction" {
    for_each = local.restrict_web_app_service_to_cdn_inbound_only && local.enable_cdn_frontdoor ? [1] : []
    content {
      name   = "Allow CDN Traffic"
      action = "Allow"
      headers {
        x_azure_fdid = [
          azurerm_cdn_frontdoor_profile.cdn[0].resource_guid
        ]
      }
      service_tag = "AzureFrontDoor.Backend"
    }
  }

  identity {
    type = "SystemAssigned"
  }

  lifecycle {
    ignore_changes = [
      sticky_settings,
    ]
  }

  tags = local.tags
}