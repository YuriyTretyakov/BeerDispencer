# BeerDispencer

## What is it?

This service was intended for educational purpose and can be used as a portfolio
of skills and knowledge obtaiined. Original challenge task was taken [here](https://rviewer.stoplight.io/docs/beer-tap-dispenser/juus8uwnzzal5-beer-tap-dispenser)

This product demonstrates author's understanding of:
- ASP.Net
- CLean Architecture approach
- MediatR pipeline
- Entity Framework
- Identity Server with Roles-based authorization
- Using Healthchecks
- Using MemoryCache
- Unit Test coverage with using Moq library
- Using FluentValidation to validate http requests
- Using FluentMigrator to allow deployment with less downtime
- Stripe payments gateway integration
- Fault-tolerance with Outbox pattern


![Code Coverage](https://img.shields.io/badge/Code%20Coverage-18%25-critical?style=flat) [![.NET Build And UnitTests](https://github.com/YuriyTretyakov/BeerDispencer/actions/workflows/dotnet.yml/badge.svg?event=workflow_run)](https://github.com/YuriyTretyakov/BeerDispencer/actions/workflows/dotnet.yml)
## Create a new dispenser

```
Request
POST
/dispenser
This endpoint will create a new dispenser with a configuration about how much volume comes out (litres per second)

Body
application/json
flow_volume number required
Flow volume is the number of liters per second are coming out from the tap
```
```
Responses
200 Dispenser created correctly
500 Unexpected API error


Body
application/json
id string <uuid> required
Id of the created dispenser

flow_volume number required
The configured flow volume (litres/second) for the new dispenser
```

## Change Dispencer's Status

```
Request
/dispenser/{id}/status
Path Parameters
id string required Dispenser Id

Body
application/json
status string required
Status of the flow dispenser

Allowed values:
open
close

updated_at
string <date-time>
Timestamp for the update
```
```
Responses
202 Status of the tap changed correctly
409 Dispenser is already opened/closed
500 Unexpected API error
```

## Returns the money spent by the given dispenser Id
```
Request
/dispenser/{id}/spending
Path Parameters
id string required Dispenser Id
```

```
Responses
200 Total amount spent by the dispenser
404 Requested dispenser does not exist
500 Unexpected API error


Body
application/json

amount number required Total amount
usages array[object] required 

Usage lines:
opened_at string <date-time> Example: 2022-01-01T02:00:00Z
closed_at string<date-time> or null Example: 2022-01-01T02:00:50Z
flow_volume number Example: 0.064
total_spent number Example: 39.2
```
