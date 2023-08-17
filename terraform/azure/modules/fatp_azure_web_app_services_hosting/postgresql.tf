resource "azurerm_postgresql_flexible_server" "default" {
  name                   = "${local.resource_prefix}-psqlflexible-server"
  resource_group_name    = local.resource_group.name
  location               = local.azure_location
  version                = local.postgresql_database_version
  delegated_subnet_id    = local.postgresql_network_connectivity_method == "private" ? azurerm_subnet.postgresql_subnet[0].id : null
  private_dns_zone_id    = local.postgresql_network_connectivity_method == "private" ? azurerm_private_dns_zone.postgresql_private_link[0].id : null
  administrator_login    = local.postgresql_server_admin_username
  administrator_password = local.postgresql_server_admin_password
  zone                   = local.postgresql_availability_zone
  storage_mb             = local.postgresql_storage_mb
  sku_name               = local.postgresql_sku_name
  backup_retention_days  = var.postgresql_backup_retention_days
  tags                   = local.tags

  dynamic "high_availability" {
    for_each = local.postgresql_is_high_available ? [1] : []
    content {
      mode = "SameZone"
    }
  }

  lifecycle {
    ignore_changes = [
      # Azure will automatically assign an Availability Zone if one is not specified. 
      # If the PostgreSQL Flexible Server fails-over to the Standby Availability Zone,
      # the zone will be updated to reflect the current Primary Availability Zone.
      zone,
    ]
  }
}

resource "azurerm_postgresql_flexible_server_database" "default" {
  name      = "${local.resource_prefix}-db"
  server_id = azurerm_postgresql_flexible_server.default.id
  collation = local.postgresql_collation
  charset   = local.postgresql_charset
}

# https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/concepts-extensions?WT.mc_id=Portal-Microsoft_Azure_OSSDatabases#postgres-13-extensions
resource "azurerm_postgresql_flexible_server_configuration" "extensions" {
  count = local.postgresql_enabled_extensions != "" ? 1 : 0

  name      = "azure.extensions"
  server_id = azurerm_postgresql_flexible_server.default.id
  value     = local.postgresql_enabled_extensions
}

resource "azurerm_postgresql_flexible_server_firewall_rule" "web_app_default_static_ip" {
  count = local.postgresql_network_connectivity_method == "public" ? 1 : 0

  name             = "${local.resource_prefix}webapp"
  server_id        = azurerm_postgresql_flexible_server.default.id
  start_ip_address = azurerm_public_ip.nat_gateway[0].ip_address
  end_ip_address   = azurerm_public_ip.nat_gateway[0].ip_address
}

resource "azurerm_postgresql_flexible_server_firewall_rule" "firewall_rule" {
  for_each = local.postgresql_network_connectivity_method == "public" ? local.postgresql_firewall_ipv4_allow : {}

  name             = each.key
  server_id        = azurerm_postgresql_flexible_server.default.id
  start_ip_address = each.value.start_ip_address
  end_ip_address   = each.value.end_ip_address
}
