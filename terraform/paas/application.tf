resource "cloudfoundry_app" "find-a-tuition-partner-sandbox-test" {
  name         = var.paas_application_name
  space        = data.cloudfoundry_space.space.id
  instances    = var.application_instances
  stopped      = var.application_stopped
  memory       = var.application_memory
  docker_image = var.paas_app_docker_image
  disk_quota   = var.application_disk

 
    routes {
    route = cloudfoundry_route.api_route_cloud.id
  }


  service_binding {
    service_instance = cloudfoundry_service_instance.postgres_common.id
  }

}