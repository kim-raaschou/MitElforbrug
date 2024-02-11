# SmartLearning: Cloud-baseret POC af Visualisering Mit Elforbrug

## Optimering af elforbrug

Jeg har længe haft fokus på vores elforbrug i hjemmet, og på hvordan vi eventuelt kan optimere vores forbrug af el hen over døgnets 24 timer. Dette er ikke blevet mindre relevant efter vi har fået elbil og vores elforbrug derfor er steget betragteligt. 

Som eksamensopgave i faget Cloud, vil jeg derfor udvikle en serveless applikation, med der formål at indsamle og præsentere mit elforbrug.
Dette kan gøres ved at kalde den åbne datahub, der udstiller Api'er for ens elfoprbrug (https://energinet.dk/data-om-energi/datahub). Disse data vil jeg holde op i mod el spotpriser fra Energinettet - der udstilles via energidataservice.dk.

Løsningen bliver udviklet på .Net i C# og med nogle simple Web Components via Carbon Design System.

Formålet er at visualisere forbrug i forhold til pris, og dermed skabe et muligt overblik omkring eventuelle uhensigtsmæssigheder i forbrug og/eller potentiale for bedre udnyttelse at pris kontra forbrug.
Det er muligt at finde sit elforbrug hos sin eludbyder, men den er som regel ikke særlig specifik, og der er ikke nogle spotpriser for ens forbrug. 

Produktet bliver udviklet som POC, og kan være et oplæg til diskussion i forhold til sikkerhed, opbevaring af date, performance mv. 
