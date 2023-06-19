resource "azurerm_redis_cache" "default" {

  name                = "${local.service_name}-${local.environment}-redis-cache"
  location            = local.azure_location
  resource_group_name = module.fatp_azure_web_app_services_hosting.azurerm_resource_group_default.name
  capacity            = local.redis_cache_capacity
  family              = local.redis_cache_family
  sku_name            = local.redis_cache_sku
  redis_version       = local.redis_cache_version
  enable_non_ssl_port = false
  minimum_tls_version = "1.2"
  public_network_access_enabled = local.launch_in_vnet ? (
    local.redis_cache_sku == "Premium" ? false : true
  ) : true

  redis_configuration {
    enable_authentication = true
  }

  patch_schedule {
    day_of_week    = local.redis_cache_patch_schedule_day
    start_hour_utc = local.redis_cache_patch_schedule_hour
  }

  tags = local.tags

  depends_on = [module.fatp_azure_web_app_services_hosting]
}