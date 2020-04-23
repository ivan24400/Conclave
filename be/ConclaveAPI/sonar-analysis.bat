dotnet sonarscanner begin /k:"conclave" 
dotnet build "./Conclave"
dotnet sonarscanner end
pause