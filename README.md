# Gestion Formation

Démonstration d'une application de gestion ecrite en CQRS/ES & MVVM

## Introduction

Après avoir suivi pas mal de conférences, lu plusieurs bouquins & articles sur le sujet, j'ai décidé fin 2017 de m'inscrire à l'excellente  [Formation DDD - CQRS/ES chez HackYourJob](https://formation.hackyourjob.com/catalogue/ddd-cqrs-eventsourcing.html) pour voir le sujet de façon plus pratique.

Au cours de cette formation, nous avons appris à ne plus imaginer et concevoir un système de manière technique (CRUD, base de données, tables, triggers, algo, UML, ...) mais en partant des évènements que doivent produire le système.

Si cette approche peut paraitre déroutante au premier abord, elle cache en elle une notion très puissante souvent négligée par les développeurs : la possibilité d'inclure des clients finaux qui ne connaissent rien à l'informatique à la conception du logiciel.

Ici, pas besoin de chef de projet, architecte, PO, business analyst, coach ou troubadour pour que les développeurs et les clients se comprennent. Il suffit que le client décrive les évènements qu'il vit tous les jours pour que les développeurs fassent émerger le "design" du système.

Tout cela parait beau sur le papier, mais qu'en est-il en réalité ?

Pour en avoir le coeur net, je suis parti en mission à la recherche d'un client prêt à me donner carte blanche pour créer une application de gestion non critique.

Le budget était de 40 jours, et le cahier des charges était des plus sommaire. Le service ADV du client gérait un centre de formation en interne, et devait se débrouiller avec une base Access pleine d'erreurs à cause du changement fréquent des modes d'organisation en interne.

Il fallait donc recueillir le besoin de ce service, constitué de personnes dont l'informatique se résume à la maitrise de la suite MSOffice afin de leur créer un logiciel sur mesure. Comme vous vous en doutez, le recueil du besoin ainsi que les artefacts de conceptions devaient rentrer dans les 40 jours !

![Challange accepted](https://media.giphy.com/media/LPrAK9rEedDwjtL1J0/giphy.gif)

## Déterminer le besoin

Le truc assez génial quand on commence à réfléchir évènement, c'est qu'il existe des ateliers qui permettent de faire participer un peu tout le monde à notre délire. Lors de la formation nous avons pu voir notamment un [Event storming](https://en.wikipedia.org/wiki/Event_storming) dont le but est de comprendre brièvement comment se comporte un domaine en faisant émerger les évènements du système.

C'est dans cette optique que j'ai donc organisé une réunion avec le service ADV et quelques managers.

Comme vous pouvez vous en douter, lorsque j'ai demandé aux gens de m'expliquer ce qu'ils voulaient, c'est parti dans tous les sens ! j'ai donc dû essayer de cadrer un peu les choses en expliquant que je n'attendais pas qu'on me décrive des écrans ou des fonctionnalités souhaitées, mais plutôt un mode de fonctionnement souhaité. Et pour décrire ce mode de fonctionnement, je voulais qu'on se concentre sur des évènements qui allait se passer.

Par exemple, les utilisateurs m'expliquent que pour commencer on va créer une session de formation, que cette session à des contraintes et qu'à l'intérieur  d'une session nous allons réserver des places à des stagiaires. Ces places devront être validées par un formateur pour être sûr que le stagiaire a le niveau, puis alors nous allons générer une convention de formation que le stagiaire devra retourner signée...

Pendant qu'on m'explique tout ça, je crée des postits que je colle au tableau :
- Session créée
- Place réservée
- Place validée
- Convention générée
- Convention signée
...

Les personnes autour de la table commencent à comprendre le concept, et envoient plein d'autres notions : une convention peut être annulée, une place peut être refusé, une session doit être clôturée, etc.

Au bout de 4H de taf, de nettoyage et de réorganisation de la pensée de chacun, une sorte de poulpe commence à émerger

![Event_storming](https://github.com/aboudoux/GestionFormation/blob/master/Captures/eventstorming.PNG)

## Développement

L'event storming n'est pas un cahier des charges gravé dans le marbre, c'est une aide à la compréhension. Ainsi au fur et à mesure que je crée l'ensemble de mes [evenements ](https://github.com/aboudoux/GestionFormation/tree/master/GestionFormation/CoreDomain) ainsi que leur [commandes associées](https://github.com/aboudoux/GestionFormation/tree/master/GestionFormation/Applications) le tout guidé par des [tests](https://github.com/aboudoux/GestionFormation/tree/master/GestionFormation.Tests), de nouvelles questions se posent et j'en discute directement avec le service ADV qui rectifie mes incompréhensions  ou leurs confusions interne.

Ces échanges font parfois éclater des débats houleux au sein du service sur tel ou tel concept, ce qui me permet d'affiner mon système.

Mes compétences en interfaces graphiques étant faibles (promis, je suis en train d'y [travailler](https://github.com/aboudoux/TetrisBlazor)) des premiers écrans commencent à sortir, comme par exemple

### Le login

![login](https://github.com/aboudoux/GestionFormation/blob/master/Captures/1.PNG)

### Le planning

![Planning formation](https://github.com/aboudoux/GestionFormation/blob/master/Captures/2.PNG)

### La gestion du referenciel

![Referenciel](https://github.com/aboudoux/GestionFormation/blob/master/Captures/3.png)

### La reservation des places

![Referenciel](https://github.com/aboudoux/GestionFormation/blob/master/Captures/5.PNG)

### La clôture d'une session

![Referenciel](https://github.com/aboudoux/GestionFormation/blob/master/Captures/6.PNG)

### Les actions de workflow

![Referenciel](https://github.com/aboudoux/GestionFormation/blob/master/Captures/7.PNG)

## La cerise sur le gateau

Lorsque je présente cette première bouture à l'équipe, le service ADV trouve qu'il y a quand même un peu trop de restrictions à son gout, comme par exemple le fait de ne pas pouvoir forcer une validation.

Cela fait bien sûr sursauter les managers qui veulent empêcher au maximum les malversations dans le système, mais le service ADV défend son point de vue en disant que souvent les "validateurs" ne sont pas dispo ce qui bloque certaines parties du processus.

Je me connecte donc en mode administrateur et explique qu'il existe une fonctionnalités cachée liée à notre architecture basée sur les évènements (event sourcing) : la possibilité d'avoir une trace de chaque modification apportée au système

![AuditLog](https://github.com/aboudoux/GestionFormation/blob/readme/Captures/9.png)

Effectivement, notre source de vérité ne tient que dans une seule table, la table "Event", toutes les autres tables ne sont que des conteneurs de données plates donc le but est de faire de la lecture. Toute modification du système doit passer par là, ce qui en assure une sorte de sécurité intrinsèque.

![Event](https://github.com/aboudoux/GestionFormation/blob/master/Captures/10.png)

En voyant cela, les managers se détendent et se disent que l'on peut effectivement mettre un peu plus de souplesse dans le système dans la mesure où on sera capable d'identifier qui et quand a fait une action.

# Du coup event sourcing ?

Cette première application mise en oeuvre dans l'esprit DDD m'a convaincu sur l'utilité de cette architecture et des méthodes associées.

Malheureusement, j'ai eu d'autres expériences  par la suite qui n'ont pas été aussi concluantes, car le client n'acceptait pas une collaboration direct entre les développeurs et les utilisateurs, ce qui conduisait à devoir discuter avec des "experts métiers" qui raisonnait exclusivement en terme de champs et formulaires basics sans possibilité de discussion.

pour ma part, ces expériences mettent en lumière une chose : Le succès d'un logiciel dépend du nombre d'intermédiaires qu'il existe entre les développeurs et leurs utilisateurs. Plus ce nombre et grand, plus la probabilité d'échec est grande. Le nombre idéal étant bien sûr 1 : lorsque le développeur est son propre utilisateur.

# Conclusion

Voilà, je vous laisse regarder le code et poser vos questions si besoin !
