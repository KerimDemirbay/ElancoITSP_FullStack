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
### Choice +	Why

Because most of this project was based on my old repo, the tech stack remained largely the same.

ASP.NET Core Web API (.NET 10) is the current long-term support release, as .NET 8 will reach End of Life on November 10th 2026. .NET 10 offers controllers, built-in validation and ProblemDetails give consistent errors with little code.

SQLite + EF Core was used as the data needs to be filtered, grouped and paged. A database does that in SQL with indexes instead of loading everything into memory, so it scales to much larger datasets. SQLite is a single file, so there's nothing extra to install.

Background refresh service was used as data is downloaded on start-up and then every 30 minutes (configurable in appsettings.json), so requests never wait on the Elanco API. A failed refresh is retried after 1 minute, and the last good data stays available, which is something that isn't achievable with a snapshot approach for example.

Microsoft.Extensions.Http.Resilience calls to the Elanco API get retries with backoff, timeouts and a circuit breaker. Provides great utility for zero implementation cost.

Vue 3 + TypeScript + Vite are the same frontend stack as last year's repo, now with TypeScript types that match the API's JSON.

Leaflet + OpenStreetMap tiles are the same as last year's repo. The map is shown in greyscale so its own road colours don't clash with the yellow-to-red scale. Pins and circles were chosen, not a heatmap	as the data only gives a city for each sighting, not an exact location. A heatmap or boundary shading would suggest more precision in data.



>Credits
>
>Map data © OpenStreetMap contributors. Map library: Leaflet.
>
>Postcode lookup: postcodes.io.
>
>Species facts: UKHSA tick species profiles. Tick advice: NHS.
>
>Tick photos courtesy of ESCCAP UK & Ireland.
