Option Strict On
Option Explicit On

Public Enum UserRole
    Student
    Admin
    SuperAdmin
End Enum

Public Module UserRoleHelper
    Public Function IsAdministrator(role As UserRole) As Boolean
        Return role = UserRole.Admin OrElse role = UserRole.SuperAdmin
    End Function
End Module

Public Enum ProposalStatus
    Pending
    Approved
    Rejected
    ForRevision
End Enum