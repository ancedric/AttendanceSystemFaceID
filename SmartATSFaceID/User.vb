Option Strict On
Option Explicit On

Public MustInherit Class User
    Private nom As String
    Private prenom As String

    Public Sub New(nom As String, prenom As String)
        Me.nom = nom
        Me.prenom = prenom
    End Sub

    Public MustOverride Sub createUser() 'Méthode pour ajouter un utilisateur à la base de données
    Public MustOverride Sub updateUser() 'Méthode pour mettre à jour les informations d'un utilisateur dans la base de données
    Public MustOverride Sub deleteUser() 'Méthode pour supprimer un utilisateur dans la base de données

End Class
