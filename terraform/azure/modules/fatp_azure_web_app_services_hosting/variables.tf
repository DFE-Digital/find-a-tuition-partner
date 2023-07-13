variable "environment" {
  description = "Environment name. Will be used along with `project_name` as a prefix for all resources."
  type        = string
}

variable "project_name" {
  description = "Project name. Will be used along with `environment` as a prefix for the resource group."
  type        = string
}

variable "service_name" {
  description = "Service name. Will be used along with `environment` as a prefix for all resources."
  type        = string
}

variable "azure_location" {
  description = "Azure location in which to launch resources."
  type        = string
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
  default     = "Windows"

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
  default     = "v4.0"
}

variable "service_worker_count" {
  description = "The number of Workers for the App Service"
  type        = number
  default     = 1
}

variable "service_app_settings" {
  description = "Service app settings"
  type        = map(string)
  default     = {}
}

variable "service_health_check_path" {
  description = "Service health check path"
  type        = string
  default     = "/"
}

variable "service_health_check_eviction_time_in_min" {
  description = "The amount of time in minutes that a node can be unhealthy before being removed from the load balancer"
  type        = number
  default     = 5
}

variable "enable_service_logs" {
  description = "Enable service logs, stored in blob storage"
  type        = bool
  default     = true
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
  description = "A list of email addresses that should be notified by monitoring alerts"
  type        = list(string)
  default     = []
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
  default     = "/"
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

variable "enable_dns_zone" {
  description = "Conditionally create a DNS zone"
  type        = bool
  default     = false
}

variable "dns_zone_domain_name" {
  description = "DNS zone domain name. If created, records will automatically be created to point to the CDN."
  type        = string
  default     = ""
}

variable "dns_zone_soa_record" {
  description = "DNS zone SOA record block (https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_zone#soa_record)"
  type        = map(string)
  default     = {}
}

variable "dns_a_records" {
  description = "DNS A records to add to the DNS Zone"
  type = map(
    object({
      ttl : optional(number, 300),
      records : list(string)
    })
  )
  default = {}
}

variable "dns_alias_records" {
  description = "DNS ALIAS records to add to the DNS Zone"
  type = map(
    object({
      ttl : optional(number, 300),
      target_resource_id : string
    })
  )
  default = {}
}

variable "dns_aaaa_records" {
  description = "DNS AAAA records to add to the DNS Zone"
  type = map(
    object({
      ttl : optional(number, 300),
      records : list(string)
    })
  )
  default = {}
}

variable "dns_caa_records" {
  description = "DNS CAA records to add to the DNS Zone"
  type = map(
    object({
      ttl : optional(number, 300),
      records : list(
        object({
          flags : number,
          tag : string,
          value : string
        })
      )
    })
  )
  default = {}
}

variable "dns_cname_records" {
  description = "DNS CNAME records to add to the DNS Zone"
  type = map(
    object({
      ttl : optional(number, 300),
      record : string
    })
  )
  default = {}
}

variable "dns_mx_records" {
  description = "DNS MX records to add to the DNS Zone"
  type = map(
    object({
      ttl : optional(number, 300),
      records : list(
        object({
          preference : number,
          exchange : string
        })
      )
    })
  )
  default = {}
}

variable "dns_ns_records" {
  description = "DNS NS records to add to the DNS Zone"
  type = map(
    object({
      ttl : optional(number, 300),
      records : list(string)
    })
  )
  default = {}
}

variable "dns_ptr_records" {
  description = "DNS PTR records to add to the DNS Zone"
  type = map(
    object({
      ttl : optional(number, 300),
      records : list(string)
    })
  )
  default = {}
}

variable "dns_srv_records" {
  description = "DNS SRV records to add to the DNS Zone"
  type = map(
    object({
      ttl : optional(number, 300),
      records : list(
        object({
          priority : number,
          weight : number,
          port : number,
          target : string
        })
      )
    })
  )
  default = {}
}

variable "dns_txt_records" {
  description = "DNS TXT records to add to the DNS Zone"
  type = map(
    object({
      ttl : optional(number, 300),
      records : list(string)
    })
  )
  default = {}
}
