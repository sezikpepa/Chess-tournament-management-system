Ve složkách se nacházejí jednotlivé projekty aplikace. V Dockerfile je definována její kompilace, v souboru docker-compose.yml jsou definovány kontejnery pro databázové systémy a samotnou aplikaci. 
Pokud příkaz na compose aplikace spouštíme poprvé a není tím pádem ještě vytvořené volume, nic neměníme. Pokud ale již vytvořeno je, změníme v souboru .env jedinou proměnnou, která tam 
je, na false, a tím pádem se již nebude vytvářet schéma. Mimo jiné je na řádce 37 v docker-compose.yml je číslo 90. To udává, kolik vteřin se má čekat, než se skript na vytvoření schématu 
spustí - po vytvoření kontejneru s databází. Jelikož image Microsoft SQL Server neposkytuje jinou možnost jak schéma vytvořit, je to udělané takhle nepěkně pomocí dalšího kontejneru. Jelikož server ještě chvíli
nabíhá, tak je potřeba chvíli počkat s vytvářením schématu. Kdyby to váš počítač nestihl za 90 vteřin spustit, je možné tento čas zvýšit. U mě to trvá asi 15 vteřin, číslo jsem pro jistotu 
velmi nadsadil. Když se schéma správně vytvoří, ukáže se zpráva s několika vykřičníky, že lze aplikaci používat. Pravděpodobně po naběhnutí kontejneru s neo4j, se vám nebude v logu nic 
vypisovat, a poté po přibližně minutě dojde k vytvoření databázové struktury. To, že byla databáze s účty úspěšně vytvořena, je oznámeno zprávou 
"Database with accounts has correct structure, you can use the app!!!!!!!!!!!!!!!!!!!" (vykřičkníky jsou tam, aby to bylo vidět mezi ostatními zprávami). Aplikaci poté můžete testovat
na localhost na portu 9000

V kořenové složce s Dockerfile lze aplikaci jednoduše spustit příkazem 

-------------------------
docker-compose up --build
-------------------------


Následující příkazy vytvoří testovací data. Skripty je nutné spouštět, až když kontejner běží, ve složce kde se daný skript nachází. Na Windows fungují pouze v běžném příkazovém řádku, 
nikoliv v Powershellu. Powershell neakceptuje znak "<". Na WSL s běžícím Ubuntu fungovalo spuštění také bez problému.

První skript vytvoří turnaje, hráče a týmy. (Turnaje, hráči i týmy byli vygenerováni pomocí různých nástrojů, aby byla zajištěna diverzita jmen, a nejsou mojím dílem)

!!!!Nespouštějte skript na vytváření dat více než jednou, jelikož některé vrcholy, mají unikátní Id. Opakované spuštění by vytvořilo entity s duplicitním Id, což by způsobilo nefunkčnost
aplikace v některých částech. Pokud by bylo potřeba nahrát data znovu, stačí odkomentovat první řádku skriptu, a poté skript stejným způsobem použít. Tato akce provede kompletní
výmaz databáze.

Případné změny testovacích dat lze udělat v souboru data.cypher

----------------------------------------------------------------------------
docker exec -i neo4jdatabase cypher-shell -u neo4j -p 12345678 < data.cypher
----------------------------------------------------------------------------

Druhý skript vytvoří deset testovacích účtů. Prvních pět má již vyplněné jméno a příjmení, zbylých pět je po čistém vytvoření. Emaily byly samozřejmě ověřeny manuálně a v realitě 
neexistují a jsou pouze pro testovací účely. Na otestování přihlašování do turnajů, vytváření týmů a turnajů by měl stačit tento počet. Na testování losování, jsou již vytvořené turnaje
s přihlášenými hráči. Všechny turnaje jsou spravované prvním účtem!!!. První přihlášení může trvat i několik vteřin, jelikož dochází k inicializaci databáze, poté již probíhá vše normálně. 
Jak je zmíněno v textové části, aby bylo možné vytvářet nové účty, bylo by nutné nakonfigurovat emailovou stránku vlastními údaji!!!

--------------------------------------------------------------------------------------------------------------------------------------------------------
docker exec -i sqlserverdatabaseaccounts /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrongPassword123! -d AccountsDatabase < dataAccounts.sql
--------------------------------------------------------------------------------------------------------------------------------------------------------

Vytvoří se účty s údaji níže:

adminmail@email.cz
AdminHeslo123!

adminmail2@email.cz
AdminHeslo123!

adminmail3@email.cz
AdminHeslo123!

adminmail4@email.cz
AdminHeslo123!

adminmail5@email.cz
AdminHeslo123!

adminmail6@email.cz
AdminHeslo123!

adminmail7@email.cz
AdminHeslo123!

adminmail8@email.cz
AdminHeslo123!

adminmail9@email.cz
AdminHeslo123!

adminmail10@email.cz
AdminHeslo123!