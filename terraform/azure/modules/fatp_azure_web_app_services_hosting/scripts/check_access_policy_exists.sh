#!/bin/bash

# Read input JSON
input=$(cat)

# Extract values from JSON input
KEYVAULT_NAME=$(echo $input | jq -r .keyvault_name)
PRINCIPAL_NAME=$(echo $input | jq -r .principal_name)

# Fetch the list of access policies for the Key Vault
policies=$(az keyvault show --name $KEYVAULT_NAME --query "properties.accessPolicies[?contains(objectId, '$PRINCIPAL_NAME')]" --output tsv)

# Check if the principal has an access policy and return a JSON response
if [ -z "$policies" ]; then
    echo '{"exists": "false"}'
else
    echo '{"exists": "true"}'
fi
