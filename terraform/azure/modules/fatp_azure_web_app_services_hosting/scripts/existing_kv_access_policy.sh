#!/bin/bash

# Read input JSON
input=$(cat)

# Extract value from JSON input
KEYVAULT_NAME=$(echo $input | jq -r .keyvault_name)

# Fetch the list of access policies for the Key Vault
policies=$(az keyvault show --name $KEYVAULT_NAME --query "properties.accessPolicies[*].objectId" --output tsv)

# Convert the list of objectIds to a JSON array
objectIds_json=$(echo "$policies" | jq -R . | jq -s .)

# Output the JSON array of objectIds
echo $objectIds_json
