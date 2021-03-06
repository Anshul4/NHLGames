﻿Imports System.IO
Imports NHLGames.My.Resources

Namespace Utilities
    Public Class FileAccess
        Public Shared Function IsFileReadonly(path As String) As Boolean
            Dim attributes As FileAttributes = File.GetAttributes(path)
            Return (attributes And FileAttributes.[ReadOnly]) = FileAttributes.[ReadOnly]
        End Function

        Public Shared Sub RemoveReadOnly(path As String)
            Dim attributes As FileAttributes = File.GetAttributes(path)

            If (attributes And FileAttributes.[ReadOnly]) = FileAttributes.[ReadOnly] Then
                ' Make the file RW
                attributes = RemoveAttribute(attributes, FileAttributes.[ReadOnly])
                File.SetAttributes(path, attributes)
                Console.WriteLine(English.msgRemoveReadOnly, path)
            End If
        End Sub

        Public Shared Sub AddReadonly(path As String)
            File.SetAttributes(path, File.GetAttributes(path) Or FileAttributes.ReadOnly)
            Console.WriteLine(English.msgAddReadOnly, path)
        End Sub

        Private Shared Function RemoveAttribute(attributes As FileAttributes, attributesToRemove As FileAttributes) _
            As FileAttributes
            Return attributes And Not attributesToRemove
        End Function

        Public Shared Function HasAccess(filePath As String, Optional createIt As Boolean = true,
                                         Optional reportException As Boolean = false,
                                         Optional contentText As String = "")
            Try
                If createIt AndAlso Not File.Exists(filePath) Then File.WriteAllText(filePath, contentText)

                Using New StreamReader(filePath)
                End Using

                Using File.Open(filePath, FileMode.Open, IO.FileAccess.ReadWrite, FileShare.None)
                End Using

                Return True
            Catch ex As Exception
                If reportException Then
                    Console.WriteLine(English.errorAccessPath, ex.Message)
                End If
                Return False
            End Try
        End Function
    End Class
End Namespace
