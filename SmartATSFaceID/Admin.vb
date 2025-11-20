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

    Public Function GetInfos() As String
        Dim nom As String = Me.nom
        Dim prenom As String = Me.prenom

        Return nom + " " + prenom
    End Function
    Public Shared Function Login(email As String, password As String) As Boolean
        Dim result As Admin
        Try

            Using connection As New MySqlConnection(connectionString)
                Dim query As String = "SELECT nom, prenom, email, password FROM admins WHERE email=@email"

                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@email", email)

                    connection.Open()

                    Using reader As MySqlDataReader = command.ExecuteReader()

                        If reader.HasRows Then
                            reader.Read()
                            If (reader("password").ToString() = password) Then

                                result = New Admin(reader("nom").ToString(), reader("prenom").ToString, reader("email").ToString(), reader("password").ToString())
                                loggedAdmin = result

                                Return True
                            Else
                                MessageBox.Show("Mot de passe incorrect.", "Erreur de Connexion")
                                Return False
                            End If
                        Else
                            MessageBox.Show("Utilisateur non trouvé.", "Erreur de Connexion")
                            Return False
                        End If
                    End Using

                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show($"Une erreur est survenue lors de la connexion : {ex.Message}", "Erreur Critique de Base de Données")
            Console.WriteLine($"Erreur détaillée: {ex.StackTrace}")
            Return False
        End Try
    End Function

    Public Shared Function SignUp(nom As String, prenom As String, email As String, password As String) As Boolean
        Using connection As New MySqlConnection(connectionString)
            Dim query As String = " INSERT INTO admins (nom, prenom, email, password) VALUES(@nom, @prenom, @email, @password)"
            Using command As New MySqlCommand(query, connection)
                command.Parameters.AddWithValue("@nom", nom)
                command.Parameters.AddWithValue("@prenom", prenom)
                command.Parameters.AddWithValue("@email", email)
                command.Parameters.AddWithValue("@password", password)
                connection.Open()

                Dim rowsAffected As Integer = command.ExecuteNonQuery()

                If rowsAffected > 0 Then
                    Return True
                Else
                    Return False
                End If

            End Using
        End Using
    End Function

End Class
