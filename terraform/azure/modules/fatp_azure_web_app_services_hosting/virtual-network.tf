resource "azurerm_virtual_network" "default" {
  count = local.existing_virtual_network == "" ? (
    local.launch_in_vnet ? 1 : 0
  ) : 0

  name                = "${local.resource_prefix}-default"
  address_space       = [local.virtual_network_address_space]
  location            = local.resource_group.location
  resource_group_name = local.resource_group.name
  tags                = local.tags
}

resource "azurerm_route_table" "default" {
  count = local.launch_in_vnet ? 1 : 0

  name                          = "${local.resource_prefix}-default"
  location                      = local.resource_group.location
  resource_group_name           = local.resource_group.name
  disable_bgp_route_propagation = false
  tags                          = local.tags
}

resource "azurerm_subnet" "web_app_service_infra_subnet" {
  count = local.launch_in_vnet ? 1 : 0

  name                 = "${local.resource_prefix}-webappserviceinfra"
  virtual_network_name = local.virtual_network.name
  resource_group_name  = local.resource_group.name
  address_prefixes     = [local.web_app_service_infra_subnet_cidr]

  service_endpoints = ["Microsoft.Storage"]

  delegation {
    name = "delegation"

    service_delegation {
      name = "Microsoft.Web/serverFarms"
      actions = [
        "Microsoft.Network/virtualNetworks/subnets/action",
      ]
    }
  }
}

resource "azurerm_subnet_route_table_association" "web_app_service_infra_subnet" {
  count = local.launch_in_vnet ? 1 : 0

  subnet_id      = azurerm_subnet.web_app_service_infra_subnet[0].id
  route_table_id = azurerm_route_table.default[0].id
}

resource "azurerm_network_security_group" "web_app_service_infra_allow_frontdoor_inbound_only" {
  count = local.launch_in_vnet && local.restrict_web_app_service_to_cdn_inbound_only && local.enable_cdn_frontdoor ? 1 : 0

  name                = "${local.resource_prefix}-webappserviceinfransg"
  location            = local.resource_group.location
  resource_group_name = local.resource_group.name

  security_rule {
    name                       = "AllowFrontdoor"
    priority                   = 100
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "*"
    source_port_range          = "*"
    destination_port_range     = "443"
    source_address_prefix      = "AzureFrontDoor.Backend"
    destination_address_prefix = "${data.dns_a_record_set.web_app_service_ip_address.addrs[0]}/32"
  }

  tags = local.tags
}

resource "azurerm_subnet_network_security_group_association" "web_app_service_infra_allow_frontdoor_inbound_only" {
  count = local.launch_in_vnet && local.restrict_web_app_service_to_cdn_inbound_only && local.enable_cdn_frontdoor ? 1 : 0

  subnet_id                 = azurerm_subnet.web_app_service_infra_subnet[0].id
  network_security_group_id = azurerm_network_security_group.web_app_service_infra_allow_frontdoor_inbound_only[0].id
}

resource "azurerm_subnet" "redis_cache_subnet" {
  count = local.launch_in_vnet ? (
    local.redis_cache_sku == "Premium" ? 1 : 0
  ) : 0

  name                 = "${local.resource_prefix}rediscache"
  virtual_network_name = local.virtual_network.name
  resource_group_name  = local.resource_group.name
  address_prefixes     = [local.redis_cache_subnet_cidr]
}

resource "azurerm_subnet_route_table_association" "redis_cache_subnet" {
  count = local.launch_in_vnet ? (
    local.redis_cache_sku == "Premium" ? 1 : 0
  ) : 0

  subnet_id      = azurerm_subnet.redis_cache_subnet[0].id
  route_table_id = azurerm_route_table.default[0].id
}

resource "azurerm_subnet" "redis_cache_private_endpoint_subnet" {
  count = local.launch_in_vnet ? (
    local.redis_cache_sku == "Premium" ? 1 : 0
  ) : 0

  name                                      = "${local.resource_prefix}rediscacheprivateendpoint"
  virtual_network_name                      = local.virtual_network.name
  resource_group_name                       = local.resource_group.name
  address_prefixes                          = [local.redis_cache_private_endpoint_subnet_cidr]
  private_endpoint_network_policies_enabled = true
}

resource "azurerm_subnet_route_table_association" "redis_cache_private_endpoint_subnet" {
  count = local.launch_in_vnet ? (
    local.redis_cache_sku == "Premium" ? 1 : 0
  ) : 0

  subnet_id      = azurerm_subnet.redis_cache_private_endpoint_subnet[0].id
  route_table_id = azurerm_route_table.default[0].id
}

resource "azurerm_private_dns_zone" "redis_cache_private_link" {
  count = local.launch_in_vnet ? (
    local.redis_cache_sku == "Premium" ? 1 : 0
  ) : 0

  name                = "${local.resource_prefix}.redis.cache.windows.net"
  resource_group_name = local.resource_group.name
  tags                = local.tags
}

resource "azurerm_private_dns_zone_virtual_network_link" "redis_cache_private_link" {
  count = local.launch_in_vnet ? (
    local.redis_cache_sku == "Premium" ? 1 : 0
  ) : 0

  name                  = "${local.resource_prefix}rediscacheprivatelink"
  resource_group_name   = local.resource_group.name
  private_dns_zone_name = azurerm_private_dns_zone.redis_cache_private_link[0].name
  virtual_network_id    = local.virtual_network.id
  tags                  = local.tags
}

resource "azurerm_subnet" "postgresql_subnet" {
  count = local.launch_in_vnet && local.postgresql_network_connectivity_method == "private" ? 1 : 0

  name                                      = "${local.resource_prefix}postgresql"
  virtual_network_name                      = local.virtual_network.name
  resource_group_name                       = local.resource_group.name
  address_prefixes                          = [local.postgresql_subnet_cidr]
  private_endpoint_network_policies_enabled = true
  service_endpoints                         = ["Microsoft.Sql"]
  delegation {
    name = "fs"
    service_delegation {
      name = "Microsoft.DBforPostgreSQL/flexibleServers"
      actions = [
        "Microsoft.Network/virtualNetworks/subnets/join/action",
      ]
    }
  }
}

resource "azurerm_subnet_route_table_association" "postgresql_subnet" {
  count = local.launch_in_vnet && local.postgresql_network_connectivity_method == "private" ? 1 : 0

  subnet_id      = azurerm_subnet.postgresql_subnet[0].id
  route_table_id = azurerm_route_table.default[0].id
}

resource "azurerm_private_dns_zone" "postgresql_private_link" {
  count = local.launch_in_vnet && local.postgresql_network_connectivity_method == "private" ? 1 : 0

  name                = "${local.resource_prefix}.postgres.database.azure.com"
  resource_group_name = local.resource_group.name
  tags                = local.tags
}

resource "azurerm_private_dns_zone_virtual_network_link" "postgresql_private_link" {
  count = local.launch_in_vnet && local.postgresql_network_connectivity_method == "private" ? 1 : 0

  name                  = "${local.resource_prefix}pgsqlprivatelink"
  resource_group_name   = local.resource_group.name
  private_dns_zone_name = azurerm_private_dns_zone.postgresql_private_link[0].name
  virtual_network_id    = local.virtual_network.id
  tags                  = local.tags
}
