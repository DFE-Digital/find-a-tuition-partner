locals {
  environment     = var.environment
  project_name    = "${var.project_name}-${local.environment}"
  service_name    = "${var.service_name}-${local.environment}"
  azure_location  = var.azure_location
  resource_prefix = local.service_name

  tags = {
    "Environment"      = var.environment,
    "Product"          = var.service_offering,
    "Service Offering" = var.service_offering
  }
}