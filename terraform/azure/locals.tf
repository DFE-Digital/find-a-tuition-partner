locals {
  monitor_email_receivers = [
    for email in split("|", var.monitor_email_receivers) : trim(
    email, " ")
  ]
}