//MATCH (n) DETACH DELETE n;
//RADEK VYSE ODKOMENTUJE V PRIPADE ZE MA DOJIT KE SMAZANI DAT A ZAROVEN ZAKOMENTUJE NASLEDUJICI RADKY, KOMENTOVANI SE DELA DVEMA LOMITKY

//VYTVORENI HRACU

WITH ["Jan", "Jiří", "Pavel", "Martin", "Tomáš", "Petr", "Josef", "Miroslav", "Jakub", "František", "Zdeněk", "Jaroslav", "Michal", "Milan", "Václav", "David", "Karel", "Lukáš", "Stanislav", "Jindřich"] AS firstNames
WITH firstNames, ["Novák", "Svoboda", "Novotný", "Dvořák", "Černý", "Procházka", "Kučera", "Veselý", "Horák", "Němec", "Král", "Krejčí", "Pokorný", "Růžička", "Hájek", "Jelínek", "Marek", "Šťastný", "Pospíšil", "Urban"] AS lastNames
WITH firstNames, lastNames, ["Prague", "Brno", "Ostrava", "Plzeň", "Liberec", "Olomouc", "Ústí nad Labem", "Hradec Králové", "České Budějovice", "Pardubice", "Havířov", "Zlín", "Kladno", "Most", "Karlovy Vary", "Opava", "Frýdek-Místek", "Karviná", "Jihlava", "Teplice"] AS cities
FOREACH(ignoreMe IN range(1, 100) |
    CREATE (:Player {
        FirstName: firstNames[toInteger(ROUND(rand() * (size(firstNames) - 1)))],
        LastName: lastNames[toInteger(ROUND(rand() * (size(lastNames) - 1)))],
        AccountId: "Account" + toString(ignoreMe),
        Id: toString(ignoreMe),
        ChessClub: cities[toInteger(ROUND(rand() * (size(cities) - 1)))],
		Country: "Czechia"
    })
)

//VYTVORENI HRACEK

WITH ["Anna", "Eva", "Martina", "Jana", "Petra", "Michaela", "Lucie", "Lenka", "Markéta", "Veronika", "Kateřina", "Alena", "Barbora", "Simona", "Tereza", "Ivana", "Monika", "Hana", "Natálie", "Kristýna"] AS firstNames
WITH firstNames, ["Nováková", "Svobodová", "Novotná", "Dvořáková", "Černá", "Procházková", "Kučerová", "Veselá", "Horáková", "Němcová", "Králíčková", "Krejčířová", "Pokorná", "Růžičková", "Hájková", "Jelínková", "Marková", "Šťastná", "Pospíšilová", "Urbanová"] AS lastNames
WITH firstNames, lastNames, ["Prague", "Brno", "Ostrava", "Plzeň", "Liberec", "Olomouc", "Ústí nad Labem", "Hradec Králové", "České Budějovice", "Pardubice", "Havířov", "Zlín", "Kladno", "Most", "Karlovy Vary", "Opava", "Frýdek-Místek", "Karviná", "Jihlava", "Teplice"] AS cities
FOREACH(ignoreMe IN range(101, 200) |
    CREATE (:Player {
        FirstName: firstNames[toInteger(ROUND(rand() * (size(firstNames) - 1)))],
        LastName: lastNames[toInteger(ROUND(rand() * (size(lastNames) - 1)))],
        AccountId: "Account" + toString(ignoreMe),
        Id: toString(ignoreMe),
        ChessClub: cities[toInteger(ROUND(rand() * (size(cities) - 1)))],
		Country: "Czechia"
    })
);


//VYTVORENI PROFILU UCTUM adminmail@email.cz az adminmail5@email.cz

WITH ["Jan", "Jiří", "Pavel", "Martin", "Tomáš", "Petr", "Josef", "Miroslav", "Jakub", "František", "Zdeněk", "Jaroslav", "Michal", "Milan", "Václav", "David", "Karel", "Lukáš", "Stanislav", "Jindřich"] AS firstNames
WITH firstNames, ["Novák", "Svoboda", "Novotný", "Dvořák", "Černý", "Procházka", "Kučera", "Veselý", "Horák", "Němec", "Král", "Krejčí", "Pokorný", "Růžička", "Hájek", "Jelínek", "Marek", "Šťastný", "Pospíšil", "Urban"] AS lastNames
WITH firstNames, lastNames, ["Prague", "Brno", "Ostrava", "Plzeň", "Liberec", "Olomouc", "Ústí nad Labem", "Hradec Králové", "České Budějovice", "Pardubice", "Havířov", "Zlín", "Kladno", "Most", "Karlovy Vary", "Opava", "Frýdek-Místek", "Karviná", "Jihlava", "Teplice"] AS cities
CREATE (:Player {
        FirstName: firstNames[toInteger(ROUND(rand() * (size(firstNames) - 1)))],
        LastName: lastNames[toInteger(ROUND(rand() * (size(lastNames) - 1)))],
        AccountId: "2dee72d6-8e62-4373-80d5-dcfb9a616040",
        Id: "659a6624-dd98-564568f2-03baa6d04ec3",
        ChessClub: cities[toInteger(ROUND(rand() * (size(cities) - 1)))],
		Country: "Czechia"
    })
	
CREATE (:Player {
        FirstName: firstNames[toInteger(ROUND(rand() * (size(firstNames) - 1)))],
        LastName: lastNames[toInteger(ROUND(rand() * (size(lastNames) - 1)))],
        AccountId: "659a6624-dd98-4b75-88f2-03baa6d04ec3",
        Id: "659a662445658-4b75-88f2-03baa6d04ec3",
        ChessClub: cities[toInteger(ROUND(rand() * (size(cities) - 1)))],
		Country: "Czechia"
    })
	
CREATE (:Player {
        FirstName: firstNames[toInteger(ROUND(rand() * (size(firstNames) - 1)))],
        LastName: lastNames[toInteger(ROUND(rand() * (size(lastNames) - 1)))],
        AccountId: "196cfbae-6bab-4586-9e57-5259380fb079",
        Id: "196cfbae-6bab-4586-9e57sdff80fb079",
        ChessClub: cities[toInteger(ROUND(rand() * (size(cities) - 1)))],
		Country: "Czechia"
    })
	
CREATE (:Player {
        FirstName: firstNames[toInteger(ROUND(rand() * (size(firstNames) - 1)))],
        LastName: lastNames[toInteger(ROUND(rand() * (size(lastNames) - 1)))],
        AccountId: "c2b9ae5e-9da2-429b-a67a-b98365ad8e35",
        Id: "c2b9ae5e56985-429b-a67a-b98365ad8e35",
        ChessClub: cities[toInteger(ROUND(rand() * (size(cities) - 1)))],
		Country: "Czechia"
    })
	
CREATE (:Player {
        FirstName: firstNames[toInteger(ROUND(rand() * (size(firstNames) - 1)))],
        LastName: lastNames[toInteger(ROUND(rand() * (size(lastNames) - 1)))],
        AccountId: "cb5f7c5e-2337-44b8-b926-6c60e61a11fb",
        Id: "cb5f7c5e-9867-44b8-b926-6c60e61a11fb",
        ChessClub: cities[toInteger(ROUND(rand() * (size(cities) - 1)))],
		Country: "Czechia"
    });


//VYTVORENI TURNAJU PRO JEDNOTLIVCE, KTERE JSOU SPRAVOVANE UCTEM s loginem adminmail@email.cz

CREATE (tournament:Tournament {Id: '52sdf22c5655', StartDate: date('2024-05-06'), EndDate: date('2024-06-06'), TournamentName: 'RoundRobin jednotlivci', ShortDescription: 'všichni proti všem', CurrentRound: 0, ExpectedNumberOfRound: 0, TournamentType: 'RoundRobin', IsTeam: false, HasStarted: false})
WITH tournament
MATCH (player:Player { AccountId : '2dee72d6-8e62-4373-80d5-dcfb9a616040' })
CREATE (tournament)-[:MANAGED_BY]->(player);

CREATE (tournament:Tournament {Id: '52s4613315655', StartDate: date('2024-05-06'), EndDate: date('2024-06-06'), TournamentName: 'PlayOff jednotlivci', ShortDescription: 'vyřazovací', CurrentRound: 0, ExpectedNumberOfRound: 0, TournamentType: 'Playoff', IsTeam: false, HasStarted: false})
WITH tournament
MATCH (player:Player { AccountId : '2dee72d6-8e62-4373-80d5-dcfb9a616040' })
CREATE (tournament)-[:MANAGED_BY]->(player);

CREATE (tournament:Tournament {Id: '52s4613xvcvv655', StartDate: date('2024-05-06'), EndDate: date('2024-06-06'), TournamentName: 'Swiss jednotlivci', ShortDescription: 'švýcarský systém', CurrentRound: 0, ExpectedNumberOfRound: 0, TournamentType: 'Swiss', IsTeam: false, HasStarted: false})
WITH tournament
MATCH (player:Player { AccountId : '2dee72d6-8e62-4373-80d5-dcfb9a616040' })
CREATE (tournament)-[:MANAGED_BY]->(player);

//PRIHLASENI DVACETI HRACU NA KAZDY TURNAJ - POKUD JICH CHCETE MIN, STACI SE ODHLASIT V SEKCI REGISTROVANI HRACI SE SPRAVOU TURNAJE - DLE UZIVATELSKE DOKUMENTACE
//CISLO SI ZDE MUZETE NASTAVIT NA LIBOVOLNOU HODNOTU (MAXIMALNE POCET HRACU V DATABAZI - VIZ SKRIPTY VYSE), PRIPADNE SI JE PRIHLASIT V MANUALNI REGISTRACI VE SPRAVE TURNAJE
MATCH (p:Player)
WITH p
LIMIT 16
MATCH (t:Tournament) WHERE t.IsTeam = false
WITH p, t
CREATE (p)-[:PLAYS_IN]->(t);


//VYTVORENI TURNAJU PRO TYMY, KTERE JSOU OPET SPRAVOVANY UCTEM S LOGINEM adminmail@email.cz

CREATE (tournament:Tournament {Id: '52sdf2sdffsc5', StartDate: date('2024-05-06'), EndDate: date('2024-06-06'), TournamentName: 'RoundRobin tymy', ShortDescription: 'všichni proti všem', CurrentRound: 0, ExpectedNumberOfRound: 0, TournamentType: 'RoundRobin', IsTeam: true, HasStarted: false})
WITH tournament
MATCH (player:Player { AccountId : '2dee72d6-8e62-4373-80d5-dcfb9a616040' })
CREATE (tournament)-[:MANAGED_BY]->(player)
WITH tournament
CREATE (settings:TeamDrawSettings {TeamSizeForRound: 2})
CREATE (settings)<-[:HAS_TEAMDRAW_SETTINGS]-(tournament);

CREATE (tournament:Tournament {Id: '52s46lkjoi55', StartDate: date('2024-05-06'), EndDate: date('2024-06-06'), TournamentName: 'PlayOff tymy', ShortDescription: 'vyřazovací', CurrentRound: 0, ExpectedNumberOfRound: 0, TournamentType: 'Playoff', IsTeam: true, HasStarted: false})
WITH tournament
MATCH (player:Player { AccountId : '2dee72d6-8e62-4373-80d5-dcfb9a616040' })
CREATE (tournament)-[:MANAGED_BY]->(player)
WITH tournament
CREATE (settings:TeamDrawSettings {TeamSizeForRound: 2})
CREATE (settings)<-[:HAS_TEAMDRAW_SETTINGS]-(tournament);

CREATE (tournament:Tournament {Id: '52s46kjxckvh55', StartDate: date('2024-05-06'), EndDate: date('2024-06-06'), TournamentName: 'Swiss tymy', ShortDescription: 'švýcarský systém', CurrentRound: 0, ExpectedNumberOfRound: 0, TournamentType: 'Swiss', IsTeam: true, HasStarted: false})
WITH tournament
MATCH (player:Player { AccountId : '2dee72d6-8e62-4373-80d5-dcfb9a616040' })
CREATE (tournament)-[:MANAGED_BY]->(player)
WITH tournament
CREATE (settings:TeamDrawSettings {TeamSizeForRound: 2})
CREATE (settings)<-[:HAS_TEAMDRAW_SETTINGS]-(tournament);


//VYTVORENI TYMU, KTERE JSOU OPET PRO JEDNODUCHOST SPRAVOVANY UCTEM S LOGINEM adminmail@email.cz

WITH range(1, 8) AS rangeList
UNWIND rangeList AS i
MATCH (player1:Player {Id : toString(i * 2)})
MATCH (player2:Player {Id : toString(i * 2 + 1)})
CREATE (team:Team {
  Id: toString(i * 123456),
  Name: 
  CASE i
    WHEN 1 THEN "Bílá věž"
    WHEN 2 THEN "Černý král"
    WHEN 3 THEN "Dáma"
    WHEN 4 THEN "Střelec"
    WHEN 5 THEN "Pěšec"
    WHEN 6 THEN "Rosada"
    WHEN 7 THEN "Šach"
    WHEN 8 THEN "Mat"
  END
})
CREATE (player1)-[:PLAYS_FOR {OrderNumber: 0}]->(team)
CREATE (player2)-[:PLAYS_FOR {OrderNumber: 1}]->(team);


MATCH (player:Player { AccountId : '2dee72d6-8e62-4373-80d5-dcfb9a616040' })
MATCH (team:Team)
CREATE (team)-[:MANAGED_BY]->(player);


//PRIHLASENI VSECH TYMU NA TYMOVE TURNAJE
MATCH (team:Team)
MATCH (tournament:Tournament) WHERE tournament.IsTeam = true
CREATE (team)-[:PLAYS_IN]->(tournament);