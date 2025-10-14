#!/bin/bash

# Script Automático: Configurar Azure Deploy no GitHub
# Para Linux/Mac

set -e

# Cores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${CYAN}========================================"
echo -e "  Setup Azure Deploy - GitHub Actions"
echo -e "========================================${NC}"
echo ""

# PASSO 1: Verificar Azure CLI
echo -e "${YELLOW}[1/7] Verificando Azure CLI...${NC}"
if ! command -v az &> /dev/null; then
    echo -e "${RED}? Azure CLI não encontrado!${NC}"
    echo -e "${YELLOW}?? Instale com:${NC}"
    echo "  curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash"
    echo ""
    exit 1
fi
echo -e "${GREEN}? Azure CLI encontrado${NC}"
echo ""

# PASSO 2: Login no Azure
echo -e "${YELLOW}[2/7] Fazendo login no Azure...${NC}"
if az account show &> /dev/null; then
    echo -e "${GREEN}? Já está logado no Azure${NC}"
else
    echo -e "${YELLOW}?? Abrindo janela de login...${NC}"
    az login
    echo -e "${GREEN}? Login realizado com sucesso${NC}"
fi
echo ""

# PASSO 3: Verificar/Criar Resource Group
echo -e "${YELLOW}[3/7] Verificando Resource Group...${NC}"
if az group exists --name finansmart-rg | grep -q "false"; then
    echo -e "${YELLOW}?? Criando Resource Group 'finansmart-rg'...${NC}"
    az group create --name finansmart-rg --location brazilsouth
    echo -e "${GREEN}? Resource Group criado${NC}"
else
    echo -e "${GREEN}? Resource Group 'finansmart-rg' já existe${NC}"
fi
echo ""

# PASSO 4: Verificar/Criar App Service Plan
echo -e "${YELLOW}[4/7] Verificando App Service Plan...${NC}"
if ! az appservice plan show --name finansmart-plan --resource-group finansmart-rg &> /dev/null; then
    echo -e "${YELLOW}?? Criando App Service Plan 'finansmart-plan'...${NC}"
    az appservice plan create \
        --name finansmart-plan \
        --resource-group finansmart-rg \
        --sku B1 \
        --is-linux
    echo -e "${GREEN}? App Service Plan criado${NC}"
else
    echo -e "${GREEN}? App Service Plan 'finansmart-plan' já existe${NC}"
fi
echo ""

# PASSO 5: Verificar/Criar Web App
echo -e "${YELLOW}[5/7] Verificando Web App...${NC}"
if ! az webapp show --name finansmart-app --resource-group finansmart-rg &> /dev/null; then
    echo -e "${YELLOW}?? Criando Web App 'finansmart-app'...${NC}"
    az webapp create \
        --name finansmart-app \
        --resource-group finansmart-rg \
        --plan finansmart-plan \
        --runtime "DOTNET|8.0"
    echo -e "${GREEN}? Web App criado${NC}"
else
    echo -e "${GREEN}? Web App 'finansmart-app' já existe${NC}"
fi
echo ""

# PASSO 6: Obter Publish Profile
echo -e "${YELLOW}[6/7] Obtendo Publish Profile...${NC}"
az webapp deployment list-publishing-profiles \
    --name finansmart-app \
    --resource-group finansmart-rg \
    --xml > publish-profile.xml

if [ -f "publish-profile.xml" ]; then
    echo -e "${GREEN}? Publish Profile salvo em: publish-profile.xml${NC}"
else
    echo -e "${RED}? Erro ao obter Publish Profile${NC}"
    exit 1
fi
echo ""

# PASSO 7: Instruções
echo -e "${YELLOW}[7/7] Configuração do GitHub Secret${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

echo -e "${YELLOW}?? PRÓXIMOS PASSOS MANUAIS:${NC}"
echo ""
echo -e "${NC}1?? Copie o conteúdo do arquivo: publish-profile.xml${NC}"
echo -e "   ${BLUE}cat publish-profile.xml | pbcopy${NC}  # Mac"
echo -e "   ${BLUE}cat publish-profile.xml | xclip -selection clipboard${NC}  # Linux"
echo ""
echo -e "${NC}2?? Acesse o GitHub:${NC}"
echo -e "   ${CYAN}https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions${NC}"
echo ""
echo -e "${NC}3?? Clique em 'New repository secret'${NC}"
echo ""
echo -e "${NC}4?? Preencha:${NC}"
echo -e "   Name: ${GREEN}AZURE_WEBAPP_PUBLISH_PROFILE${NC}"
echo -e "   Value: ${GREEN}[Cole o conteúdo XML]${NC}"
echo ""
echo -e "${NC}5?? Clique em 'Add secret'${NC}"
echo ""
echo -e "${NC}6?? Faça push para testar:${NC}"
echo -e "   ${BLUE}git add .${NC}"
echo -e "   ${BLUE}git commit -m 'chore: Configure Azure deployment'${NC}"
echo -e "   ${BLUE}git push origin main${NC}"
echo ""

echo -e "${CYAN}========================================${NC}"
echo -e "${GREEN}? Setup Azure Completo!${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

echo -e "${YELLOW}?? INFORMAÇÕES DO DEPLOY:${NC}"
echo -e "  Resource Group: finansmart-rg"
echo -e "  App Service Plan: finansmart-plan (B1)"
echo -e "  Web App: finansmart-app"
echo -e "  URL: ${CYAN}https://finansmart-app.azurewebsites.net${NC}"
echo -e "  Swagger: ${CYAN}https://finansmart-app.azurewebsites.net/swagger${NC}"
echo ""

echo -e "${YELLOW}?? SEGURANÇA:${NC}"
echo -e "  ${RED}?? NÃO COMMITE o arquivo publish-profile.xml!${NC}"
echo -e "  ${GREEN}? Ele já está no .gitignore${NC}"
echo ""

echo -e "${YELLOW}Deseja ver o conteúdo do publish-profile.xml? (y/n):${NC} "
read -r response
if [[ "$response" =~ ^[Yy]$ ]]; then
    cat publish-profile.xml
fi

echo ""
echo -e "${YELLOW}Deseja abrir a página de GitHub Secrets? (y/n):${NC} "
read -r response
if [[ "$response" =~ ^[Yy]$ ]]; then
    if command -v open &> /dev/null; then
        open "https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions"
    elif command -v xdg-open &> /dev/null; then
        xdg-open "https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions"
    else
        echo "Abra manualmente: https://github.com/Leandro-Solany/Finansmart/settings/secrets/actions"
    fi
fi

echo ""
echo -e "${NC}Pressione Enter para finalizar...${NC}"
read -r
