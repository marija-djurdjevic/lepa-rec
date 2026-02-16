## Goal
Evolve this Angular v21 app with the smallest change that fits existing patterns.

## Always follow
- Read relevant files first; don’t speculate about unseen code.
- Don’t add/upgrade npm deps unless explicitly asked.
- Don’t introduce new architecture layers (global store/CQRS/etc.) unless a concrete need exists.
- Don’t edit lockfiles, generated files, or build config unless explicitly asked.
- Never output/request secrets (keys/tokens/credentials).

## Angular v21 essentials
- **Zoneless is default in v21**. Don’t assume ZoneJS behavior.  
  - Don’t add `provideZoneChangeDetection(...)`.
  - Don’t re-introduce `zone.js` imports/polyfills.
- Prefer **Signals** for local/feature state; keep effects explicit.
- Don't use **Signal Forms**.

## Structure & boundaries (keep it feature-first)
UI code lives in `src/`.

- `src/app/core/`   app-wide singletons only (auth/session, interceptors, global error handling, config)
- `src/app/shared/` reusable UI + utils (no app-wide stateful services)
- `src/app/features/<feature>/` the default home for feature code (routes + UI + state + data-access)

Boundaries:
- No imports between features. If reused, move to `shared/` (pure) or `core/` (app-wide).
- `core/` must not depend on features.

## How to execute any task
1) Find the owning feature (routes → UI → feature state/service → data-access).
2) Ask me questions regarding the requirements and design options.
3) Ask me questions about automated test design while giving suggestions on what makes sense.
4) Propose a minimal plan (3–6 bullets) if the change is non-trivial.
5) Implement locally to the owning feature and include automated tests.
6) Quality gate: run lint + relevant unit tests; run build if you touched routing/DI/public APIs.

## Commands
Use the repo’s existing scripts (check package.json):
- dev: `npm start` / `ng serve`
- test: `npm test` / `ng test` (v21 uses Vitest)
- lint: `npm run lint`
- build: `npm run build`