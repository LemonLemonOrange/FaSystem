# Requirements Document

## Introduction

This feature adds a **Drought Alert (枯旱預警)** integration to the fa_api flood/water management system. The backend fetches the latest drought alert data from the NCDR (國家災害防救科技中心) public API, parses the CAP (Common Alerting Protocol) XML format, and exposes the structured alert data through an ASP.NET Core MVC controller and view.

The feature ports existing JavaScript/React logic into a C# backend service, eliminating the need for client-side XML parsing and CORS proxy workarounds. The severity levels follow the NCDR standard: Minor (綠色/水情注意), Moderate (黃色/減壓供水), Severe (橙色/限制供水), Extreme (紅色/定點供水停止).

## Glossary

- **NCDR**: 國家災害防救科技中心 — National Science and Technology Center for Disaster Reduction; operates the public alert feed at `https://alerts.ncdr.nat.gov.tw`
- **CAP**: Common Alerting Protocol — an OASIS XML standard for emergency alert messages
- **DroughtAlert**: A single parsed drought alert record derived from a CAP `.cap` XML file
- **DroughtAlertFeed**: The JSON Atom feed listing available drought alert entries, fetched from `https://alerts.ncdr.nat.gov.tw/ncdr/webapi/JSONAtomFeed.ashx?AlertType=2099`
- **AlertType=2099**: The NCDR alert type code that filters results to drought (枯旱預警) alerts only
- **Severity**: The drought severity level — one of `Minor`, `Moderate`, `Severe`, or `Extreme`
- **DroughtAlertService**: The C# service responsible for fetching and parsing drought alert data
- **WaterGovController**: The existing ASP.NET Core MVC controller that will expose the drought alert endpoint
- **HttpClient**: The .NET HTTP client used to call external APIs
- **IHttpClientFactory**: The ASP.NET Core factory for creating named/typed `HttpClient` instances

## Requirements

### Requirement 1: Fetch Drought Alert Feed

**User Story:** As a system operator, I want the backend to retrieve the latest drought alert feed from NCDR, so that the application always has up-to-date drought warning data without relying on client-side API calls.

#### Acceptance Criteria

1. WHEN the DroughtAlertService is invoked, THE DroughtAlertService SHALL send an HTTP GET request to `https://alerts.ncdr.nat.gov.tw/ncdr/webapi/JSONAtomFeed.ashx?AlertType=2099`
2. WHEN the feed response contains one or more entries, THE DroughtAlertService SHALL select the entry with the most recent `updated` timestamp as the latest alert
3. WHEN the feed response contains no entries, THE DroughtAlertService SHALL return a null result indicating no active drought alert
4. IF the HTTP request to the feed endpoint fails, THEN THE DroughtAlertService SHALL throw a descriptive exception containing the HTTP status code and endpoint URL
5. THE DroughtAlertService SHALL deserialize the feed response as JSON into a structured feed model containing an `entry` array

### Requirement 2: Fetch and Parse CAP XML Alert Detail

**User Story:** As a system operator, I want the backend to retrieve and parse the full CAP XML alert file for the latest drought entry, so that structured alert details (severity, headline, description, affected areas) are available to the application.

#### Acceptance Criteria

1. WHEN the latest feed entry is identified, THE DroughtAlertService SHALL extract the `.cap` file URL from the entry's `link.href` field
2. WHEN the CAP file URL is available, THE DroughtAlertService SHALL send an HTTP GET request to that URL to retrieve the CAP XML content
3. WHEN the CAP XML is retrieved, THE DroughtAlertService SHALL parse it into a DroughtAlert object containing: `identifier`, `sent`, `status`, `info.severity`, `info.headline`, `info.description`, `info.effective`, `info.expires`, and `info.area`
4. WHEN the `area` element in the CAP XML contains a single object, THE DroughtAlertService SHALL normalize it into a single-element list
5. WHEN the `area` element in the CAP XML contains multiple objects, THE DroughtAlertService SHALL preserve all elements as a list
6. IF the CAP file URL is missing or empty, THEN THE DroughtAlertService SHALL return a null result
7. IF the HTTP request to the CAP file URL fails, THEN THE DroughtAlertService SHALL throw a descriptive exception containing the HTTP status code and CAP URL

### Requirement 3: Expose Drought Alert via MVC Controller

**User Story:** As a developer, I want a controller action that returns the latest drought alert data, so that the frontend or other consumers can display current drought warning information.

#### Acceptance Criteria

1. THE WaterGovController SHALL expose a `DroughtAlert` action at the route `WaterGov/DroughtAlert`
2. WHEN the `DroughtAlert` action is invoked and an active alert exists, THE WaterGovController SHALL pass the DroughtAlert object to the view
3. WHEN the `DroughtAlert` action is invoked and no active alert exists (null result), THE WaterGovController SHALL pass a null model to the view
4. IF the DroughtAlertService throws an exception, THEN THE WaterGovController SHALL log the error and return an error view with a user-readable message
5. THE WaterGovController SHALL inject DroughtAlertService via constructor dependency injection

### Requirement 4: Display Drought Alert in View

**User Story:** As a user of the flood management system, I want to see the current drought alert status on a dedicated page, so that I can quickly understand the severity and affected areas.

#### Acceptance Criteria

1. WHEN the view receives a non-null DroughtAlert model, THE View SHALL display the severity level, headline, description, effective date, expiry date, and list of affected area names
2. WHEN the view receives a null model, THE View SHALL display a message indicating no active drought alert is currently in effect
3. WHEN the severity is `Minor`, THE View SHALL render the severity indicator with a green colour
4. WHEN the severity is `Moderate`, THE View SHALL render the severity indicator with a yellow colour
5. WHEN the severity is `Severe`, THE View SHALL render the severity indicator with an orange colour
6. WHEN the severity is `Extreme`, THE View SHALL render the severity indicator with a red colour
7. THE View SHALL display the list of affected areas (areaDesc) as a readable list

### Requirement 5: HTTP Client Configuration

**User Story:** As a developer, I want HTTP clients for external API calls to be properly configured via dependency injection, so that connections are managed efficiently and base addresses are centralised.

#### Acceptance Criteria

1. THE Startup SHALL register a named HttpClient for the NCDR feed endpoint with base address `https://alerts.ncdr.nat.gov.tw`
2. THE DroughtAlertService SHALL use IHttpClientFactory to create HttpClient instances
3. WHEN making requests to the CAP file URL, THE DroughtAlertService SHALL use the same named HttpClient registered for the NCDR base address
