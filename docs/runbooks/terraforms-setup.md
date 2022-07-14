
# Terraforms Setup

## Description

In Order to Run Terraform Config locally which will allow you to deploy your latest code directly to GPass run the follwoing steps.

```
cd ./terraform
```
terraform init 

```

This will  install the terraform config in your local machine

```
terraform validate 
```


This Will Validate the config Note: You need to provide password and username for the Cloud Foundary Account you use

```
terraform apply 

```

This Will apply the terraform config and build and deploy the application on Given GPass Space .

```