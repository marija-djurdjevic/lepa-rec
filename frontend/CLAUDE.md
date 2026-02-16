## Goal
Evolve this Angular v21 app with the smallest change that fits existing patterns.

## Always follow
- Read relevant files first; don't speculate about unseen code.
- Don't add/upgrade npm deps unless explicitly asked.
- Don't introduce new architecture layers (global store/CQRS/etc.) unless a concrete need exists.
- Don't edit lockfiles, generated files, or build config unless explicitly asked.
- Never output/request secrets (keys/tokens/credentials).

## Angular v21 essentials
- **Zoneless is default in v21**. Don't assume ZoneJS behavior.
  - Don't add `provideZoneChangeDetection(...)`.
  - Don't re-introduce `zone.js` imports/polyfills.
- Use **standalone components** everywhere (NgModules are legacy).
- Prefer **Signals** for local/feature state; keep effects explicit.
- Use **Signals** for state; keep **RxJS** for streams, events, and async operations.
- Use **Resource API** (`rxResource`) for data fetching with signals.
- Don't use **Signal Forms** (experimental in v21; stick to reactive forms).

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
6) Quality gate: run lint + unit tests for changed feature and potentially affected areas; run build if you touched routing/DI/config.

## Testing
- Unit tests: Vitest with `@angular/core/testing`
- Test signals directly (no need to trigger change detection manually)
- Test components via public API (inputs/outputs/DOM), not internals
- Keep tests focused: test behavior, not implementation details
- Mock external dependencies (HTTP, services) but avoid over-mocking

### What NOT to test
- **No "should create" smoke tests** — if the component is broken, other tests fail anyway.
- **No internal structure assertions** — don't assert that a form control exists, a signal holds a specific value, or a property has a type. Test the user-visible consequence instead.
- **No framework-logic tests** — don't verify that `Validators.required` makes a form invalid. Angular already tests that.
- **No incomplete HTTP tests** — if you fire an HTTP request in a test, assert the *outcome* (state change, navigation, DOM update), not just that the request was made.

### What TO test
- **Behavior from the user's perspective** — given state X, the DOM shows Y; when the user clicks Z, effect W happens.
- **State transitions with observable outcomes** — login stores tokens *and* navigates; logout clears tokens *and* clears storage.
- **Edge cases and error paths** — failed HTTP calls show error messages; missing tokens trigger redirects.
- **Guards, interceptors, and services** — these are pure logic; test inputs → outputs directly.

## Commands
Use the repo's existing scripts (check package.json):
- dev: `npm start` / `ng serve`
- test: `npm test` / `ng test` (Vitest)
- lint: `npm run lint`
- build: `npm run build` (esbuild-based, default in v21)