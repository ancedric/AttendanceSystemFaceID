Imports System.Windows.Threading
Imports MySql.Data.MySqlClient
Class MainWindow
    Private timer As DispatcherTimer

    Public Sub New()

        InitializeComponent()
        CreateDataBase()
        SetupTimer()
    End Sub

    Private Sub SetupTimer()
        timer = New DispatcherTimer With {
            .Interval = TimeSpan.FromMilliseconds(100) ' Mettre à jour toutes les 100ms
            }
        AddHandler timer.Tick, AddressOf Timer_Tick
        timer.Start()
    End Sub
    Private Sub Timer_Tick(sender As Object, e As EventArgs)
        If progressBar.Value < progressBar.Maximum Then
            progressBar.Value += 1
        Else
            timer.Stop() ' Arrêter le timer une fois terminé
            Dim login As New loginScreen
            Me.Close()
            login.Show()
        End If
    End Sub

    Private Sub CreateDataBase()
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()

                ' 1. Créer la base de données
                Dim createDbCommand As New MySqlCommand($"CREATE DATABASE IF NOT EXISTS {databaseName};", connection)
                createDbCommand.ExecuteNonQuery()

                ' Utiliser la nouvelle base de données
                Dim useDbCommand As New MySqlCommand($"USE {databaseName};", connection)
                useDbCommand.ExecuteNonQuery()

                ' 2. Définir le schéma (créer des tables, etc.)
                Dim createAdminCommand As New MySqlCommand(
                    "CREATE TABLE IF NOT EXISTS Admins (" &
                    "ID INT AUTO_INCREMENT PRIMARY KEY," &
                    "Nom VARCHAR(100) NOT NULL," &
                    "Prenom VARCHAR(100) NOT NULL," &
                    "Email VARCHAR(255) UNIQUE," &
                    "Password VARCHAR(100) NOT NULL" &
                    ");", connection)
                createAdminCommand.ExecuteNonQuery()

                Dim createEmployesCommand As New MySqlCommand(
                    "CREATE TABLE IF NOT EXISTS Employes (" &
                    "ID INT AUTO_INCREMENT PRIMARY KEY," &
                    "Nom VARCHAR(100) NOT NULL," &
                    "Prenom VARCHAR(100) NOT NULL," &
                    "photo VARCHAR(255) NOT NULL" &
                    ");", connection)
                createEmployesCommand.ExecuteNonQuery()

                Dim createPresencesCommand As New MySqlCommand(
                    "CREATE TABLE IF NOT EXISTS Presences (" &
                    "ID INT AUTO_INCREMENT PRIMARY KEY," &
                    "userId INT NOT NULL," &
                    "dtae ATE) NOT NULL" &
                    ");", connection)
                createEmployesCommand.ExecuteNonQuery()
            End Using

        Catch ex As MySqlException
            MessageBox.Show("Erreur MySQL : " & ex.Message, "Erreur")
        Catch ex As Exception
            MessageBox.Show("Erreur : " & ex.Message, "Erreur")
        End Try
    End Sub
End Class
