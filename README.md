# SmartLearning Cloud: POC Visualisering af Mit Elforbrug

## Indledning
Fokus på hjemligt elforbrug, især efter indførelsen af en elbil, motiverer udviklingen af en serverløs applikation i Azure.

## Problemformulering
I Cloud-faget søger jeg med POC'en at indsamle og præsentere elforbrugsdata for at identificere og optimere forbrugsmønstre, især med henblik på at håndtere stigningen i elforbrug efter elbilanskaffelsen.

## Teoretiske Grundlag
Netværksteori og serverless functions i Azure vil muliggøre effektiv kommunikation og skalerbaritet. Anvendelse af HTTP/HTTPS-protokoller og Azure Functions støtter i udviklingen af POC'en.

## Produkt
POC'en anvender Azure Functions (.Net 8.0/C#) og Web Components til visualisering. Disse teknologier sikrer skalerbarhed og en brugervenlig grænseflade.

## Literaturliste
1. [Energinet - Åbne el-datahub](https://energinet.dk/data-om-energi/datahub)
2. [Energidataservice - El-spotpriser](https://www.energidataservice.dk)
3. [Carbon Design System](https://carbondesignsystem.com)








########################################




#SmartLearning Cloud: POC Visualisering af Mit Elforbrug

## Indledning
Med fokus på vores hjemlige elforbrug, søger jeg efter måder at optimere det i løbet af døgnet, især efter vi har anskafet os en elbil, hvilket har resulteret i en markant stigning i vores elforbrug.

## Problemformulering
I forbindelse med min eksamensopgave inden for Cloud-faget har jeg besluttet mig for at udvikle en serverløs applikationc i Azure, med det formål at indsamle og præsentere data om mit elforbrug. 

Med dette projekt er at skabe en visualisering af elforbruget i forhold til pris, med henblik på at identificere potentielle uhensigtsmæssigheder i forbrugsmønstre, og her af muligheder for at optimere vores elforbrug. Selvom det er muligt at finde elforbrugsdata hos ens eludbyder, er denne information normalt ikke tilstrækkelig specifik, og der er sjældent tilgængelige spotpriser for ens individuelle forbrug.

## Teoretiske grundlag
Projektets teoretiske grundlag bygger på netværksteori og anvendelsen af serverless functions i Azure.

### Netværksteori:
Netværksteorien udgør fundamentet for kommunikationen mellem applikationen og de eksterne API'er. Ved brug af HTTP/HTTPS-protokollen på applikationslaget muliggøres effektiv kommunikation mellem applikationen og eksterne API'er. Dette, i kombination med datalink- og transportlagene, der udnytter Ethernet og TCP-protokollen, sikrer pålidelig og sikker datatransmission.

### Serverless Functions i Azure:
Valget af serverless functions i Azure, specifikt Azure Functions (.Net 8.0/C#), baserer sig på deres evne til at levere individuelle funktioner uden behov for permanent infrastruktur. Dette giver mulighed for at skabe en skalerbar og omkostningseffektiv løsning.

### Produkt
Opgave realiseres ved at tilgå den åbne el-datahub (1), der tilbyder API'er til elforbrugsdata, og kombinere disse data med el-spotpriser fra Energinettet (2).

Min valgte løsning tager form af Azure Functions (.Net 8.0/C#), og en statisk web-side opbygges ved hjælp af Web Components (3).

Produktet udvikles som en Proof of Concept (POC), og det vil danne grundlag for diskussioner om sikkerhed, dataopbevaring, performance og andre relevante emner under den mundtlige eksamination.

### Literaturliste
1. https://energinet.dk/data-om-energi/datahub
2. https://www.energidataservice.dk
3. https://carbondesignsystem.com



#########################







# SmartLearning: Cloud-baseret opgave 

## POC Visualisering af Mit Elforbrug

Jeg har længe haft fokus på vores elforbrug i hjemmet, og på hvordan vi eventuelt kan optimere vores forbrug hen over døgnets 24 timer. Dette er ikke blevet mindre relevant efter vi har fået elbil og vores elforbrug derfor er steget betragteligt.

Som eksamensopgave i faget Cloud, vil jeg derfor udvikle en serveless applikation, med det formål at indsamle og præsentere mit elforbrug.
Dette gøres ved at kalde den åbne el-datahub, der udstiller Api'er for ens elfoprbrug (https://energinet.dk/data-om-energi/datahub), og kombinere disse med el-spotpriser fra Energinettet (energidataservice.dk).

Løsningen bliver udviklet som Azure Functions (.Net 8.0/C#) med en statisk web-side opbygget af Web Components (https://carbondesignsystem.com/).

Formålet er at visualisere forbrug i forhold til pris, og dermed skabe et muligt overblik omkring eventuelle uhensigtsmæssigheder i forbrug og/eller potentiale for bedre udnyttelse at pris kontra forbrug.
Det er muligt at finde sit elforbrug hos sin eludbyder, men den er som regel ikke særlig specifik, og der er ikke nogle spotpriser for ens forbrug. 

Produktet bliver udviklet som POC, og kan være et oplæg til diskussion i forhold til sikkerhed, opbevaring af date, performance mv. 
