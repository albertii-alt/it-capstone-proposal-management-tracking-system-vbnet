# Architecture — IT Capstone Proposal Management and Tracking System

4-layer architecture: **Presentation → Business Logic → Data Access →
MySQL**. Each layer only calls the one directly below it.

```
Student ─┐
         ├──▶ Presentation layer (Windows Forms)
Admin ───┘              │
                         ▼
              Business logic layer
                         │
                         ▼
              Data access layer
                         │
                         ▼
                  MySQL database
```

## 1. Presentation layer (Forms/)

Windows Forms only. No SQL, no `MySqlConnection` here — every action calls a
BLL method and renders the result.

| Form | Role | Purpose |
|---|---|---|
| `frmLogin` | Both | Credential entry, routes by role after `AuthService.Login()` |
| `frmStudentDashboard` | Student | Landing screen, links to submission and status views |
| `frmSubmitProposal` | Student | Proposal submission form + inline duplicate-check warning |
| `frmMyProposals` | Student | Read-only list of the student's own proposals, status, remarks |
| `frmAdminDashboard` | Admin | Summary stats, navigation to admin tools |
| `frmManageProposals` | Admin | Review queue: view details, set status, write remarks |
| `frmTitleArchive` | Admin | Browse approved titles across all semesters |
| `frmReports` | Admin | Filter + print summary reports |
| `frmManageAccounts` | Admin | Create/deactivate user accounts |

## 2. Business logic layer (BLL/)

| Class | Responsibility |
|---|---|
| `AuthService` | Validates credentials against `UserDAL`, resolves role, hashes/verifies passwords |
| `ProposalService` | Create/update proposals, enforce valid status transitions, orchestrate submission flow |
| `DuplicateChecker` | Keyword extraction, stop-word filtering, similarity scoring (see below) |
| `ReportService` | Aggregates proposal data by status/semester/group for `frmReports` and `frmAdminDashboard` |
| `ArchiveService` | Queries/maintains the approved-title archive |

**`DuplicateChecker` outline:**
1. Normalize the submitted title (lowercase, strip punctuation).
2. Tokenize into keywords, remove stop words (`and`, `of`, `the`, `for`, ...).
3. Compare the keyword set against every existing title's keyword set —
   Jaccard similarity (intersection / union of keyword sets) is a
   reasonable, easy-to-explain choice for a capstone defense; Levenshtein
   distance is a fine alternative if you want character-level rather than
   keyword-level comparison.
4. If the highest score across all comparisons is ≥ 0.60, return that score
   plus the matched title so the UI can show a warning. The student may
   still submit; the Admin makes the final duplicate call during review.

## 3. Data access layer (DAL/)

| Class | Responsibility |
|---|---|
| `DbConnectionHelper` | Holds the connection string, exposes a method that returns a new `MySqlConnection` |
| `UserDAL` | CRUD for `tbl_users` |
| `ProposalDAL` | CRUD for `tbl_proposals` and `tbl_proposal_members` |
| `StatusLogDAL` | Insert/read `tbl_status_log` (status history + remarks) |
| `ArchiveDAL` | Read/write `tbl_title_archive` |

Every method: open connection in a `Using` block, use parameterized
`MySqlCommand`, close/dispose automatically on block exit (including on
exception).

## 4. Models/ (shared across layers)

| Model | Notes |
|---|---|
| `User` | Id, Username, PasswordHash, Role (enum), FullName, Email |
| `Proposal` | Id, Title, Description, Course, YearLevel, Semester, Status (enum), SubmittedBy, SubmittedAt |
| `ProposalMember` | Id, ProposalId, MemberName |
| `StatusLogEntry` | Id, ProposalId, AdminId, OldStatus, NewStatus, Remarks, ChangedAt |
| `UserRole` (enum) | `Student`, `Admin` |
| `ProposalStatus` (enum) | `Pending`, `Approved`, `Rejected`, `ForRevision` |

## Solution / folder structure

A single Visual Studio Windows Forms project keeps things manageable for a
subject-level capstone (multi-project class-library separation is optional
polish, not required):

```
ITCapstoneProposalSystem/
├── ITCapstoneProposalSystem.sln
└── ITCapstoneProposalSystem/
    ├── Forms/
    │   ├── frmLogin.vb
    │   ├── frmStudentDashboard.vb
    │   ├── frmSubmitProposal.vb
    │   ├── frmMyProposals.vb
    │   ├── frmAdminDashboard.vb
    │   ├── frmManageProposals.vb
    │   ├── frmTitleArchive.vb
    │   ├── frmReports.vb
    │   └── frmManageAccounts.vb
    ├── BLL/
    │   ├── AuthService.vb
    │   ├── ProposalService.vb
    │   ├── DuplicateChecker.vb
    │   ├── ReportService.vb
    │   └── ArchiveService.vb
    ├── DAL/
    │   ├── DbConnectionHelper.vb
    │   ├── UserDAL.vb
    │   ├── ProposalDAL.vb
    │   ├── StatusLogDAL.vb
    │   └── ArchiveDAL.vb
    ├── Models/
    │   ├── User.vb
    │   ├── Proposal.vb
    │   ├── ProposalMember.vb
    │   ├── StatusLogEntry.vb
    │   └── Enums.vb
    ├── Helpers/
    │   └── PasswordHasher.vb
    └── My Project/  (VS-generated: settings, resources, app.config)
```

`docs/` and `.github/copilot-instructions.md` sit at the repo root, next to
the `.sln` file, not inside the WinForms project folder.

## Connection string

Keep it out of source-controlled code. Put it in `App.config`
(`<connectionStrings>`) or a `Settings.settings` entry, and read it via
`My.Settings` / `ConfigurationManager.ConnectionStrings` in
`DbConnectionHelper` — never hardcode credentials directly in a `.vb` file.
