Option Strict On
Option Explicit On
Imports MySql.Data.MySqlClient
Imports Emgu.CV
Imports System.IO
Imports System.ComponentModel.DataAnnotations
Imports System.Windows.Automation.Peers
Public Class Employe

    Public Property Id As Integer
    Public Property Nom As String
    Public Property Prenom As String
    Public Property PhotoPath As String

    Public Shared Function createUser(nom As String, prenom As String, image As Emgu.CV.Mat) As Integer
        ' 1. Préparer le dossier de stockage (où les images seront stockées)
        Dim saveDirectory As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FaceImages")

        ' Créer le dossier s'il n'existe pas
        If Not Directory.Exists(saveDirectory) Then
            Directory.CreateDirectory(saveDirectory)
        End If

        ' 2. Créer un nom de fichier unique et le chemin complet
        ' Utiliser un GUID ou une combinaison de nom/prénom + date
        Dim fileName As String = $"{nom}_{prenom}_{Guid.NewGuid().ToString()}.jpg"
        Dim fullPath As String = Path.Combine(saveDirectory, fileName)

        ' La valeur à stocker dans la BD est le chemin relatif ou absolu
        ' Le chemin relatif est souvent préférable pour la portabilité
        Dim dbPath As String = Path.Combine("FaceImages", fileName)

        Try
            ' 3. Sauvegarder l'image Mat sur le disque
            ' La classe CvInvoke permet de sauvegarder les objets Mat
            CvInvoke.Imwrite(fullPath, image)

            ' 4. Implémentation de l'insertion en Base de Données
            Using connection As New MySqlConnection(connectionString)
                ' S'assurer que la colonne 'photo' dans MySQL est de type VARCHAR(255) ou plus
                Dim query As String = " INSERT INTO Employes (nom, prenom, photo) VALUES(@nom, @prenom, @photo); SELECT LAST_INSERT_ID();"

                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@nom", nom)
                    command.Parameters.AddWithValue("@prenom", prenom)

                    ' Stocker le chemin du fichier (dbPath) et NON l'objet Mat
                    command.Parameters.AddWithValue("@photo", dbPath)

                    connection.Open()

                    Dim newIdObject As Object = command.ExecuteScalar()
                    Dim newId As Integer

                    If newIdObject IsNot Nothing AndAlso Integer.TryParse(newIdObject.ToString(), newId) Then
                        Return newId
                    Else
                        ' Échec de l'insertion ou de la récupération de l'ID : supprimer l'image
                        If File.Exists(fullPath) Then
                            File.Delete(fullPath)
                        End If
                        Return -1 ' Échec
                    End If

                End Using
            End Using

        Catch ex As Exception
            ' Gérer les erreurs (BD ou sauvegarde de fichier)
            MsgBox($"Erreur lors de la création de l'utilisateur : {ex.Message}")

            ' Si une erreur survient, tenter de nettoyer le fichier s'il a été créé
            If File.Exists(fullPath) Then
                File.Delete(fullPath)
            End If

            Return -1
        End Try
    End Function

    Public Function getUser(ByVal userId As Integer) As Employe
        Dim retrievedEmploye As Employe = Nothing

        Try
            Using connection As New MySqlConnection(connectionString)
                ' 1. Requête SQL pour sélectionner toutes les colonnes par ID
                Dim query As String = "SELECT Id, nom, prenom, photo FROM Employes WHERE Id = @Id"

                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Id", userId)

                    connection.Open()

                    Using reader As MySqlDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            ' 2. Création et remplissage de l'objet Employe
                            retrievedEmploye = New Employe With {
                                .Id = reader.GetInt32("Id"),
                                .Nom = reader.GetString("nom"),
                                .Prenom = reader.GetString("prenom"),
                                .PhotoPath = reader.GetString("photo")
                            }
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            MsgBox($"Erreur lors de la récupération de l'employé : {ex.Message}")
            retrievedEmploye = Nothing
        End Try

        Return retrievedEmploye
    End Function

    Public Sub createParticipation(userId)
        Dim currentDate As DateAndTime = New Date.Date()
        Dim time As DateAndTime = New Date.Hour()
        Try
            Using connection As New MySqlConnection(connectionString)
                ' 1. Requête SQL pour sélectionner toutes les colonnes par ID
                Dim query As String = "INSERT INTO Presences userId, date, time VALUES (@userId, @date, @time)"

                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@userId", userId)
                    command.Parameters.AddWithValue("@date", currentDate)
                    command.Parameters.AddWithValue("@time", time)

                    connection.Open()

                    command.ExecuteNonQuery()
                End Using
            End Using

        Catch ex As Exception
            MsgBox($"Erreur lors de la récupération de l'employé : {ex.Message}")
            retrievedEmploye = Nothing
        End Try

    End Sub
End Class
