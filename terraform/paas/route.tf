resource "cloudfoundry_route" "api_route_cloud" {
  domain   = data.cloudfoundry_domain.cloudapps.id
  hostname = var.paas_application_name
  space    = data.cloudfoundry_space.space.id

}