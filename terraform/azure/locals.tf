locals {
  environment                        = var.environment
  project_name                       = var.project_name
  service_name                       = var.service_name
  azure_location                     = var.azure_location
  launch_in_vnet                     = var.launch_in_vnet
  postgresql_database_version        = var.postgresql_database_version
  postgresql_server_admin_username   = var.postgresql_server_admin_username
  postgresql_server_admin_password   = var.postgresql_server_admin_password
  postgresql_storage_mb              = var.postgresql_storage_mb
  postgresql_sku_name                = var.postgresql_sku_name
  postgresql_ssl_enforcement_enabled = var.postgresql_ssl_enforcement_enabled
  postgresql_backup_retention_days   = var.postgresql_backup_retention_days
  redis_cache_capacity               = var.redis_cache_capacity
  redis_cache_family                 = var.redis_cache_family
  redis_cache_sku                    = var.redis_cache_sku
  redis_cache_version                = var.redis_cache_version
  redis_cache_patch_schedule_day     = var.redis_cache_patch_schedule_day
  redis_cache_patch_schedule_hour    = var.redis_cache_patch_schedule_hour

  tags = {
    "Environment"      = var.environment,
    "Product"          = var.service_offering,
    "Service Offering" = var.service_offering
  }
}