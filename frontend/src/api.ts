//interface to match the record classes in C#
export interface DataQualityReport {
    received: number
    accepted: number
    rejected: Record<string, number>
}

export interface RefreshFailure {
    failedAt: string
    message: string
}

export interface DataStatus {
    isReady: boolean
    lastRefreshedAt: string | null
    sightingCount: number
    quality: DataQualityReport | null
    lastFailure: RefreshFailure | null
}

export interface ProblemDetails {
    title?: string
    detail?: string
    status?: number
    errors?: Record<string, string[]>
}

export class ApiError extends Error {
    readonly status: number
    readonly problem: ProblemDetails | null

    constructor(status: number, problem: ProblemDetails | null) {
        super(problem?.detail ?? problem?.title ?? `Request failed with status ${status}`)
        this.status = status
        this.problem = problem
    }
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
    const response = await fetch(`/api${path}`, init)

    if (!response.ok) {
        const problem = await response.json().catch(() => null)
        throw new ApiError(response.status, problem)
    }

    return response.json() as Promise<T>
}

export function getDataStatus(): Promise<DataStatus> {
    return request<DataStatus>('/data-status')
}

export interface SpeciesCount {
    species: string
    count: number
}

export interface CityReport {
    city: string
    region: string
    latitude: number
    longitude: number
    count: number
    latestSighting: string | null
    species: SpeciesCount[]
}

export interface SpeciesInfo {
    name: string
    latinName: string
}

export interface SightingFilter {
    from?: string
    to?: string
    city?: string
    region?: string
    species?: string
}

function toQueryString(filter: SightingFilter): string {
    const params = new URLSearchParams()

    for (const [key, value] of Object.entries(filter)) {
        if (value) params.set(key, value)
    }

    const query = params.toString()
    return query ? `?${query}` : ''
}

export function getCityReports(filter: SightingFilter): Promise<CityReport[]> {
    return request<CityReport[]>(`/reports/by-city${toQueryString(filter)}`)
}

export function getSpecies(): Promise<SpeciesInfo[]> {
    return request<SpeciesInfo[]>('/reference/species')
}