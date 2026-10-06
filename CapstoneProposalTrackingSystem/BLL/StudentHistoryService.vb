Imports System.Collections.Generic
Imports System.Linq

Public Class StudentHistoryService
    Public Shared Sub Record(student As User, proposalId As Integer?, proposalTitle As String,
                             actionType As String, remarks As String, recordedAt As DateTime)
        If student Is Nothing OrElse student.Role <> UserRole.Student Then
            Throw New UnauthorizedAccessException("Only students can create student history records.")
        End If
        StudentHistoryDAL.Insert(New StudentHistoryEntry With {
            .StudentId = student.UserId,
            .ProposalId = proposalId,
            .ProposalTitle = If(proposalTitle, "").Trim(),
            .Action = If(actionType, "").Trim(),
            .Remarks = If(remarks, "").Trim(),
            .RecordedAt = recordedAt
        })
        AdminActivityLogService.RecordStudentEvent(student, proposalId, proposalTitle, actionType, remarks, recordedAt)
    End Sub

    Public Shared Function GetForStudent(studentId As Integer) As List(Of StudentHistoryEntry)
        Return StudentHistoryDAL.GetForStudent(studentId)
    End Function

    Public Shared Function DeleteForStudent(student As User, entries As List(Of StudentHistoryEntry)) As Integer
        If student Is Nothing OrElse student.Role <> UserRole.Student Then
            Throw New UnauthorizedAccessException("Only students can delete student history.")
        End If
        If entries Is Nothing Then Throw New ArgumentNullException(NameOf(entries))
        Return StudentHistoryDAL.DeleteForStudent(student.UserId, entries.Where(Function(entry) entry IsNot Nothing).Select(Function(entry) entry.HistoryId).ToList())
    End Function

    Public Shared Function DeleteForStudent(student As User, historyIds As List(Of Integer)) As Integer
        If student Is Nothing OrElse student.Role <> UserRole.Student Then
            Throw New UnauthorizedAccessException("Only students can delete student history.")
        End If
        Return StudentHistoryDAL.DeleteForStudent(student.UserId, historyIds)
    End Function
End Class
