module "fatp_azure_web_app_services_hosting" {
  source = "./modules/fatp_azure_web_app_services_hosting"

  environment    = var.environment
  project_name   = var.project_name
  service_name   = var.service_name
  azure_location = var.azure_location

  tags = {
    "Environment"      = var.environment,
    "Product"          = var.service_offering,
    "Service Offering" = var.service_offering
  }

  launch_in_vnet                = var.launch_in_vnet
  existing_virtual_network      = var.existing_virtual_network
  existing_resource_group       = var.existing_resource_group
  virtual_network_address_space = var.virtual_network_address_space

  service_plan_sku      = var.service_plan_sku
  service_plan_os       = var.service_plan_os
  service_stack         = var.service_stack
  service_stack_version = var.service_stack_version
  service_worker_count  = var.service_worker_count
  service_app_settings = {
    "ASPNETCORE_ENVIRONMENT"                  = var.aspnetcore_environment,
    "BlobStorage_AccountName"                 = var.app_setting_blobStorage_accountName,
    "BlobStorage_ClientId"                    = var.app_setting_blobStorage_clientId,
    "BlobStorage_ContainerName"               = var.app_setting_blobStorage_containerName,
    "BlobStorage_TenantId"                    = var.app_setting_blobStorage_tenantId,
    "EmailSettings_AllSentToEnquirer"         = var.app_setting_emailSettings_allSentToEnquirer,
    "EmailSettings_AmalgamateResponses"       = var.app_setting_emailSettings_amalgamateResponses,
    "EmailSettings_MergeResponses"            = var.app_setting_emailSettings_mergeResponses,
    "FatpAzureKeyVaultName"                   = "${var.service_name}-${var.environment}-kv",
    "AppLogging_DefaultLogEventLevel"         = var.appLogging_defaultLogEventLevel,
    "AppLogging_OverrideLogEventLevel"        = var.appLogging_overrideLogEventLevel
  }

  # App secrets
  govuk_notify_apikey        = var.govuk_notify_apikey
  blob_storage_client_secret = var.blob_storage_client_secret

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
  monitor_email_receivers        = local.monitor_email_receivers
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

  # Postges and Redis stuff
  postgresql_database_version            = var.postgresql_database_version
  postgresql_server_admin_username       = var.postgresql_server_admin_username
  postgresql_server_admin_password       = var.postgresql_server_admin_password
  postgresql_storage_mb                  = var.postgresql_storage_mb
  postgresql_sku_name                    = var.postgresql_sku_name
  postgresql_ssl_enforcement_enabled     = var.postgresql_ssl_enforcement_enabled
  postgresql_backup_retention_days       = var.postgresql_backup_retention_days
  postgresql_availability_zone           = var.postgresql_availability_zone
  postgresql_enabled_extensions          = var.postgresql_enabled_extensions
  postgresql_collation                   = var.postgresql_collation
  postgresql_charset                     = var.postgresql_charset
  postgresql_network_connectivity_method = var.postgresql_network_connectivity_method
  postgresql_firewall_ipv4_allow         = var.postgresql_firewall_ipv4_allow
  redis_cache_capacity                   = var.redis_cache_capacity
  redis_cache_family                     = var.redis_cache_family
  redis_cache_sku                        = var.redis_cache_sku
  redis_cache_version                    = var.redis_cache_version
  redis_cache_patch_schedule_day         = var.redis_cache_patch_schedule_day
  redis_cache_patch_schedule_hour        = var.redis_cache_patch_schedule_hour
  redis_cache_firewall_ipv4_allow_list   = var.redis_cache_firewall_ipv4_allow_list
  key_vault_secret_expiry_years          = var.key_vault_secret_expiry_years
}