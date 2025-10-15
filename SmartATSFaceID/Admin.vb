Option Strict On
Option Explicit On
Imports System.ComponentModel.DataAnnotations
Imports System.Windows.Automation.Peers
Imports MySql.Data.MySqlClient

Public Class Admin
    Inherits User
    Private nom As String
    Private prenom As String
    Private email As String
    Private password As String

    Public Sub New(nom As String, prenom As String, email As String, password As String)
        MyBase.New(nom, prenom)
        Me.nom = nom
        Me.prenom = prenom
        Me.email = email
        Me.password = password
    End Sub
    Public Overrides Sub CreateUser()
        Throw New NotImplementedException()
    End Sub

    Public Overrides Sub UpdateUser()
        Throw New NotImplementedException()
    End Sub

    Public Overrides Sub DeleteUser()
        Throw New NotImplementedException()
    End Sub
    Public Function login(email As String, password As String) As Boolean
        Dim result As Admin

        Using connection As New MySqlConnection(connectionString)
            Dim query As String = " SELECT (nom, prenom, email) FROM admins WHERE  email=@email AND password=@password"
            Using command As New MySqlCommand(query, connection)
                command.Parameters.AddWithValue(" @email ", email)
                command.Parameters.AddWithValue(" @password", password)
                connection.Open()
                Dim reader As MySqlDataReader = command.ExecuteReader()

                If reader.HasRows Then
                    reader.Read()

                    result = New Admin(reader("nom").ToString(), reader("prenom").ToString, reader("email").ToString(), reader("PasswordBoxAutomationPeer").ToString())
                    loggedAdmin = result

                    Return True

                Else
                    Return False
                End If

            End Using
        End Using
    End Function

End Class
