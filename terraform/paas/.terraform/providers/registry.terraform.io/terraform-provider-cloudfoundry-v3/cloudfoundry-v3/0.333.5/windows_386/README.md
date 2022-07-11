# Cloud Foundry Terraform Provider (v3 API)

Experimental implementation of terraform resource for rolling deployments of
cloudfoundry applications using the v3 API.

Long-term intention is to contribute back
[upstream](https://github.com/cloudfoundry-community/terraform-provider-cloudfoundry).
This is the minimum viable
chunk to meet our immediate need.

If you have stumbled here, you most likely want
[the more complete provider from cloudfoundry-community](https://github.com/cloudfoundry-community/terraform-provider-cloudfoundry)

## Design goals of this experiment

* Enable use of the V3 "rolling" deployment features now available in cloudfoundry v3+ API
* Enable use of multiple buildpacks
* Enable use of sidecars (not yet implemented)
* Seperate the "application", "package", "build", "deployment" concepts into their own resources so that the provider resources map closer to the cloudfoundry API resources.
* Decouple the starting of applications from the creation of application resources so that external `depends_on` can be set for things like network policies.

## Usage

See the entry in the [Terraform Registry](https://registry.terraform.io/providers/terraform-provider-cloudfoundry-v3/cloudfoundry-v3/latest)

```hcl
terraform {
  required_providers {
    cloudfoundry-v3 = {
      source  = "terraform-provider-cloudfoundry-v3/cloudfoundry-v3"
      version = "0.333.2"
    }
  }
  required_version = ">= 0.13"
}

provider "cloudfoundry-v3" {
  api_url      = var.cf_api_url
  user         = var.cf_username
  password     = var.cf_password
}

resource "cloudfoundry_app" "basic" {
	provider              = cloudfoundry-v3
	name                  = "basic-buildpack"
	space_id              = data.cloudfoundry_space.myspace.id
	environment           = {MY_VAR = "1"}
	instances             = 2
	memory_in_mb          = 1024
	disk_in_mb            = 1024
	health_check_type     = "http"
	health_check_endpoint = "/"
}

resource "cloudfoundry_droplet" "basic" {
	provider         = cloudfoundry-v3
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

resource "cloudfoundry_deployment" "basic" {
	provider   = cloudfoundry-v3
	strategy   = "rolling"
	app_id     = cloudfoundry_app.basic.id
	droplet_id = cloudfoundry_droplet.basic.id
}
```

## Development / Releases

There were conserns that the org-wide permissions the terraform registery
requires for release were too broad, so the release process is bit funky...

* Merge to master in this repo trigger a sync to master on a repo outside the alphagov org [here](https://github.com/terraform-provider-cloudfoundry-v3/terraform-provider-cloudfoundry-v3)
* Creating a tag in this repo of the form `v0.333.X` will trigger a Github Action that performs the release: [see here](https://github.com/terraform-provider-cloudfoundry-v3/terraform-provider-cloudfoundry-v3/actions)
* The Terraform Registry entry will automatically get updated [here](https://registry.terraform.io/providers/terraform-provider-cloudfoundry-v3/cloudfoundry-v3/latest)

## Running tests

It's a bit of a hack right now, and you need a cloudfoundry deployment to test against..

Most of the env vars should be guessable, but for TEST_SERVICE_NAME /
TEST_SERVICE_PLAN_NAME: You will need to `cf push` the "fake broker" in
tests/ dir with the name `async-broker`,  and do `create-service-broker`
pointing to the pushed app's route and using admin/admin as creds ... then
get the service name and plan name from the `cf marketplace` or hitting
/v2/catalog on the broker app

Run:

```
TEST_ORG_NAME=test \
TEST_SPACE_NAME=test \
CF_API_URL=https://your.cf.env \
CF_USER=admin \
CF_PASSWORD=<secret> \
CF_UAA_CLIENT_ID=admin \
CF_UAA_CLIENT_SECRET=<secret> \
CF_CA_CERT="" \
CF_SKIP_SSL_VALIDATION=true \
TEST_DOMAIN_NAME=autom8.dev.cloudpipeline.digital \
TF_ACC=1 \
TEST_SERVICE_NAME=fake-service-ecf60c6f-b2ba-4ecf-9e4a-35683929f6b0 \
TEST_SERVICE_PLAN_NAME=fake-async-plan \
TEST_SERVICE_BROKER_NAME=async-broker \
go test -v -timeout 120m -gcflags="-e" ./cloudfoundryv3
```

