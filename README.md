# SmartLearning Cloud: POC Visualisering af Mit Elforbrug

## Indledning
Med fokus på vores hjemlige elforbrug, søger jeg efter måder at optimere det i løbet af døgnet, især efter vi har anskafet os en elbil, hvilket har resulteret i en markant stigning i vores elforbrug.

## Problemformulering
I forbindelse med min eksamensopgave inden for Cloud-faget har jeg besluttet mig for at udvikle en serverløs applikationc i Azure, med det formål at indsamle og præsentere data om mit elforbrug. 

Jeg vil forsøge at skabe en visualisering af mit elforbrug i forhold til spotpris, med henblik på at identificere potentielle uhensigtsmæssigheder i forbrugsmønstre, og her af muligheder for at optimere på vores elforbrug. 

## Teoretiske grundlag
Projektets teoretiske grundlag bygger på netværksteori og anvendelsen af serverless functions i Azure.

### Netværksteori:
OSI-modellen udgør fundamentet for kommunikationen mellem applikationen og de eksterne API'er. Ved brug af HTTPS-protokollen på applikationslaget muliggøres sikker kommunikation mellem applikationen og de eksterne API'er. Dette, i kombination med datalink- og transportlagene, der benytter Ethernet og TCP/IP-protokollen og dermed sikrer en pålidelig datatransmission.

### Serverless Functions i Azure:
Valget af serverless functions i Azure, specifikt Azure Functions (.Net 8.0/C#), baserer sig på deres evne til at levere individuelle funktioner uden behov for permanent infrastruktur. Dette giver mulighed for at skabe en skalerbar og omkostningseffektiv løsning.

### Produkt
Opgave realiseres ved at tilgå den åbne el-datahub (1), der tilbyder API'er til elforbrugsdata, og kombinere disse data med el-spotpriser fra Energinettet (2).

Min valgte løsning tager form af Azure Functions (.Net 8.0/C#), og en statisk web-side opbygges ved hjælp af Web Components (3).

Produktet udvikles som en Proof of Concept (POC), og det vil danne grundlag for diskussioner om sikkerhed, dataopbevaring, performance og andre relevante emner under den mundtlige eksamination.

**Det endelige produkt kan findes her: https://blue-beach-0e39cba03.4.azurestaticapps.net**

### Literaturliste
1. https://energinet.dk/data-om-energi/datahub
2. https://www.energidataservice.dk
3. https://carbondesignsystem.com
