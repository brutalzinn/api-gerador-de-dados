# API-GERADOR-DE-DOCUMENTOS V1

1. Gera um cnpj usando o algoritmo de CNPJ.
2. Consulta a receitaWS para tentar obter dados para o CNPJ gerado.
3. Salva o dado obtido em cache no Redis.
4. Expurga os CNPJS gerados todo domingo e reinicia o processo

# Recursos

- Auto preenchimento de cnpjs com quantidade mínima e máxima
- Auto expurgo de dados com padrão CronTab
- Cria placeholders para mockar textos

# Tela de testes

https://util.robertocpaes.dev/
https://previewutil.robertocpaes.dev/