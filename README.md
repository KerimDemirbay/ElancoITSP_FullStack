# UK Tick Sightings

An MVP full-stack web app for exploring and reporting tick sightings in UK cities, built for the Elanco pre-interview task.

Backend: ASP.NET Core (.NET 10) Web API with EF Core and SQLite.
Frontend: Vue + TypeScript + Vite, with a Leaflet map.

It builds on my project from last year (Elanco Tick Sighting Visualiser), rebuilt to fit the Elanco API and to a more professional standard.

# Features

## Map
A pin for every city, with a circle that grows slightly and goes from yellow to dark red as the number of sightings rises.
Filters for a date range and a species. Cities with no sightings for the current filters are hidden, and the legend rescales to the visible cities.
A postcode search, using postcodes.io. It opens the matching city, or says plainly when the postcode is outside the cities we have data for.
Works on phones: the filters fold behind a button and the city panel slides up from the bottom.

## Sighting information
Clicking a pin or circle opens a side panel with the city's number of sightings, its latest sighting and a breakdown by species.
Get directions opens Google Maps directions to the city.
Share uses the phone's share menu, or copies a link. The link (/?city=Leeds) opens the map on that city.

## Education
A species guide covering all five species in the data, with facts from the UK Health Security Agency.
Advice on avoiding tick bites and removing ticks, from the NHS.

## Report a sighting
A form with a date, city, species and optional notes.
Errors are shown under each field. The form is checked in the browser first, and any errors the API returns are mapped onto the right fields too.
On success, a green confirmation appears with a link to the city on the map.
The API can return amber warnings, for example when a sighting is more than a year old or has no notes.
New reports are saved as "Community" sightings and appear on the map straight away.

# Running it

You need the .NET 10 SDK and Node.js 24.12+.

## 1. Backend

bash
```
cd backend/TickSightings
dotnet run
```
The API runs on http://localhost:5105.
On first start it creates tick-sightings.db (SQLite) and downloads the Elanco data in the background. Until that finishes, data endpoints return 503 Service Unavailable.
In Development, Swagger UI is at http://localhost:5105/swagger.

## 2. Frontend (in a second terminal)

bash
```
cd frontend
npm install
npm run dev
```
Open http://localhost:5173.
Vite forwards every /api request to the backend, so both run without any CORS setup.

# Technical choices

Because most of this project was based on my old repo, the tech stack remained largely the same.

### ASP.NET Core Web API (.NET 10):
.NET 10 is the current long-term support release, as .NET 8 will reach end of life on 10 November 2026. It offers controllers, built-in validation and ProblemDetails, which give consistent errors with little code.

### SQLite + EF Core:
the data needs to be filtered, grouped and paged. A database does that in SQL with indexes instead of loading everything into memory, so it scales to much larger datasets. SQLite is a single file, so there's nothing extra to install.

### Background refresh service:
data is downloaded on start-up and then every 30 minutes (configurable in appsettings.json), so requests never wait on the Elanco API. A failed refresh is retried after 1 minute, and the last good data stays available. Because the data is stored in the database rather than in memory, it also survives a restart.

### Microsoft.Extensions.Http.Resilience:
calls to the Elanco API get retries with backoff, timeouts and a circuit breaker. It provides great utility for zero implementation cost.

### Vue 3 + TypeScript + Vite:
the same frontend stack as last year's repo, now with TypeScript types that match the API's JSON.

### Leaflet + OpenStreetMap tiles:
the same as last year's repo. The map is shown in greyscale so its own road colours don't clash with the yellow-to-red scale.

### Pins and circles (not a heatmap):
the data only gives a city for each sighting, not an exact location. A heatmap or boundary shading would suggest more precision than the data has.

# How the data is handled

SightingCleaner checks every record from the Elanco API.

### Rejected and counted by reason:
missing id;

missing or invalid date;

a date in the future;

an unknown location;

an unknown species;

a duplicate id;

duplicate content (same date, city and species under a different id).
### Repaired:
names are matched without caring about case or extra spaces;

a species is identified from its Latin name when the common name is missing.


>The counts are available at GET /api/data-status.

On each refresh, the Elanco rows are replaced inside a database transaction, so a failed refresh can't leave half-written data. Community reports are stored separately and are kept.

Data note: the Elanco data gives the passerine tick's Latin name as Dermacentor frontalis. The UK Health Security Agency lists it as Ixodes frontalis. The backend keeps the API's value so records still match, and the species guide shows the UKHSA name with a note.

>Credits
>
>Map data © OpenStreetMap contributors. Map library: Leaflet.
>
>Postcode lookup: postcodes.io.
>
>Species facts: UKHSA tick species profiles. Tick advice: NHS.
>
>Tick photos courtesy of ESCCAP UK & Ireland.
