# National Tutoring Programme (NTP)

## Introduction

National Tutoring Programme's Find a Tutoring Partner service is currently in discovery. This respository will initially contain spikes and prototype work and will eventually contain the publically accessible code under an appropriate licence for the live service.

### GOV.UK PaaS

#### Commands

cf push
cf delete fatp-dev
cf create-service postgres tiny-unencrypted-13 fatp-dev-postgres-db
cf delete-service fatp-dev-postgres-db
