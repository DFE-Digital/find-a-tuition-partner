resource "azurerm_windows_web_app" "default" {
  count = local.service_plan_os == "Windows" ? 1 : 0

  name                = local.resource_prefix
  resource_group_name = local.resource_group.name
  location            = local.resource_group.location
  service_plan_id     = azurerm_service_plan.default.id

  virtual_network_subnet_id = local.launch_in_vnet ? azurerm_subnet.web_app_service_infra_subnet[0].id : null
  https_only                = true
  app_settings = merge(
    local.service_app_settings,
    local.service_app_insights_settings,
  )

  site_config {
    always_on                         = true
    health_check_path                 = local.service_health_check_path
    health_check_eviction_time_in_min = local.service_health_check_eviction_time_in_min
    vnet_route_all_enabled            = true
    http2_enabled                     = true
    minimum_tls_version               = "1.2"
    worker_count                      = local.service_worker_count
    application_stack {
      current_stack       = local.service_stack
      dotnet_version      = local.service_stack == "dotnet" ? local.service_stack_version : null
      dotnet_core_version = local.service_stack == "dotnetcore" ? local.service_stack_version : null
      node_version        = local.service_stack == "node" ? local.service_stack_version : null
      python              = local.service_stack == "python"
      php_version         = local.service_stack == "php" ? local.service_stack_version : null
      java_version        = local.service_stack == "java" ? local.service_stack_version : null
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
  }

  dynamic "logs" {
    for_each = local.enable_service_logs ? [1] : []

    content {
      application_logs {
        azure_blob_storage {
          level             = local.service_log_level
          retention_in_days = local.service_log_retention
          sas_url           = local.service_log_app_sas_url
        }
        file_system_level = local.service_log_level
      }
      http_logs {
        azure_blob_storage {
          retention_in_days = local.service_log_retention
          sas_url           = local.service_log_http_sas_url
        }
      }
      detailed_error_messages = true
      failed_request_tracing  = true
    }
  }

  lifecycle {
    ignore_changes = [
      sticky_settings,
    ]
  }

  tags = local.tags
}
