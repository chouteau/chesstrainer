# Directives de développement pour Antigravity · ChessTrainer

Ce document définit les directives techniques, règles de codage et standards d'architecture pour tout agent intervenant sur la solution **ChessTrainer**.

---

## 🎯 Vue d'ensemble du projet
- **Nom officiel** : ChessTrainer
- **Objet** : Application web interactive d'entraînement aux échecs axée sur trois modes complémentaires :
  1. **Quiz des ouvertures** (`/quiz`) : reconnaissance visuelle de structures d'ouvertures à choix multiples.
  2. **Pièges d'ouverture** (`/`) : entraînement tactique interactif coup par coup avec réplique automatique adverse.
  3. **Entraînement aux coordonnées** (`/coordonnees`) : repérage spatial rapide sur l'échiquier avec options d'affichage graduées (Très Facile à Difficile) et orientation Blancs/Noirs.

---

## 🛠️ Stack technique & Standards .NET
- **Framework** : .NET 10 (ASP.NET Core Blazor Server).
- **Mode de rendu** : Interactive Server (`AddInteractiveServerRenderMode`) avec communication temps réel via SignalR.
- **Moteur d'échecs** : Moteur interne autonome en C# pur (`Chess/BoardState.cs`). Ne pas ajouter de librairie d'échecs externe sans accord explicite.
- **Dépendances** : Conserver la solution légère et sans dépendances NuGet tierces superflues.
- **Style C#** :
  - Nullable reference types activés (`<Nullable>enable</Nullable>`).
  - Espaces de noms à portée de fichier (*file-scoped namespaces*).
  - Utilisation du C# moderne (pattern matching, records, expressions de collection `[...]`).

---

## 🌐 Interface utilisateur & Langue
- **Langue de l'interface & Documentation** : Français obligatoire pour tous les textes visibles par l'utilisateur, libellés d'exercices, messages d'erreur et fichiers de documentation (`README.md`).
- **Composants** :
  - `PositionBoard.razor` pour les rendus d'échiquiers statiques (ex. Quiz).
  - `ChessBoard.razor` pour l'échiquier interactif complet.

---

## 🔒 Sécurité & Bonnes pratiques
- **XSS** : Ne jamais utiliser `MarkupString` avec des données arbitraires ou non sanitaires ; faire confiance à l'encodage Razor automatique.
- **Anti-forgery** : Maintenir `app.UseAntiforgery()` dans `Program.cs`.
- **Gestion des secrets** : Ne jamais inscrire de mot de passe, token, ou secret dans le code ou les fichiers de configuration commités (`appsettings.json`).

---

## 🐳 Conteneurisation & Déploiement en Production
- **Image Docker de base** : Image minimale durcie officielle Microsoft **Ubuntu Chiseled** (`mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled`).
- **Exécution sécurisée** : Utilisateur non-root obligatoire (`USER $APP_UID`).
- **Port d'écoute du conteneur** : `8080` (`ASPNETCORE_HTTP_PORTS=8080`).
- **Reverse Proxy Traefik & Orchestrateur Arcane** :
  - Déployé via `docker-compose.prod.yml` sur un VPS (réseau externe `traefik-public`, certresolver `le-http`).
  - Domaine de production : `chesstrainer.chouteau.info`.
  - Toujours conserver `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true` pour la gestion des en-têtes `X-Forwarded-*` et la compatibilité WebSockets/SignalR derrière Traefik.
  - Redirection automatique HTTP vers HTTPS via middleware Traefik.

---

## 🚀 Git & Intégration Continue (CI/CD)
- **Branche par défaut** : `main`.
- **Branche de production** : `prod`.
- **Registry** : GitHub Container Registry (`ghcr.io/chouteau/chesstrainer`).
- **Workflows CI/CD** :
  - Publication Docker : déclenchée automatiquement lors d'une Pull Request sur la branche `prod` (`.github/workflows/docker-publish.yml`).
  - Purge des anciennes images : planifiée quotidiennement et déclenchable manuellement avec simulation (`.github/workflows/docker-cleanup.yml`). Nettoie les versions de plus de 7 jours tout en préservant `latest` et la version la plus haute.
