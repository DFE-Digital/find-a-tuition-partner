locals {
  environment                        = var.environment
  project_name                       = var.project_name
  service_name                       = var.service_name
  azure_location                     = var.azure_location
  postgresql_database_version        = var.postgresql_database_version
  postgresql_server_admin_username   = var.postgresql_server_admin_username
  postgresql_server_admin_password   = var.postgresql_server_admin_password
  postgresql_storage_mb              = var.postgresql_storage_mb
  postgresql_sku_name                = var.postgresql_sku_name
  postgresql_ssl_enforcement_enabled = var.postgresql_ssl_enforcement_enabled
  postgresql_backup_retention_days   = var.postgresql_backup_retention_days


  tags = {
    "Environment"      = var.environment,
    "Product"          = var.service_offering,
    "Service Offering" = var.service_offering
  }
}