# Features

This is the **default home for feature code**.

## Structure:
Each feature should have its own folder:
```
features/
└── <feature-name>/
    ├── <feature-name>.component.ts     # Smart/container component
    ├── <feature-name>.component.html
    ├── <feature-name>.component.css
    ├── <feature-name>.component.spec.ts
    ├── <feature-name>.service.ts       # Feature-specific data access
    ├── <feature-name>.service.spec.ts
    └── components/                      # Feature-specific dumb components
        └── ...
```

## What belongs here:
- Feature routes and routing components
- Feature UI (smart and dumb components)
- Feature-specific state management
- Feature-specific data-access services
- Feature-specific models and types

## Boundaries:
- **No imports between features** - If code is reused across features, move it to `shared/` (if pure) or `core/` (if app-wide singleton)
- Features can import from `core/` and `shared/`
- Features should be as self-contained as possible
