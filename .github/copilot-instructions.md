# Copilot Instructions

## Project Guidelines
- User prefers implementing custom Identity stores and custom user/role classes instead of using EF; wants stores implementing IUserStore, IUserRoleStore, IUserPasswordStore, IUserLockoutStore and IRoleStore; will use stored procedures for DB interactions.
- Prefer adding using directives so object and enum types are referenced by short names (e.g., IAlertService instead of fully-qualified namespace).
- Prefer Bootstrap for styling in generated UI components.