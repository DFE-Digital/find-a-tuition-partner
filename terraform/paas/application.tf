
resource "cloudfoundry_app" "national-tutoring-sandbox" {
  name         = var.paas_application_name
  command      = "cf push --strategy rolling"
  space        = data.cloudfoundry_space.space.id
  instances    = var.application_instances
  stopped      = var.application_stopped
  memory       = var.application_memory
  disk_quota   = var.application_disk

 
    routes {
    route = cloudfoundry_route.api_route_cloud.id
  }


  service_binding {
    service_instance = cloudfoundry_service_instance.postgres_common.id
  }

}