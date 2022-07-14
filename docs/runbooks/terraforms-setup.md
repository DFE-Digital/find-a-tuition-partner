
# Terraforms Setup

## Prerequisits

The terrforms required  Installation of terraforms in your local machine. Below is the link to Install it Locally

https://www.terraform.io/downloads

Once you have installed it add Terraforms to your SYSTEM CONFIGURATION 

## Description

In Order to Run Terraform Config locally which will allow you to deploy your latest code directly to GPass run the follwoing steps.

```
cd ./terraform
```
This will  install the terraform config in your local machine


```

terraform init 

```
This Will Validate the config Note: You need to provide password and username for the Cloud Foundary Account you use

```

terraform validate 

```
This Will apply the terraform config and build and deploy the application on Given GPass Space .


```

terraform apply 

```