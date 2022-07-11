provider "cloudfoundry" {
  api_url  = var.api_url
  user     = var.username
  password = var.password
}

resource "null_resource" "bundle_web_assests" {
    for_each = local.map1
    provisioner "local-exec" {
        command = "cd ${path.module}/UI  &&  npm ci npm run build"
    }
}

terraform {

  required_providers {
    cloudfoundry = {
      source  = "cloudfoundry-community/cloudfoundry"
      version = "0.15.3"
    }


    statuscake = {
      source  = "StatusCakeDev/statuscake"
      version = "1.0.1"
    }
  }
}