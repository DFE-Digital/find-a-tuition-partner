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

resource "cloudfoundry_app" "basic" {
	provider              = cloudfoundry
	name                  = "basic-buildpack"
	space_id              = data.cloudfoundry_space.myspace.id
	environment           = {MY_VAR = "1"}
	instances             = 2
	memory_in_mb          = 1024
	disk_in_mb            = 1024
	health_check_type     = "http"
	health_check_endpoint = "/"
}

resource "cloudfoundry_deployment" "basic" {
	provider   = cloudfoundry
	strategy   = "rolling"
	app_id     = cloudfoundry_app.basic.id
	droplet_id = cloudfoundry_droplet.basic.id
}

resource "cloudfoundry_droplet" "basic" {
	provider         = cloudfoundry
	app_id           = cloudfoundry_app.basic.id
	buildpacks       = ["binary_buildpack"]
	environment      = cloudfoundry_app.basic.environment
	command          = cloudfoundry_app.basic.command
	source_code_path = "/path/to/source.zip"
	source_code_hash = filemd5("/path/to/source.zip")
	depends_on = [
		cloudfoundry_service_binding.splunk,
		cloudfoundry_network_policy.basic,
	]
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