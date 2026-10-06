Imports System.Collections.Generic

Public Class AdminActivityLogService

    Public Shared Sub Record(actor As User, actionType As String, entityType As String,
                             entityId As Integer?, entityName As String, description As String,
                             Optional metadata As String = Nothing)
        If actor Is Nothing Then Throw New ArgumentNullException(NameOf(actor))
        If Not UserRoleHelper.IsAdministrator(actor.Role) Then
            Throw New UnauthorizedAccessException("Only administrators can create admin activity records.")
        End If
        If String.IsNullOrWhiteSpace(actionType) Then Throw New ArgumentException("An action type is required.", NameOf(actionType))
        If String.IsNullOrWhiteSpace(entityType) Then Throw New ArgumentException("An entity type is required.", NameOf(entityType))
        If String.IsNullOrWhiteSpace(description) Then Throw New ArgumentException("A description is required.", NameOf(description))

        AdminActivityLogDAL.Insert(New AdminActivityLogEntry With {
            .ActorId = actor.UserId,
            .ActorName = actor.FullName,
            .ActionType = actionType.Trim(),
            .EntityType = entityType.Trim(),
            .EntityId = entityId,
            .EntityName = If(entityName, "").Trim(),
            .Description = description.Trim(),
            .Metadata = If(metadata, "").Trim()
        })
    End Sub

    Public Shared Sub RecordStudentEvent(student As User, proposalId As Integer?, proposalTitle As String,
                                         actionType As String, remarks As String, recordedAt As DateTime)
        If student Is Nothing Then Throw New ArgumentNullException(NameOf(student))
        AdminActivityLogDAL.Insert(New AdminActivityLogEntry With {
            .ActorId = student.UserId,
            .ActorName = student.FullName,
            .ActionType = actionType.Trim(),
            .EntityType = "Student History",
            .EntityId = proposalId,
            .EntityName = If(proposalTitle, "").Trim(),
            .Description = "Student history: " & actionType.Trim() & " for """ & If(proposalTitle, "").Trim() & """.",
            .Metadata = If(remarks, "").Trim(),
            .CreatedAt = recordedAt
        })
    End Sub

    Public Shared Function GetEntries(actionType As String, entityType As String, searchText As String,
                                      fromDate As DateTime?, toDate As DateTime?,
                                      pageIndex As Integer, pageSize As Integer,
                                      ByRef totalCount As Integer) As List(Of AdminActivityLogEntry)
        Return AdminActivityLogDAL.GetEntries(actionType, entityType, searchText, fromDate, toDate,
                                               pageIndex, pageSize, totalCount)
    End Function

    Public Shared Function DeleteEntries(entries As List(Of AdminActivityLogEntry)) As Integer
        Return AdminActivityLogDAL.DeleteEntries(entries)
    End Function
End Class
