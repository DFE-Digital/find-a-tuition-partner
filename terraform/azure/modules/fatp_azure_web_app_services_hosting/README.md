# Azure Web App Service Hosting terraform module

[![Terraform CI](./actions/workflows/continuous-integration-terraform.yml/badge.svg?branch=main)](./actions/workflows/continuous-integration-terraform.yml?branch=main)
[![GitHub release](./releases)](./releases)

This module creates and manages [Azure Web App Services][1], deployed within an [Azure Virtual Network][2].

## Usage

Example module usage:

```hcl
module "azure_web_app_services_hosting" {
  source  = "github.com/DFE-Digital/terraform-azurerm-web-app-services-hosting?ref=v0.1.2"

  environment    = "dev"
  project_name   = "myproject"
  azure_location = "uksouth"

  tags = {
    "Foo" = "bar",
  }

  launch_in_vnet                = true
  existing_virtual_network      = "vnet-id" # setting this will launch resources into this existing virtual network, rather than creating a new one.
  existing_resource_group       = "resource-id" # setting this will launch resources into this existing resource group, rather than creating a new one.
  virtual_network_address_space = "172.16.0.0/12"

  service_plan_sku      = "S1"
  service_plan_os       = "Windows"
  service_stack         = "dotnet"
  service_stack_version = "v4.0"
  service_worker_count  = 1
  service_app_settings  = {
    "Foo" = "Bar"
  }
  service_health_check_path                 = "/"
  service_health_check_eviction_time_in_min = 5
  enable_service_logs                       = true
  service_log_level                         = "Informational"
  service_log_retention                     = 30
  service_log_storage_sas_start             = "2023-03-22T00:00:00Z"
  service_log_storage_sas_expiry            = "2024-03-22T00:00:00Z"

  # Monitoring is disabled by default. If enabled, the following metrics will be monitored:
  # CPU usage, Memory usage, Latency, HTTP regional availability
  enable_monitoring              = true
  monitor_email_receivers        = [ "list@email.com" ]
  monitor_endpoint_healthcheck   = "/"
  monitor_enable_slack_webhook   = true
  monitor_slack_webhook_receiver = "https://hooks.slack.com/services/xxx/xxx/xxx"
  monitor_slack_channel          = "channel-name-or-id"

  enable_dns_zone      = true
  dns_zone_domain_name = "example.com"
  dns_zone_soa_record  = {
    email         = "hello.example.com"
    host_name     = "ns1-03.azure-dns.com."
    expire_time   = "2419200"
    minimum_ttl   = "300"
    refresh_time  = "3600"
    retry_time    = "300"
    serial_number = "1"
    ttl           = "3600"
  }
  dns_a_records = {
    "example" = {
      ttl = 300,
      records = [
        "1.2.3.4",
        "5.6.7.8",
      ]
    }
  }
  dns_alias_records = {
    "alias-example" = {
      ttl = 300,
      target_resource_id = "azure_resource_id",
    }
  }
  dns_aaaa_records = {
    "aaaa-example" = {
      ttl = 300,
      records = [
        "2001:db8::1:0:0:1",
        "2606:2800:220:1:248:1893:25c8:1946",
      ]
    }
  }
  dns_caa_records = {
    "caa-example" = {
      ttl = 300,
      records = [
        {
          flags = 0,
          tag   = "issue",
          value = "example.com"
        },
        {
          flags = 0
          tag   = "issuewild"
          value = ";"
        },
        {
          flags = 0
          tag   = "iodef"
          value = "mailto:caa@example.com"
        }
      ]
    }
  }
  dns_cname_records = {
    "cname-example" = {
      ttl    = 300,
      record = "example.com",
    }
  }
  dns_mx_records = {
    "mx-example" = {
      ttl = 300,
      records = [
        {
          preference = 10,
          exchange   = "mail.example.com"
        }
      ]
    }
  }
  dns_ns_records = {
    "ns-example" = {
      ttl = 300,
      records = [
        "ns-1.net",
        "ns-1.com",
        "ns-1.org",
        "ns-1.info"
      ]
    }
  }
  dns_ptr_records = {
    "ptr-example" = {
      ttl = 300,
      records = [
        "example.com",
      ]
    }
  }
  dns_srv_records = {
    "srv-example" = {
      ttl = 300,
      records = [
        {
          priority = 1,
          weight   = 5,
          port     = 8080
          target   = target.example.com
        }
      ]
    }
  }
  dns_txt_records = {
    "txt-example" = {
      ttl = 300,
      records = [
        "google-site-authenticator",
        "more site information here"
      ]
    }
  }

  enable_cdn_frontdoor                         = true
  restrict_web_app_service_to_cdn_inbound_only = true
  cdn_frontdoor_sku                            = "Standard_AzureFrontDoor"
  enable_cdn_frontdoor_health_probe            = true
  cdn_frontdoor_health_probe_interval          = 30
  cdn_frontdoor_health_probe_path              = "/"
  cdn_frontdoor_response_timeout               = 120
  cdn_frontdoor_custom_domains = [
    "example.com",
    "www.example.com"
  ]
  cdn_frontdoor_host_redirects = [
    {
      "from" = "example.com",
      "to"   = "www.example.com",
    }
  ]
  cdn_frontdoor_enable_rate_limiting              = true
  cdn_frontdoor_rate_limiting_duration_in_minutes = 1
  cdn_frontdoor_rate_limiting_threshold           = 300
  cdn_frontdoor_rate_limiting_bypass_ip_list      = ["8.8.8.8/32"]
  cdn_frontdoor_waf_mode                          = "Prevention"
  cdn_frontdoor_host_add_response_headers = [
    {
      "name"  = "Strict-Transport-Security",
      "value" = "max-age=31536000",
    }
  ]
  cdn_frontdoor_remove_response_headers = [
    "Server",
  ]
}
```

<!-- BEGIN_TF_DOCS -->
## Requirements

| Name | Version |
|------|---------|
| <a name="requirement_terraform"></a> [terraform](#requirement\_terraform) | >= 1.4.5 |
| <a name="requirement_azapi"></a> [azapi](#requirement\_azapi) | >= 1.4.0 |
| <a name="requirement_azurerm"></a> [azurerm](#requirement\_azurerm) | >= 3.48.0 |
| <a name="requirement_dns"></a> [dns](#requirement\_dns) | >= 3.2.4 |
| <a name="requirement_null"></a> [null](#requirement\_null) | >= 3.2.1 |

## Providers

| Name | Version |
|------|---------|
| <a name="provider_azurerm"></a> [azurerm](#provider\_azurerm) | 3.57.0 |
| <a name="provider_dns"></a> [dns](#provider\_dns) | 3.3.2 |

## Resources

| Name | Type |
|------|------|
| [azurerm_application_insights.web_app_service](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/application_insights) | resource |
| [azurerm_application_insights_standard_web_test.web_app_service](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/application_insights_standard_web_test) | resource |
| [azurerm_cdn_frontdoor_custom_domain.custom_domain](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_custom_domain) | resource |
| [azurerm_cdn_frontdoor_custom_domain_association.custom_domain_association](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_custom_domain_association) | resource |
| [azurerm_cdn_frontdoor_endpoint.endpoint](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_endpoint) | resource |
| [azurerm_cdn_frontdoor_firewall_policy.waf](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_firewall_policy) | resource |
| [azurerm_cdn_frontdoor_origin.origin](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_origin) | resource |
| [azurerm_cdn_frontdoor_origin_group.group](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_origin_group) | resource |
| [azurerm_cdn_frontdoor_profile.cdn](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_profile) | resource |
| [azurerm_cdn_frontdoor_route.route](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_route) | resource |
| [azurerm_cdn_frontdoor_rule.add_response_headers](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_rule) | resource |
| [azurerm_cdn_frontdoor_rule.redirect](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_rule) | resource |
| [azurerm_cdn_frontdoor_rule.remove_response_header](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_rule) | resource |
| [azurerm_cdn_frontdoor_rule_set.add_response_headers](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_rule_set) | resource |
| [azurerm_cdn_frontdoor_rule_set.redirects](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_rule_set) | resource |
| [azurerm_cdn_frontdoor_rule_set.remove_response_headers](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_rule_set) | resource |
| [azurerm_cdn_frontdoor_security_policy.waf](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cdn_frontdoor_security_policy) | resource |
| [azurerm_dns_a_record.dns_a_records](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_a_record) | resource |
| [azurerm_dns_a_record.dns_alias_records](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_a_record) | resource |
| [azurerm_dns_a_record.frontdoor_custom_domain](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_a_record) | resource |
| [azurerm_dns_aaaa_record.dns_aaaa_records](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_aaaa_record) | resource |
| [azurerm_dns_caa_record.dns_caa_records](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_caa_record) | resource |
| [azurerm_dns_cname_record.dns_cname_records](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_cname_record) | resource |
| [azurerm_dns_mx_record.dns_mx_records](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_mx_record) | resource |
| [azurerm_dns_ns_record.dns_ns_records](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_ns_record) | resource |
| [azurerm_dns_ptr_record.dns_ptr_records](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_ptr_record) | resource |
| [azurerm_dns_srv_record.dns_srv_records](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_srv_record) | resource |
| [azurerm_dns_txt_record.dns_txt_records](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_txt_record) | resource |
| [azurerm_dns_txt_record.frontdoor_custom_domain](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_txt_record) | resource |
| [azurerm_dns_zone.default](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_zone) | resource |
| [azurerm_linux_web_app.default](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/linux_web_app) | resource |
| [azurerm_log_analytics_workspace.web_app_service](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/log_analytics_workspace) | resource |
| [azurerm_logic_app_action_http.slack](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/logic_app_action_http) | resource |
| [azurerm_logic_app_trigger_http_request.webhook](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/logic_app_trigger_http_request) | resource |
| [azurerm_logic_app_workflow.webhook](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/logic_app_workflow) | resource |
| [azurerm_monitor_action_group.web_app_service](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/monitor_action_group) | resource |
| [azurerm_monitor_diagnostic_setting.web_app_service](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/monitor_diagnostic_setting) | resource |
| [azurerm_monitor_diagnostic_setting.webhook](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/monitor_diagnostic_setting) | resource |
| [azurerm_monitor_metric_alert.cpu](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/monitor_metric_alert) | resource |
| [azurerm_monitor_metric_alert.http](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/monitor_metric_alert) | resource |
| [azurerm_monitor_metric_alert.latency](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/monitor_metric_alert) | resource |
| [azurerm_monitor_metric_alert.memory](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/monitor_metric_alert) | resource |
| [azurerm_nat_gateway.default](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/nat_gateway) | resource |
| [azurerm_nat_gateway_public_ip_association.default](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/nat_gateway_public_ip_association) | resource |
| [azurerm_network_security_group.web_app_service_infra_allow_frontdoor_inbound_only](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/network_security_group) | resource |
| [azurerm_public_ip.nat_gateway](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/public_ip) | resource |
| [azurerm_resource_group.default](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/resource_group) | resource |
| [azurerm_route_table.default](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/route_table) | resource |
| [azurerm_service_plan.default](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/service_plan) | resource |
| [azurerm_storage_account.logs](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/storage_account) | resource |
| [azurerm_storage_container.logs](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/storage_container) | resource |
| [azurerm_subnet.web_app_service_infra_subnet](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/subnet) | resource |
| [azurerm_subnet_nat_gateway_association.app_service](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/subnet_nat_gateway_association) | resource |
| [azurerm_subnet_network_security_group_association.web_app_service_infra_allow_frontdoor_inbound_only](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/subnet_network_security_group_association) | resource |
| [azurerm_subnet_route_table_association.web_app_service_infra_subnet](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/subnet_route_table_association) | resource |
| [azurerm_virtual_network.default](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/virtual_network) | resource |
| [azurerm_windows_web_app.default](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/windows_web_app) | resource |
| [azurerm_resource_group.existing_resource_group](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/data-sources/resource_group) | data source |
| [azurerm_storage_account_blob_container_sas.logs](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/data-sources/storage_account_blob_container_sas) | data source |
| [azurerm_virtual_network.existing_virtual_network](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/data-sources/virtual_network) | data source |
| [dns_a_record_set.web_app_service_ip_address](https://registry.terraform.io/providers/hashicorp/dns/latest/docs/data-sources/a_record_set) | data source |

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:--------:|
| <a name="input_azure_location"></a> [azure\_location](#input\_azure\_location) | Azure location in which to launch resources. | `string` | n/a | yes |
| <a name="input_cdn_frontdoor_custom_domains"></a> [cdn\_frontdoor\_custom\_domains](#input\_cdn\_frontdoor\_custom\_domains) | Azure CDN Front Door custom domains. If they are within the DNS zone (optionally created), the Validation TXT records and ALIAS/CNAME records will be created | `list(string)` | `[]` | no |
| <a name="input_cdn_frontdoor_enable_rate_limiting"></a> [cdn\_frontdoor\_enable\_rate\_limiting](#input\_cdn\_frontdoor\_enable\_rate\_limiting) | Enable CDN Front Door Rate Limiting. This will create a WAF policy, and CDN security policy. For pricing reasons, there will only be one WAF policy created. | `bool` | `false` | no |
| <a name="input_cdn_frontdoor_health_probe_interval"></a> [cdn\_frontdoor\_health\_probe\_interval](#input\_cdn\_frontdoor\_health\_probe\_interval) | Specifies the number of seconds between health probes. | `number` | `30` | no |
| <a name="input_cdn_frontdoor_health_probe_path"></a> [cdn\_frontdoor\_health\_probe\_path](#input\_cdn\_frontdoor\_health\_probe\_path) | Specifies the path relative to the origin that is used to determine the health of the origin. | `string` | `"/"` | no |
| <a name="input_cdn_frontdoor_health_probe_request_type"></a> [cdn\_frontdoor\_health\_probe\_request\_type](#input\_cdn\_frontdoor\_health\_probe\_request\_type) | Specifies the type of health probe request that is made. | `string` | `"GET"` | no |
| <a name="input_cdn_frontdoor_host_add_response_headers"></a> [cdn\_frontdoor\_host\_add\_response\_headers](#input\_cdn\_frontdoor\_host\_add\_response\_headers) | List of response headers to add at the CDN Front Door `[{ "Name" = "Strict-Transport-Security", "value" = "max-age=31536000" }]` | `list(map(string))` | `[]` | no |
| <a name="input_cdn_frontdoor_host_redirects"></a> [cdn\_frontdoor\_host\_redirects](#input\_cdn\_frontdoor\_host\_redirects) | CDN FrontDoor host redirects `[{ "from" = "example.com", "to" = "www.example.com" }]` | `list(map(string))` | `[]` | no |
| <a name="input_cdn_frontdoor_origin_fqdn_override"></a> [cdn\_frontdoor\_origin\_fqdn\_override](#input\_cdn\_frontdoor\_origin\_fqdn\_override) | Manually specify the hostname that the CDN Front Door should target. Defaults to the App Service hostname | `string` | `""` | no |
| <a name="input_cdn_frontdoor_rate_limiting_bypass_ip_list"></a> [cdn\_frontdoor\_rate\_limiting\_bypass\_ip\_list](#input\_cdn\_frontdoor\_rate\_limiting\_bypass\_ip\_list) | List if IP CIDRs to bypass CDN Front Door rate limiting | `list(string)` | `[]` | no |
| <a name="input_cdn_frontdoor_rate_limiting_duration_in_minutes"></a> [cdn\_frontdoor\_rate\_limiting\_duration\_in\_minutes](#input\_cdn\_frontdoor\_rate\_limiting\_duration\_in\_minutes) | CDN Front Door rate limiting duration in minutes | `number` | `1` | no |
| <a name="input_cdn_frontdoor_rate_limiting_threshold"></a> [cdn\_frontdoor\_rate\_limiting\_threshold](#input\_cdn\_frontdoor\_rate\_limiting\_threshold) | Maximum number of concurrent requests before Rate Limiting policy is applied | `number` | `300` | no |
| <a name="input_cdn_frontdoor_remove_response_headers"></a> [cdn\_frontdoor\_remove\_response\_headers](#input\_cdn\_frontdoor\_remove\_response\_headers) | List of response headers to remove at the CDN Front Door | `list(string)` | `[]` | no |
| <a name="input_cdn_frontdoor_response_timeout"></a> [cdn\_frontdoor\_response\_timeout](#input\_cdn\_frontdoor\_response\_timeout) | Azure CDN Front Door response timeout in seconds | `number` | `120` | no |
| <a name="input_cdn_frontdoor_sku"></a> [cdn\_frontdoor\_sku](#input\_cdn\_frontdoor\_sku) | Azure CDN Front Door SKU | `string` | `"Standard_AzureFrontDoor"` | no |
| <a name="input_cdn_frontdoor_waf_mode"></a> [cdn\_frontdoor\_waf\_mode](#input\_cdn\_frontdoor\_waf\_mode) | CDN Front Door waf mode | `string` | `"Prevention"` | no |
| <a name="input_dns_a_records"></a> [dns\_a\_records](#input\_dns\_a\_records) | DNS A records to add to the DNS Zone | <pre>map(<br>    object({<br>      ttl : optional(number, 300),<br>      records : list(string)<br>    })<br>  )</pre> | `{}` | no |
| <a name="input_dns_aaaa_records"></a> [dns\_aaaa\_records](#input\_dns\_aaaa\_records) | DNS AAAA records to add to the DNS Zone | <pre>map(<br>    object({<br>      ttl : optional(number, 300),<br>      records : list(string)<br>    })<br>  )</pre> | `{}` | no |
| <a name="input_dns_alias_records"></a> [dns\_alias\_records](#input\_dns\_alias\_records) | DNS ALIAS records to add to the DNS Zone | <pre>map(<br>    object({<br>      ttl : optional(number, 300),<br>      target_resource_id : string<br>    })<br>  )</pre> | `{}` | no |
| <a name="input_dns_caa_records"></a> [dns\_caa\_records](#input\_dns\_caa\_records) | DNS CAA records to add to the DNS Zone | <pre>map(<br>    object({<br>      ttl : optional(number, 300),<br>      records : list(<br>        object({<br>          flags : number,<br>          tag : string,<br>          value : string<br>        })<br>      )<br>    })<br>  )</pre> | `{}` | no |
| <a name="input_dns_cname_records"></a> [dns\_cname\_records](#input\_dns\_cname\_records) | DNS CNAME records to add to the DNS Zone | <pre>map(<br>    object({<br>      ttl : optional(number, 300),<br>      record : string<br>    })<br>  )</pre> | `{}` | no |
| <a name="input_dns_mx_records"></a> [dns\_mx\_records](#input\_dns\_mx\_records) | DNS MX records to add to the DNS Zone | <pre>map(<br>    object({<br>      ttl : optional(number, 300),<br>      records : list(<br>        object({<br>          preference : number,<br>          exchange : string<br>        })<br>      )<br>    })<br>  )</pre> | `{}` | no |
| <a name="input_dns_ns_records"></a> [dns\_ns\_records](#input\_dns\_ns\_records) | DNS NS records to add to the DNS Zone | <pre>map(<br>    object({<br>      ttl : optional(number, 300),<br>      records : list(string)<br>    })<br>  )</pre> | `{}` | no |
| <a name="input_dns_ptr_records"></a> [dns\_ptr\_records](#input\_dns\_ptr\_records) | DNS PTR records to add to the DNS Zone | <pre>map(<br>    object({<br>      ttl : optional(number, 300),<br>      records : list(string)<br>    })<br>  )</pre> | `{}` | no |
| <a name="input_dns_srv_records"></a> [dns\_srv\_records](#input\_dns\_srv\_records) | DNS SRV records to add to the DNS Zone | <pre>map(<br>    object({<br>      ttl : optional(number, 300),<br>      records : list(<br>        object({<br>          priority : number,<br>          weight : number,<br>          port : number,<br>          target : string<br>        })<br>      )<br>    })<br>  )</pre> | `{}` | no |
| <a name="input_dns_txt_records"></a> [dns\_txt\_records](#input\_dns\_txt\_records) | DNS TXT records to add to the DNS Zone | <pre>map(<br>    object({<br>      ttl : optional(number, 300),<br>      records : list(string)<br>    })<br>  )</pre> | `{}` | no |
| <a name="input_dns_zone_domain_name"></a> [dns\_zone\_domain\_name](#input\_dns\_zone\_domain\_name) | DNS zone domain name. If created, records will automatically be created to point to the CDN. | `string` | `""` | no |
| <a name="input_dns_zone_soa_record"></a> [dns\_zone\_soa\_record](#input\_dns\_zone\_soa\_record) | DNS zone SOA record block (https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/dns_zone#soa_record) | `map(string)` | `{}` | no |
| <a name="input_enable_cdn_frontdoor"></a> [enable\_cdn\_frontdoor](#input\_enable\_cdn\_frontdoor) | Enable Azure CDN Front Door. This will use the Web App default hostname as the origin. | `bool` | `false` | no |
| <a name="input_enable_cdn_frontdoor_health_probe"></a> [enable\_cdn\_frontdoor\_health\_probe](#input\_enable\_cdn\_frontdoor\_health\_probe) | Enable CDN Front Door health probe | `bool` | `true` | no |
| <a name="input_enable_dns_zone"></a> [enable\_dns\_zone](#input\_enable\_dns\_zone) | Conditionally create a DNS zone | `bool` | `false` | no |
| <a name="input_enable_monitoring"></a> [enable\_monitoring](#input\_enable\_monitoring) | Create an App Insights instance and notification group for the Web App Service | `bool` | `false` | no |
| <a name="input_enable_nat_gateway"></a> [enable\_nat\_gateway](#input\_enable\_nat\_gateway) | Conditionally enable NAT Gateway. This is used to configure a static outbound IP. This is only avaiable when launching within a VNet. | `bool` | `true` | no |
| <a name="input_enable_service_logs"></a> [enable\_service\_logs](#input\_enable\_service\_logs) | Enable service logs, stored in blob storage | `bool` | `true` | no |
| <a name="input_environment"></a> [environment](#input\_environment) | Environment name. Will be used along with `project_name` as a prefix for all resources. | `string` | n/a | yes |
| <a name="input_existing_resource_group"></a> [existing\_resource\_group](#input\_existing\_resource\_group) | Conditionally launch resources into an existing resource group. Specifying this will NOT create a resource group. | `string` | `""` | no |
| <a name="input_existing_virtual_network"></a> [existing\_virtual\_network](#input\_existing\_virtual\_network) | Conditionally use an existing virtual network. The `virtual_network_address_space` must match an existing address space in the VNet. This also requires the resource group name. | `string` | `""` | no |
| <a name="input_launch_in_vnet"></a> [launch\_in\_vnet](#input\_launch\_in\_vnet) | Conditionally launch into a VNet | `bool` | `true` | no |
| <a name="input_monitor_email_receivers"></a> [monitor\_email\_receivers](#input\_monitor\_email\_receivers) | A list of email addresses that should be notified by monitoring alerts | `list(string)` | `[]` | no |
| <a name="input_monitor_enable_slack_webhook"></a> [monitor\_enable\_slack\_webhook](#input\_monitor\_enable\_slack\_webhook) | Enable slack webhooks to send monitoring notifications to a channel | `bool` | `false` | no |
| <a name="input_monitor_endpoint_healthcheck"></a> [monitor\_endpoint\_healthcheck](#input\_monitor\_endpoint\_healthcheck) | Specify a route that should be monitored for a 200 OK status | `string` | `"/"` | no |
| <a name="input_monitor_slack_channel"></a> [monitor\_slack\_channel](#input\_monitor\_slack\_channel) | Slack channel name/id to send messages to | `string` | `""` | no |
| <a name="input_monitor_slack_webhook_receiver"></a> [monitor\_slack\_webhook\_receiver](#input\_monitor\_slack\_webhook\_receiver) | A Slack App webhook URL | `string` | `""` | no |
| <a name="input_project_name"></a> [project\_name](#input\_project\_name) | Project name. Will be used along with `environment` as a prefix for all resources. | `string` | n/a | yes |
| <a name="input_restrict_web_app_service_to_cdn_inbound_only"></a> [restrict\_web\_app\_service\_to\_cdn\_inbound\_only](#input\_restrict\_web\_app\_service\_to\_cdn\_inbound\_only) | Restricts access to the Web App by addin an ip restriction rule which only allows 'AzureFrontDoor.Backend' inbound and matches the cdn fdid header. It also creates a network security group that only allows 'AzureFrontDoor.Backend' inbound, and attaches it to the subnet of the web app. | `bool` | `true` | no |
| <a name="input_service_app_settings"></a> [service\_app\_settings](#input\_service\_app\_settings) | Service app settings | `map(string)` | `{}` | no |
| <a name="input_service_health_check_eviction_time_in_min"></a> [service\_health\_check\_eviction\_time\_in\_min](#input\_service\_health\_check\_eviction\_time\_in\_min) | The amount of time in minutes that a node can be unhealthy before being removed from the load balancer | `number` | `5` | no |
| <a name="input_service_health_check_path"></a> [service\_health\_check\_path](#input\_service\_health\_check\_path) | Service health check path | `string` | `"/"` | no |
| <a name="input_service_log_level"></a> [service\_log\_level](#input\_service\_log\_level) | Service log level | `string` | `"Information"` | no |
| <a name="input_service_log_retention"></a> [service\_log\_retention](#input\_service\_log\_retention) | Service log retention in days | `number` | `30` | no |
| <a name="input_service_log_storage_sas_expiry"></a> [service\_log\_storage\_sas\_expiry](#input\_service\_log\_storage\_sas\_expiry) | Service log sas token start date/time | `string` | `""` | no |
| <a name="input_service_log_storage_sas_start"></a> [service\_log\_storage\_sas\_start](#input\_service\_log\_storage\_sas\_start) | Service log sas token start date/time | `string` | `""` | no |
| <a name="input_service_plan_os"></a> [service\_plan\_os](#input\_service\_plan\_os) | Service plan operating system. Valid values are `Windows` or `Linux`. | `string` | `"Windows"` | no |
| <a name="input_service_plan_sku"></a> [service\_plan\_sku](#input\_service\_plan\_sku) | Service plan sku | `string` | `"S1"` | no |
| <a name="input_service_stack"></a> [service\_stack](#input\_service\_stack) | The application stack for the web app. Valid values are `dotnet`, `dotnetcore`, `node`, `python`, `php`, `java`, `ruby` or `go`. | `string` | `"dotnet"` | no |
| <a name="input_service_stack_version"></a> [service\_stack\_version](#input\_service\_stack\_version) | The version of the chosen application stack | `string` | `"v4.0"` | no |
| <a name="input_service_worker_count"></a> [service\_worker\_count](#input\_service\_worker\_count) | The number of Workers for the App Service | `number` | `1` | no |
| <a name="input_tags"></a> [tags](#input\_tags) | Tags to be applied to all resources | `map(string)` | `{}` | no |
| <a name="input_virtual_network_address_space"></a> [virtual\_network\_address\_space](#input\_virtual\_network\_address\_space) | Virtual Network address space CIDR | `string` | `"172.16.0.0/12"` | no |

## Outputs

| Name | Description |
|------|-------------|
| <a name="output_azurerm_log_analytics_workspace_web_app_service"></a> [azurerm\_log\_analytics\_workspace\_web\_app\_service](#output\_azurerm\_log\_analytics\_workspace\_web\_app\_service) | Web App Service Log Analytics Workspace |
| <a name="output_azurerm_resource_group_default"></a> [azurerm\_resource\_group\_default](#output\_azurerm\_resource\_group\_default) | Default Azure Resource Group |
| <a name="output_azurerm_storage_account_logs"></a> [azurerm\_storage\_account\_logs](#output\_azurerm\_storage\_account\_logs) | Logs Storage Account |
| <a name="output_environment"></a> [environment](#output\_environment) | n/a |
<!-- END_TF_DOCS -->

[1]: https://azure.microsoft.com/en-us/products/app-service
[2]: https://docs.microsoft.com/en-us/azure/virtual-network/virtual-networks-overview
