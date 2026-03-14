### Consignes .NET: 
* Ignorez les migrations BDD
* Ne pas modifier les classes qui ont un commentaire: `// WARN: Should not be changed during the exercise
`
* Pour lancer les tests: `dotnet test`

## Ce que j'ai fait

J'ai refactorisé le code en séparant les responsabilités en couches claires :

- **Controller** : reçoit la requête et retourne la réponse, rien de plus
- **Service** : contient toute la logique métier
- **Repository** : gère les accès à la base de données
- **DTO + AutoMapper** : pour ne pas exposer directement les entités de la base de données

## Ce que j'aurais amélioré avec plus de temps

- Rendre les appels base de données asynchrones (`SaveChangesAsync`, `SingleOrDefaultAsync`)
  pour améliorer les performances et ne pas bloquer le thread principal.

- Déplacer les `SaveChanges` dans le Repository pour que le Service
  ne s'occupe que du métier, sans savoir comment les données sont sauvegardées.