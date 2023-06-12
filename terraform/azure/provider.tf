terraform {
  required_version = "~>1.4.5"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.48.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~>2.37.1"
    }
  }
}

provider "azurerm" {
  features {}
}