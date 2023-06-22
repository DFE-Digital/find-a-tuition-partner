locals {
  environment     = var.environment
  project_name    = "${var.project_name}-${local.environment}"
  service_name    = "${var.service_name}-${local.environment}"
  resource_prefix = local.service_name
  azure_location  = var.azure_location

  tags = var.tags

  launch_in_vnet                     = var.launch_in_vnet
  enable_nat_gateway                 = var.enable_nat_gateway
  existing_virtual_network           = var.existing_virtual_network
  existing_resource_group            = var.existing_resource_group
  virtual_network                    = local.existing_virtual_network == "" ? azurerm_virtual_network.default[0] : data.azurerm_virtual_network.existing_virtual_network[0]
  resource_group                     = local.existing_resource_group == "" ? azurerm_resource_group.default[0] : data.azurerm_resource_group.existing_resource_group[0]
  virtual_network_address_space      = var.virtual_network_address_space
  virtual_network_address_space_mask = element(split("/", local.virtual_network_address_space), 1)
  web_app_service_infra_subnet_cidr  = cidrsubnet(local.virtual_network_address_space, 23 - local.virtual_network_address_space_mask, 0)

  service_plan_sku      = var.service_plan_sku
  service_plan_os       = var.service_plan_os
  service_stack         = var.service_stack
  service_stack_version = var.service_stack_version
  service_app = local.service_plan_os == "Windows" ? azurerm_windows_web_app.default[0] : (
    local.service_plan_os == "Linux" ? azurerm_linux_web_app.default[0] : null
  )

  service_worker_count = var.service_worker_count

  service_app_settings = var.service_app_settings
  service_app_insights_settings = {
    "APPLICATIONINSIGHTS_CONNECTION_STRING"           = azurerm_application_insights.web_app_service.connection_string,
    "APPINSIGHTS_INSTRUMENTATIONKEY"                  = azurerm_application_insights.web_app_service.instrumentation_key,
    "APPINSIGHTS_PROFILERFEATURE_VERSION"             = "1.0.0",
    "APPINSIGHTS_SNAPSHOTFEATURE_VERSION"             = "1.0.0",
    "ApplicationInsightsAgent_EXTENSION_VERSION"      = "~2",
    "DiagnosticServices_EXTENSION_VERSION"            = "~3",
    "InstrumentationEngine_EXTENSION_VERSION"         = "disabled",
    "SnapshotDebugger_EXTENSION_VERSION"              = "disabled",
    "XDT_MicrosoftApplicationInsights_BaseExtensions" = "disabled",
    "XDT_MicrosoftApplicationInsights_Mode"           = "recommended",
    "XDT_MicrosoftApplicationInsights_PreemptSdk"     = "disabled",
    "XDT_MicrosoftApplicationInsights_Java"           = "1",
    "XDT_MicrosoftApplicationInsights_NodeJS"         = "1",
  }

  service_health_check_path                 = var.service_health_check_path
  service_health_check_eviction_time_in_min = var.service_health_check_eviction_time_in_min

  enable_service_logs            = var.enable_service_logs
  service_log_level              = var.service_log_level
  service_log_retention          = var.service_log_retention
  service_log_storage_sas_start  = var.service_log_storage_sas_start
  service_log_storage_sas_expiry = var.service_log_storage_sas_expiry
  service_log_types              = toset(["app", "http"])
  service_log_app_sas_url        = local.enable_service_logs ? "${azurerm_storage_account.logs[0].primary_blob_endpoint}${azurerm_storage_container.logs["app"].name}${data.azurerm_storage_account_blob_container_sas.logs["app"].sas}" : ""
  service_log_http_sas_url       = local.enable_service_logs ? "${azurerm_storage_account.logs[0].primary_blob_endpoint}${azurerm_storage_container.logs["http"].name}${data.azurerm_storage_account_blob_container_sas.logs["http"].sas}" : ""
  service_diagnostic_setting_types = toset([
    "AppServiceHTTPLogs",
    "AppServiceConsoleLogs",
    "AppServiceAppLogs",
    "AppServiceAuditLogs",
    "AppServiceIPSecAuditLogs",
    "AppServicePlatformLogs"
  ])

  enable_monitoring              = var.enable_monitoring
  monitor_email_receivers        = var.monitor_email_receivers
  monitor_enable_slack_webhook   = var.monitor_enable_slack_webhook
  monitor_slack_webhook_receiver = var.monitor_slack_webhook_receiver
  monitor_slack_channel          = var.monitor_slack_channel
  monitor_endpoint_healthcheck   = var.monitor_endpoint_healthcheck
  monitor_http_availability_fqdn = local.enable_cdn_frontdoor ? (
    length(local.cdn_frontdoor_custom_domains) >= 1 ? local.cdn_frontdoor_custom_domains[0] : azurerm_cdn_frontdoor_endpoint.endpoint[0].host_name
  ) : local.service_app.default_hostname
  monitor_http_availability_url = "https://${local.monitor_http_availability_fqdn}${local.monitor_endpoint_healthcheck}"

  enable_cdn_frontdoor                    = var.enable_cdn_frontdoor
  enable_cdn_frontdoor_health_probe       = var.enable_cdn_frontdoor_health_probe
  cdn_frontdoor_sku                       = var.cdn_frontdoor_sku
  cdn_frontdoor_health_probe_interval     = var.cdn_frontdoor_health_probe_interval
  cdn_frontdoor_health_probe_path         = var.cdn_frontdoor_health_probe_path
  cdn_frontdoor_health_probe_request_type = var.cdn_frontdoor_health_probe_request_type
  cdn_frontdoor_response_timeout          = var.cdn_frontdoor_response_timeout
  cdn_frontdoor_custom_domains            = var.cdn_frontdoor_custom_domains
  cdn_frontdoor_custom_domain_dns_names = local.enable_cdn_frontdoor && local.enable_dns_zone ? toset([
    for domain in local.cdn_frontdoor_custom_domains : replace(domain, local.dns_zone_domain_name, "") if endswith(domain, local.dns_zone_domain_name)
  ]) : []
  cdn_frontdoor_host_redirects                    = var.cdn_frontdoor_host_redirects
  cdn_frontdoor_enable_rate_limiting              = var.cdn_frontdoor_enable_rate_limiting
  cdn_frontdoor_rate_limiting_duration_in_minutes = var.cdn_frontdoor_rate_limiting_duration_in_minutes
  cdn_frontdoor_rate_limiting_threshold           = var.cdn_frontdoor_rate_limiting_threshold
  cdn_frontdoor_rate_limiting_bypass_ip_list      = var.cdn_frontdoor_rate_limiting_bypass_ip_list
  cdn_frontdoor_waf_mode                          = var.cdn_frontdoor_waf_mode
  cdn_frontdoor_host_add_response_headers         = var.cdn_frontdoor_host_add_response_headers
  cdn_frontdoor_remove_response_headers           = var.cdn_frontdoor_remove_response_headers
  cdn_frontdoor_origin_fqdn_override              = var.cdn_frontdoor_origin_fqdn_override != "" ? var.cdn_frontdoor_origin_fqdn_override : local.service_app.default_hostname

  ruleset_redirects_id               = length(local.cdn_frontdoor_host_redirects) > 0 ? [azurerm_cdn_frontdoor_rule_set.redirects[0].id] : []
  ruleset_add_response_headers_id    = length(local.cdn_frontdoor_host_add_response_headers) > 0 ? [azurerm_cdn_frontdoor_rule_set.add_response_headers[0].id] : []
  ruleset_remove_response_headers_id = length(local.cdn_frontdoor_remove_response_headers) > 0 ? [azurerm_cdn_frontdoor_rule_set.remove_response_headers[0].id] : []
  ruleset_ids = concat(
    local.ruleset_redirects_id,
    local.ruleset_add_response_headers_id,
    local.ruleset_remove_response_headers_id,
  )

  cdn_frontdoor_enable_waf = local.enable_cdn_frontdoor && local.cdn_frontdoor_enable_rate_limiting

  restrict_web_app_service_to_cdn_inbound_only = var.restrict_web_app_service_to_cdn_inbound_only

  enable_dns_zone      = var.enable_dns_zone
  dns_zone_domain_name = var.dns_zone_domain_name
  dns_zone_soa_record  = var.dns_zone_soa_record
  dns_a_records        = var.dns_a_records
  dns_alias_records    = var.dns_alias_records
  dns_aaaa_records     = var.dns_aaaa_records
  dns_caa_records      = var.dns_caa_records
  dns_cname_records    = var.dns_cname_records
  dns_mx_records       = var.dns_mx_records
  dns_ns_records       = var.dns_ns_records
  dns_ptr_records      = var.dns_ptr_records
  dns_srv_records      = var.dns_srv_records
  dns_txt_records      = var.dns_txt_records
}
