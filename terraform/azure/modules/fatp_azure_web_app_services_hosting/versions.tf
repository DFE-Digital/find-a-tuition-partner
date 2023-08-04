terraform {
  required_version = "~> 1.4.5"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.48.0"
    }
    azapi = {
      source  = "Azure/azapi"
      version = "~> 1.4.0"
    }
    dns = {
      source  = "hashicorp/dns"
      version = "~> 3.2.4"
    }
    null = {
      source  = "hashicorp/null"
      version = "~> 3.2.1"
    }
  }
}
