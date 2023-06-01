module "fatp_azure_web_app_services_hosting" {
  source = "github.com/hasan3d/terraform-azurerm-web-app-services-hosting?ref=v0.1.1"

  environment    = local.environment
  project_name   = local.project_name
  service_name   = local.service_name
  azure_location = local.azure_location

  tags = local.tags

  launch_in_vnet                = local.launch_in_vnet
  existing_virtual_network      = var.existing_virtual_network
  existing_resource_group       = var.existing_resource_group
  virtual_network_address_space = var.virtual_network_address_space

  service_plan_sku      = var.service_plan_sku
  service_plan_os       = var.service_plan_os
  service_stack         = var.service_stack
  service_stack_version = var.service_stack_version
  service_worker_count  = var.service_worker_count
  service_app_settings  = {
    "ASPNETCORE_ENVIRONMENT"            = var.aspnetcore_environment,
    "BlobStorage_AccountName"           = var.blobStorage_accountName,
    "BlobStorage_ClientId"              = var.blobStorage_clientId,
    "BlobStorage_ContainerName"         = var.blobStorage_containerName,
    "BlobStorage_TenantId"              = var.blobStorage_tenantId,
    "EmailSettings_AllSentToEnquirer"   = var.emailSettings_allSentToEnquirer,
    "EmailSettings_AmalgamateResponses" = var.emailSettings_amalgamateResponses,
    "FatpAzureKeyVaultName"             = var.fatp_azure_key_vault_name
  }
  service_health_check_path                 = var.service_health_check_path
  service_health_check_eviction_time_in_min = var.service_health_check_eviction_time_in_min
  enable_service_logs                       = var.enable_service_logs
  service_log_storage_sas_start             = var.service_log_storage_sas_start
  service_log_storage_sas_expiry            = var.service_log_storage_sas_expiry
  service_log_level                         = var.service_log_level
  service_log_retention                     = var.service_log_retention

  # Monitoring is disabled by default. If enabled, the following metrics will be monitored:
  # CPU usage, Memory usage, Latency, HTTP regional availability
  enable_monitoring              = var.enable_monitoring
  monitor_email_receivers        = var.monitor_email_receivers
  monitor_endpoint_healthcheck   = var.monitor_endpoint_healthcheck
  monitor_enable_slack_webhook   = var.monitor_enable_slack_webhook
  monitor_slack_webhook_receiver = var.monitor_slack_webhook_receiver
  monitor_slack_channel          = var.monitor_slack_channel

  enable_cdn_frontdoor                            = var.enable_cdn_frontdoor
  restrict_web_app_service_to_cdn_inbound_only    = var.restrict_web_app_service_to_cdn_inbound_only
  cdn_frontdoor_sku                               = var.cdn_frontdoor_sku
  enable_cdn_frontdoor_health_probe               = var.enable_cdn_frontdoor_health_probe
  cdn_frontdoor_health_probe_interval             = var.cdn_frontdoor_health_probe_interval
  cdn_frontdoor_health_probe_path                 = var.cdn_frontdoor_health_probe_path
  cdn_frontdoor_response_timeout                  = var.cdn_frontdoor_response_timeout
  cdn_frontdoor_custom_domains                    = var.cdn_frontdoor_custom_domains
  cdn_frontdoor_host_redirects                    = var.cdn_frontdoor_host_redirects
  cdn_frontdoor_enable_rate_limiting              = var.cdn_frontdoor_enable_rate_limiting
  cdn_frontdoor_rate_limiting_duration_in_minutes = var.cdn_frontdoor_rate_limiting_duration_in_minutes
  cdn_frontdoor_rate_limiting_threshold           = var.cdn_frontdoor_rate_limiting_threshold
  cdn_frontdoor_rate_limiting_bypass_ip_list      = var.cdn_frontdoor_rate_limiting_bypass_ip_list
  cdn_frontdoor_waf_mode                          = var.cdn_frontdoor_waf_mode
  cdn_frontdoor_host_add_response_headers         = var.cdn_frontdoor_host_add_response_headers
  cdn_frontdoor_remove_response_headers           = var.cdn_frontdoor_remove_response_headers
}