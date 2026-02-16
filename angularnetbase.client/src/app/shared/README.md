# Shared

This directory contains **reusable UI components and utilities** with no app-wide state.

## What belongs here:
- Reusable UI components (buttons, cards, modals, etc.)
- Pipes and directives used across features
- Utility functions and helper modules
- Type definitions and interfaces used across features
- Validators and form utilities

## What does NOT belong here:
- App-wide stateful services (put in `core/`)
- Feature-specific components (put in `features/<feature-name>/`)
- Business logic tied to specific features

## Boundaries:
- Shared components should be **pure** (no side effects)
- Components here should work in isolation
- No imports from `features/` or `core/`
- Everything here should be reusable across multiple features
