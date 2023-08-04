resource "azurerm_public_ip" "nat_gateway" {
  count = local.launch_in_vnet && local.enable_nat_gateway ? 1 : 0

  name                = "${local.resource_prefix}-natgateway"
  location            = local.resource_group.location
  resource_group_name = local.resource_group.name
  allocation_method   = "Static"
  sku                 = "Standard"
  ip_version          = "IPv4"

  tags = local.tags
}

resource "azurerm_nat_gateway" "default" {
  count = local.launch_in_vnet && local.enable_nat_gateway ? 1 : 0

  name                = "${local.resource_prefix}-default"
  location            = local.resource_group.location
  resource_group_name = local.resource_group.name
  sku_name            = "Standard"

  tags = local.tags
}

resource "azurerm_nat_gateway_public_ip_association" "default" {
  count = local.launch_in_vnet && local.enable_nat_gateway ? 1 : 0

  nat_gateway_id       = azurerm_nat_gateway.default[0].id
  public_ip_address_id = azurerm_public_ip.nat_gateway[0].id
}

resource "azurerm_subnet_nat_gateway_association" "app_service" {
  count = local.launch_in_vnet && local.enable_nat_gateway ? 1 : 0

  nat_gateway_id = azurerm_nat_gateway.default[0].id
  subnet_id      = azurerm_subnet.web_app_service_infra_subnet[0].id
}
