// Types that match the JSON the C# API returns.
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

// The ProblemDetails JSON the API sends back for every error.
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