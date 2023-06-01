
# Terraform Environment Setup

## Description

The Terraform scripts in this repo are currently work in progress. They will eventually be used to provision the full find a tution partner service and all related applications and backing services. Currently they will provision the Postgres database backing service.

## Prerequisits

The Terrform scripts require the Terraform cli to be available on your path. The steps to do this are as follows:

1. Download the Amd64 Windows binary from [Terraform downloads](https://www.terraform.io/downloads)
2. Extract the terraform.exe file and copy to `C:\Program Files\Terraform`
3. Add `C:\Program Files\Terraform` to your system level environment variables
4. Restart your command prompt or terminal and run `terraform --version` to confirm installation

## Runbook

The Terraform scripts for GOV.UK PaaS are in the `terraform/paas` folder. All of the following commands should be run from that folder.

```
cd terraform/paas
```

Start by initialising terraform which will download and install the required provider plugins

```
terraform init
```

Confirm the scripts are valid

```
terraform validate
```

Audit the changes that will be applied by running the following command. Note at this point you will need to enter the appropriate GPaaS credentials

```
terraform plan
```

If all the changes look correct, run the following command to apply them and provision the environment

```
terraform apply
```