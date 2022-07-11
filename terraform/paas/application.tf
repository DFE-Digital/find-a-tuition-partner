
resource "cloudfoundry_app" "api_application" {
  name         = var.paas_application_name
  space        = data.cloudfoundry_space.space.id
  docker_image = var.paas_api_docker_image
  stopped      = var.application_stopped
  instances    = var.application_instances
  memory       = var.application_memory
  disk_quota   = var.application_disk
  strategy     = var.strategy

  routes {
    route = cloudfoundry_route.api_route_cloud.id
  }

  routes {
    route = cloudfoundry_route.api_route_internal.id
  }

  service_binding {
    service_instance = cloudfoundry_service_instance.redis.id
  }

  service_binding {
    service_instance = cloudfoundry_service_instance.postgres_common.id
  }

}