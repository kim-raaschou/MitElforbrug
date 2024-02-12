# SmartLearning Cloud: POC Visualisering af Mit Elforbrug

## Indledning
Med fokus på vores hjemlige elforbrug, søger jeg efter måder at optimere det i løbet af døgnet. Især efter, at vi har anskafet os en elbil, har vores daglige elforbrug ændret sig, og der kan være et potentiale i at have et overordnet overblik over vores forbrug og forbrugsmønstre.

## Problemformulering
I forbindelse med min eksamensopgave inden for Cloud-faget har jeg besluttet mig for at udvikle en serverløs applikationc i Azure, med det formål at indsamle og præsentere data om mit elforbrug. 

Jeg vil forsøge at skabe en visualisering af mit elforbrug i forhold til spotpris, med henblik på at identificere potentielle uhensigtsmæssigheder i forbrugsmønstre, og her af muligheder for at optimere på vores elforbrug. 

## Teoretiske grundlag
Projektets teoretiske grundlag bygger på netværksteori og anvendelsen af serverless functions i Azure.

### Netværksteori:
OSI-modellen udgør fundamentet for kommunikationen mellem applikationen og de eksterne API'er. Ved brug af HTTPS-protokollen på applikationslaget muliggøres sikker kommunikation mellem applikationen og de eksterne API'er. Yderligere sikres pålidelig datatransmission mellem server og klienter ved brug af Ethernet og TCP/IP-protokollen på henholdsvis datalink- og transportlagene.

### Serverless Functions i Azure:
Valget af serverless functions i Azure, specifikt Azure Functions (.Net 8.0/C#), baserer sig på deres evne til at levere individuelle funktioner uden behov for permanent infrastruktur. Dette giver mulighed for at skabe en skalerbar og især omkostningseffektiv løsning.

## Produkt
Opgave realiseres ved at tilgå den åbne el-datahub (1), der tilbyder API'er til elforbrugsdata, og kombinere disse data med el-spotpriser fra Energinettet (2).

Min valgte løsning tager form af Azure Functions (.Net 8.0/C#), og en statisk web-side opbygget ved hjælp af Web Components (3).

Produktet udvikles som en Proof of Concept (POC), og vil danne grundlag for diskussioner om sikkerhed, dataopbevaring, performance og andre relevante emner under den mundtlige eksamination.

>Det endelige produkt kan findes her: https://blue-beach-0e39cba03.4.azurestaticapps.net

## Literaturliste
1. https://energinet.dk/data-om-energi/datahub
2. https://www.energidataservice.dk
3. https://carbondesignsystem.com
