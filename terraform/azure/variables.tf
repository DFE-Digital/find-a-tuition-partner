variable "environment" {
  description = "Environment name. Will be used along with `project_name` as a prefix for all resources."
  type        = string
  default     = "dev"
}

variable "aspnetcore_environment" {
  description = "ASPNETCORE_ENVIRONMENT name."
  type        = string
  default     = "Development"
}

variable "project_name" {
  description = "Project name. Will be used along with `environment` as a prefix for all resources."
  type        = string
}

variable "service_name" {
  description = "Service name. Will be used along with `environment` as a prefix for all resources."
  type        = string
  default     = "find-a-tp"
}

variable "service_offering" {
  type    = string
  default = "National Tutoring Programme"
}

variable "azure_location" {
  description = "Azure location in which to launch resources."
  type        = string
  default     = "West Europe"
}

variable "tags" {
  description = "Tags to be applied to all resources"
  type        = map(string)
  default     = {}
}

variable "launch_in_vnet" {
  description = "Conditionally launch into a VNet"
  type        = bool
  default     = true
}

variable "enable_nat_gateway" {
  description = "Conditionally enable NAT Gateway. This is used to configure a static outbound IP. This is only avaiable when launching within a VNet."
  type        = bool
  default     = true
}

variable "existing_virtual_network" {
  description = "Conditionally use an existing virtual network. The `virtual_network_address_space` must match an existing address space in the VNet. This also requires the resource group name."
  type        = string
  default     = ""
}

variable "existing_resource_group" {
  description = "Conditionally launch resources into an existing resource group. Specifying this will NOT create a resource group."
  type        = string
  default     = ""
}

variable "virtual_network_address_space" {
  description = "Virtual Network address space CIDR"
  type        = string
  default     = "172.16.0.0/12"
}

variable "service_plan_sku" {
  description = "Service plan sku"
  type        = string
  default     = "S1"
}

variable "service_plan_os" {
  description = "Service plan operating system. Valid values are `Windows` or `Linux`."
  type        = string
  default     = "Linux"

  validation {
    condition     = contains(["Windows", "Linux"], var.service_plan_os)
    error_message = "`service_plan_os` must be either `Windows` or `Linux`."
  }
}

variable "service_stack" {
  description = "The application stack for the web app. Valid values are `dotnet`, `dotnetcore`, `node`, `python`, `php`, `java`, `ruby` or `go`."
  type        = string
  default     = "dotnet"

  validation {
    condition     = contains(["dotnet", "dotnetcore", "node", "python", "php", "java", "ruby", "go"], var.service_stack)
    error_message = "`service_stack` must be either `dotnet`, `dotnetcore`, `node`, `python`, `php`, `java`, `ruby` or `go`."
  }
}

variable "service_stack_version" {
  description = "The version of the chosen application stack"
  type        = string
  default     = "6.0"
}

variable "service_worker_count" {
  description = "The number of Workers for the App Service"
  type        = number
  default     = 3
}

variable "service_app_settings" {
  description = "Service app settings"
  type        = map(string)
  default     = {}
}

variable "function_app_settings" {
  description = "Function app settings"
  type        = map(string)
  default     = {}
}

variable "service_health_check_path" {
  description = "Service health check path"
  type        = string
  default     = "/health"
}

variable "service_health_check_eviction_time_in_min" {
  description = "The amount of time in minutes that a node can be unhealthy before being removed from the load balancer"
  type        = number
  default     = 5
}

variable "enable_service_logs" {
  description = "Enable service logs, stored in blob storage"
  type        = bool
  default     = false
}

variable "service_log_level" {
  description = "Service log level"
  type        = string
  default     = "Information"
}

variable "service_log_retention" {
  description = "Service log retention in days"
  type        = number
  default     = 30
}

variable "service_log_storage_sas_start" {
  description = "Service log sas token start date/time"
  type        = string
  default     = ""
}

variable "service_log_storage_sas_expiry" {
  description = "Service log sas token start date/time"
  type        = string
  default     = ""
}

variable "service_log_ipv4_allow_list" {
  description = "IPv4 addresses that are authorised to modify the Log storage account"
  type        = list(string)
  default     = []
}

variable "enable_monitoring" {
  description = "Create an App Insights instance and notification group for the Web App Service"
  type        = bool
  default     = false
}

variable "monitor_email_receivers" {
  description = "A pipe-separated string of email addresses that should be notified by monitoring alerts"
  type        = string
  default     = ""
}

variable "monitor_enable_slack_webhook" {
  description = "Enable slack webhooks to send monitoring notifications to a channel"
  type        = bool
  default     = false
}

variable "monitor_slack_webhook_receiver" {
  description = "A Slack App webhook URL"
  type        = string
  default     = ""
}

variable "monitor_slack_channel" {
  description = "Slack channel name/id to send messages to"
  type        = string
  default     = ""
}

variable "monitor_endpoint_healthcheck" {
  description = "Specify a route that should be monitored for a 200 OK status"
  type        = string
  default     = "/health"
}

variable "enable_cdn_frontdoor" {
  description = "Enable Azure CDN Front Door. This will use the Web App default hostname as the origin."
  type        = bool
  default     = false
}

variable "restrict_web_app_service_to_cdn_inbound_only" {
  description = "Restricts access to the Web App by addin an ip restriction rule which only allows 'AzureFrontDoor.Backend' inbound and matches the cdn fdid header. It also creates a network security group that only allows 'AzureFrontDoor.Backend' inbound, and attaches it to the subnet of the web app."
  type        = bool
  default     = true
}

variable "cdn_frontdoor_sku" {
  description = "Azure CDN Front Door SKU"
  type        = string
  default     = "Standard_AzureFrontDoor"
}

variable "enable_cdn_frontdoor_health_probe" {
  description = "Enable CDN Front Door health probe"
  type        = bool
  default     = true
}

variable "cdn_frontdoor_health_probe_interval" {
  description = "Specifies the number of seconds between health probes."
  type        = number
  default     = 30
}

variable "cdn_frontdoor_health_probe_path" {
  description = "Specifies the path relative to the origin that is used to determine the health of the origin."
  type        = string
  default     = "/"
}

variable "cdn_frontdoor_health_probe_request_type" {
  description = "Specifies the type of health probe request that is made."
  type        = string
  default     = "GET"
}

variable "cdn_frontdoor_response_timeout" {
  description = "Azure CDN Front Door response timeout in seconds"
  type        = number
  default     = 120
}

variable "cdn_frontdoor_custom_domains" {
  description = "Azure CDN Front Door custom domains. If they are within the DNS zone (optionally created), the Validation TXT records and ALIAS/CNAME records will be created"
  type        = list(string)
  default     = []
}

variable "cdn_frontdoor_host_redirects" {
  description = "CDN FrontDoor host redirects `[{ \"from\" = \"example.com\", \"to\" = \"www.example.com\" }]`"
  type        = list(map(string))
  default     = []
}

variable "cdn_frontdoor_enable_rate_limiting" {
  description = "Enable CDN Front Door Rate Limiting. This will create a WAF policy, and CDN security policy. For pricing reasons, there will only be one WAF policy created."
  type        = bool
  default     = false
}

variable "cdn_frontdoor_rate_limiting_duration_in_minutes" {
  description = "CDN Front Door rate limiting duration in minutes"
  type        = number
  default     = 1
}

variable "cdn_frontdoor_rate_limiting_threshold" {
  description = "Maximum number of concurrent requests before Rate Limiting policy is applied"
  type        = number
  default     = 300
}

variable "cdn_frontdoor_rate_limiting_bypass_ip_list" {
  description = "List if IP CIDRs to bypass CDN Front Door rate limiting"
  type        = list(string)
  default     = []
}

variable "cdn_frontdoor_waf_mode" {
  description = "CDN Front Door waf mode"
  type        = string
  default     = "Prevention"
}

variable "cdn_frontdoor_host_add_response_headers" {
  description = "List of response headers to add at the CDN Front Door `[{ \"Name\" = \"Strict-Transport-Security\", \"value\" = \"max-age=31536000\" }]`"
  type        = list(map(string))
  default     = []
}

variable "cdn_frontdoor_remove_response_headers" {
  description = "List of response headers to remove at the CDN Front Door"
  type        = list(string)
  default     = []
}

variable "cdn_frontdoor_origin_fqdn_override" {
  description = "Manually specify the hostname that the CDN Front Door should target. Defaults to the App Service hostname"
  type        = string
  default     = ""
}

variable "postgresql_server_admin_username" {
  type        = string
  description = "Specify a login that will be assigned to the administrator when creating the Postgres server"
  default     = ""
}

variable "postgresql_server_admin_password" {
  type        = string
  description = "Specify a password that will be assigned to the administrator when creating the Postgres server"
  default     = ""
}

variable "postgresql_sku_name" {
  type        = string
  description = "Specify the SKU to be used for the Postgres server"
  default     = "B_Standard_B1ms"
}

variable "postgresql_storage_mb" {
  type        = number
  description = "Specify the max amount of storage allowed for the Postgres server"
  default     = 32768
}

variable "postgresql_backup_retention_days" {
  type    = number
  default = 35
}

variable "postgresql_ssl_enforcement_enabled" {
  type    = bool
  default = true
}

variable "postgresql_database_version" {
  type        = string
  description = "Specify the version of postgres server to run (either 11,12,13 or 14)"
  default     = 14
}

variable "postgresql_availability_zone" {
  type        = string
  description = "Specify the availibility zone in which the Postgres server should be located"
  default     = "1"
}

variable "postgresql_enabled_extensions" {
  type        = string
  description = "Specify a comma seperated list of Postgres extensions to enable. See https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/concepts-extensions#postgres-14-extensions"
  default     = "CITEXT,UUID-OSSP"
}

variable "postgresql_collation" {
  type        = string
  description = "Specify the collation to be used for the Postgres database"
  default     = "en_US.utf8"
}

variable "postgresql_charset" {
  type        = string
  description = "Specify the charset to be used for the Postgres database"
  default     = "utf8"
}

variable "postgresql_network_connectivity_method" {
  type        = string
  description = "Specify postgresql networking method, public or private. See https://learn.microsoft.com/en-gb/azure/postgresql/flexible-server/concepts-networking"
  default     = "public"
  validation {
    condition     = contains(["public", "private"], var.postgresql_network_connectivity_method)
    error_message = "Valid values for postgresql_network_connectivity_method are public or private."
  }
}

variable "postgresql_firewall_ipv4_allow" {
  type = map(object({
    start_ip_address = string
    end_ip_address   = string
  }))
  description = "Map of IP address ranges to add into the postgres firewall. Note: only applicable if postgresql_network_connectivity_method is set to public."
  default     = {}
}

variable "redis_cache_capacity" {
  type    = number
  default = 0 // 250 MB for Basic SKU
}

variable "redis_cache_family" {
  type    = string
  default = "C"
}

variable "redis_cache_sku" {
  type    = string
  default = "Basic"
}

variable "redis_cache_version" {
  type    = number
  default = 6
}

variable "redis_cache_patch_schedule_day" {
  description = "Redis Cache patch schedule day"
  type        = string
  default     = "Sunday"
}

variable "redis_cache_patch_schedule_hour" {
  description = "Redis Cache patch schedule hour"
  type        = number
  default     = 23
}

variable "redis_cache_firewall_ipv4_allow_list" {
  description = "A list of IPv4 address that require remote access to the Redis server"
  type        = list(string)
  default     = []
}

variable "key_vault_secret_expiry_years" {
  description = "Number of years from now when the Key Vault secret should be considered expired"
  type        = number
  default     = 5
}

variable "app_setting_blobStorage_clientId" {
  type    = string
  default = "fdd09510-77b0-419f-b67c-2b5a25a073f0"
}

variable "blob_storage_client_secret" {
  type = string
}

variable "app_setting_blobStorage_containerName" {
  type    = string
  default = "ntp-tp-data"
}

variable "app_setting_blobStorage_accountName" {
  type    = string
  default = "s177p01sharedtpdata"
}

variable "app_setting_blobStorage_tenantId" {
  type    = string
  default = "9c7d9dd3-840c-4b3f-818e-552865082e16"
}

variable "app_setting_emailSettings_allSentToEnquirer" {
  type    = bool
  default = true
}

variable "app_setting_emailSettings_overrideAddress" {
  type    = string
  default = ""
}

variable "app_setting_emailSettings_minsDelaySendingOutcomeEmailToTP" {
  type    = number
  default = 3
}

variable "app_setting_emailSettings_mergeResponses" {
  type    = bool
  default = true
}

variable "app_setting_featureFlags_enquiryBuilder" {
  type    = bool
  default = true
}

variable "app_setting_featureFlags_sendEmailsFromNtp" {
  type    = bool
  default = true
}

variable "app_setting_featureFlags_verifyEmail" {
  type    = bool
  default = true
}

variable "app_setting_featureFlags_sendTuitionPartnerEmailsWhenEnquirerDelivered" {
  type    = bool
  default = true
}

variable "app_setting_serviceUnavailableSettings_startDateTime" {
  type        = string
  description = "#dd/MM/yyyy HH:mm:ss e.g. '26/07/2023 17:00:00' - Not required, but if supplied then the current datetime must be after this for the site to be unavailable."
  default     = ""
}

variable "app_setting_serviceUnavailableSettings_endDateTime" {
  type        = string
  description = "#dd/MM/yyyy HH:mm:ss e.g. '26/07/2023 18:00:00' - To set the site to unavailable then this must be supplied as a future date/time and the site will be unavailable until then."
  default     = ""
}

variable "app_setting_googleTagManager_containerId" {
  type    = string
  default = ""
}

variable "app_setting_googleAnalytics_measurementId" {
  type    = string
  default = ""
}

variable "govuk_notify_apikey" {
  type = string
}

variable "appLogging_defaultLogEventLevel" {
  type    = string
  default = "Debug"
}

variable "appLogging_overrideLogEventLevel" {
  type    = string
  default = "Information"
}

variable "dfeAnalytics_credentialsJsonFile" {
  type    = string
  default = "dfe-analytics-credentials.json"
}

variable "dfeAnalytics_projectId" {
  type    = string
  default = "find-a-tuition-partner"
}

variable "dfeAnalytics_datasetId" {
  type    = string
  default = ""
}

variable "fa_app_setting_dataExtraction_timer_cron_expression" {
  type    = string
  default = "1 1 * * *"
}

variable "app_setting_blobStorage_enquiries_data_clientId" {
  type    = string
  default = "10e39e46-b948-4cd9-ac27-90dc6e4bd472"
}

variable "blob_storage_enquiries_data_client_secret" {
  type = string
}

variable "app_setting_blobStorage_enquiries_data_containerName" {
  type    = string
  default = "ntp-enquiries-data"
}

variable "app_setting_blobStorage_enquiries_data_accountName" {
  type    = string
  default = "s177p01enquiriesdata"
}

variable "app_setting_blobStorage_enquiries_data_tenantId" {
  type    = string
  default = "9c7d9dd3-840c-4b3f-818e-552865082e16"
}
