##Baseado em https://github.com/AlexSugak/dotnet-core-tdd/blob/master/Makefile

build:
	dotnet build GeradorDeDados.csproj

package:
	dotnet add GeradorDeDados.csproj package $(ARGS)

rebuild-docker: 
	docker-compose down
	docker-compose build --no-cache
	docker-compose up -d

restart-docker: 
	docker-compose down
	docker-compose up -d