# Core

This directory contains **app-wide singletons only**.

## What belongs here:
- Authentication and session management services
- HTTP interceptors
- Global error handling
- Application configuration
- App-wide state management (if needed)

## What does NOT belong here:
- Feature-specific code (put in `features/`)
- Reusable UI components (put in `shared/`)
- Feature-specific services (put in `features/<feature-name>/`)

## Boundaries:
- Core services are provided at the root level (`providedIn: 'root'`)
- Core **must not** depend on features
- Core services are typically injected throughout the app
