# Copilot instructions — IT Capstone Proposal Management and Tracking System

You are assisting on a **VB.NET Windows Forms desktop application** with a
**MySQL** backend. Read this file fully before generating or editing code.
Companion docs: `docs/PROJECT_OVERVIEW.md`, `docs/ARCHITECTURE.md`,
`docs/DATABASE_SCHEMA.md`. Treat all three as authoritative context.

## What this system is

A department-level system for Trinidad Municipal College's IT Department that
digitizes the capstone proposal process: submission, duplicate-title
checking, review/approval, and archiving of approved titles. Originally
scoped with three roles (Student, Faculty Adviser, Admin); **the instructor
has since simplified this to two roles only: Student and Admin.** The Admin
now performs everything the Faculty Adviser used to do (review proposals,
set status, write remarks) in addition to admin-only duties (accounts,
archive, reports). Do not reintroduce a separate adviser/staff role, table,
or form unless explicitly asked.

## Tech stack — do not substitute without being asked

- **Language:** VB.NET (not C#). `Option Strict On` and `Option Explicit On`
  at the top of every file.
- **UI:** Windows Forms (.NET Framework or .NET 8 Windows Forms — confirm
  target framework in the `.vbproj` before assuming).
- **Database:** MySQL, accessed via **MySql.Data.MySqlClient** (MySQL
  Connector/NET) or `MySqlConnector` NuGet package — pick one and stay
  consistent across the whole codebase.
- **Data access pattern:** plain ADO.NET (`MySqlConnection`, `MySqlCommand`,
  `MySqlDataReader`) wrapped in DAL classes. No ORM (Entity Framework, Dapper)
  unless explicitly requested — this is a school project and the layered
  ADO.NET approach is what's documented and expected.

## Architecture — follow the layering strictly

```
Forms (Presentation) → BLL (Business Logic) → DAL (Data Access) → MySQL
```

- **Forms never contain SQL.** A button-click handler calls a BLL method,
  never `MySqlCommand` directly.
- **BLL never touches `MySqlConnection` directly.** It calls DAL methods and
  applies validation/business rules (status transitions, duplicate-check
  scoring) in between.
- **DAL is the only layer that opens a database connection.** Every method
  uses `Using` blocks so connections and readers always close, even on
  exceptions.
- **All SQL is parameterized.** Never concatenate user input into a query
  string. Use `cmd.Parameters.AddWithValue(...)` (or `.Add` with explicit
  `MySqlDbType` for numeric/date parameters).

Full class-by-class breakdown is in `docs/ARCHITECTURE.md`.

## Roles and access control

- Two roles only: `Student`, `Admin`. Stored in `tbl_users.role`.
- After login, branch on role to load `frmStudentDashboard` or
  `frmAdminDashboard`. Do not add a third branch.
- Admin forms (`frmManageProposals`, `frmTitleArchive`, `frmReports`,
  `frmManageAccounts`) must never be reachable from the Student session, and
  vice versa — check role at form-load, not just by hiding menu buttons.

## Key business rules

- Proposal status is one of exactly four values: `Pending`, `Approved`,
  `Rejected`, `ForRevision`. Represent this as a VB `Enum` in the BLL/Models
  layer, not as free-text strings scattered through the UI.
- Duplicate-title checking: extract keywords from the submitted title,
  strip stop words (`and`, `of`, `the`, `for`, etc.), then compute a
  similarity score against existing titles. **60% or higher triggers a
  warning**, but the student may still submit — the admin makes the final
  call. Do not silently block submission at the UI level.
- Every status change must be recorded in `tbl_status_log` (old status, new
  status, remarks, who made the change, when) — this is the audit trail and
  also what powers "remarks history" on the student's dashboard.
- Passwords are always hashed before storage and never logged or displayed
  in plaintext, including in debug output.

## Coding conventions

- **Naming:** PascalCase for classes, methods, and public properties;
  camelCase for local variables and parameters; private fields prefixed
  with `_` (e.g. `_connectionString`). Forms keep the `frm` prefix
  (`frmLogin`, `frmSubmitProposal`) to match the existing Visual Studio
  designer files — don't rename existing forms.
- **Error handling:** wrap DAL calls in `Try/Catch`, log or surface a
  user-friendly message, and never let a raw `MySqlException` reach the UI
  unhandled.
- **No magic strings for status/role values** — use the enums defined in
  the Models layer.
- Match the folder structure in `docs/ARCHITECTURE.md` when adding new
  files — new forms go in `Forms/`, new business logic in `BLL/`, new data
  access in `DAL/`.

## What NOT to do

- Don't add a Faculty Adviser role, table, or form back in.
- Don't put SQL in code-behind files for forms.
- Don't use string concatenation to build SQL queries.
- Don't introduce a new database library, ORM, or UI framework without
  being asked.
- Don't invent new statuses beyond the four listed above.
