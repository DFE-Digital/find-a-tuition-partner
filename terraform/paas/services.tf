data "cloudfoundry_service" "postgres" {
  name = "postgres"
}

resource "cloudfoundry_service_instance" "postgres_common" {
  name         = var.paas_database_common_name
  space        = data.cloudfoundry_space.space.id
  service_plan = data.cloudfoundry_service.postgres.service_plans[var.database_plan]
}