# Project overview — IT Capstone Proposal Management and Tracking System

Trinidad Municipal College · Department of Information Technology
VB.NET Programming — Block 6

> This document supersedes the roles/features section of the originally
> submitted proposal wherever the instructor's revision (no separate staff
> role) applies. Everything else from the original proposal still holds.

## Background

The IT Department requires students to submit a capstone project proposal
before starting development. This is currently handled manually — printed
forms, logbooks/spreadsheets kept by the department secretary or faculty
coordinator — which causes lost records, unclear approval status, and
duplicate or near-duplicate titles submitted by different groups without
knowing it.

This system computerizes that process: a centralized platform for
submission, duplicate-title checking, approval tracking, and archiving of
approved titles.

## Problem statement

- **Duplicate titles** — groups unknowingly propose the same/similar titles.
- **No centralized records** — physical logbooks/spreadsheets are prone to
  loss, damage, inconsistency.
- **Unclear approval status** — students have no reliable way to check
  where their proposal stands.
- **Inefficient evaluation** — no unified platform to review, approve,
  reject, or request revisions.
- **No historical archive** — approved titles from past semesters aren't
  preserved, so old topics get recycled.

## Objectives

**General:** Develop a VB.NET Windows Forms system that digitizes capstone
proposal submission, evaluation, and approval for the IT Department.

**Specific:**
- Secure login distinguishing Student and Admin accounts.
- Let students submit proposals through a digital form.
- Implement keyword + similarity-score duplicate detection with a warning
  at ≥60% similarity.
- Let the Admin review proposals and record a decision — Approved,
  Rejected, or For Revision — with written remarks.
- Show students real-time status updates on their own proposals.
- Maintain an archive of all previously approved titles across semesters.
- Generate summary reports of proposals by status, semester, or group.

## Target users (revised)

| Role | Capabilities |
|---|---|
| **Student** | Log in, submit a proposal, get a live duplicate-title check, view the status and remarks on their own submissions. |
| **Admin** | Everything a Faculty Adviser and Department Administrator would have done: review all pending proposals, write remarks, set status; manage user accounts; browse the full title archive; generate printable reports. |

There is no third "staff" or "faculty adviser" role — this was in the
original proposal but the instructor asked for it to be removed. The Admin
account absorbs those review responsibilities.

## Features

1. **User authentication** — role-based login (Student/Admin/SuperAdmin), hashed
   passwords.
2. **Proposal submission form** — group leader, members, course, year
   level, title, description, preferred semester; required-field
   validation.
3. **Duplicate title checker** — keyword extraction, stop-word filtering
   (`and`, `of`, `the`, `for`, ...), similarity scoring against all existing
   titles; warns at ≥60% similarity; final call stays with the Admin.
4. **Approval status tracking** — Pending / Approved / Rejected / For
   Revision, visible to the student along with any remarks.
5. **Admin review panel** — view all proposals, enter remarks, update
   status.
6. **Title archive** — approved titles from all semesters, queryable to
   prevent topic recycling.
7. **Admin dashboard & reports** — summary counts by status; printable
   reports filtered by semester, status, or group.

## Scope

- BSIT students of the IT Department only, single department.
- Student and Admin accounts.
- Submission → review → approval workflow.
- Keyword/similarity-based duplicate detection.
- Basic summary reporting.

## Limitations

- Single department only — not built for multi-department use.
- Duplicate checker is keyword/similarity based; it won't catch
  differently-worded titles with the same meaning — final verification is
  still a human (Admin) judgment call.
- No email/SMS notifications — status must be checked by logging in.
- No integration with other school systems (enrollment, grading, etc.).
- Covers the proposal stage only, not the full capstone development
  lifecycle.

## User interface (planned forms)

- Login
- Student dashboard → Submit proposal, My proposals (status & remarks)
- Admin dashboard → Manage proposals, Title archive, Reports, Manage
  accounts
