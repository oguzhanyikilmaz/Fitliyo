#!/bin/bash
set -e

# ─── Fitliyo — Tek Komutla Deploy ───────────────────────────
# Kullanım: ./deploy.sh [up|down|restart|logs|status]

COMPOSE_FILE="docker-compose.yml"
PROJECT_NAME="fitliyo"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

print_banner() {
    echo ""
    echo -e "${CYAN}╔══════════════════════════════════════╗${NC}"
    echo -e "${CYAN}║        🏋️  Fitliyo Deploy            ║${NC}"
    echo -e "${CYAN}║    Spor & Sağlık Koçluğu Platform    ║${NC}"
    echo -e "${CYAN}╚══════════════════════════════════════╝${NC}"
    echo ""
}

check_requirements() {
    if ! command -v docker &> /dev/null; then
        echo -e "${RED}[HATA] Docker yüklü değil!${NC}"
        echo "Yüklemek için: curl -fsSL https://get.docker.com | sh"
        exit 1
    fi

    if ! docker compose version &> /dev/null; then
        echo -e "${RED}[HATA] Docker Compose (v2) yüklü değil!${NC}"
        exit 1
    fi

    echo -e "${GREEN}[OK] Docker ve Docker Compose mevcut${NC}"
}

setup_env() {
    if [ ! -f .env ]; then
        echo -e "${YELLOW}[INFO] .env dosyası bulunamadı, .env.example kopyalanıyor...${NC}"
        cp .env.example .env
        echo -e "${YELLOW}[UYARI] .env dosyasını düzenleyip şifreleri değiştirmeniz önerilir!${NC}"
        echo -e "${YELLOW}        nano .env${NC}"
        echo ""
    fi
}

cmd_up() {
    print_banner
    check_requirements
    setup_env

    echo -e "${CYAN}[1/3] Docker imajları build ediliyor...${NC}"
    docker compose -p $PROJECT_NAME -f $COMPOSE_FILE build --parallel

    echo -e "${CYAN}[2/3] Servisler başlatılıyor...${NC}"
    docker compose -p $PROJECT_NAME -f $COMPOSE_FILE up -d

    echo -e "${CYAN}[3/3] Servis durumları kontrol ediliyor...${NC}"
    sleep 5
    docker compose -p $PROJECT_NAME -f $COMPOSE_FILE ps

    source .env 2>/dev/null || true

    echo ""
    echo -e "${GREEN}════════════════════════════════════════════════${NC}"
    echo -e "${GREEN}  Fitliyo başarıyla ayağa kalktı!${NC}"
    echo -e "${GREEN}${NC}"
    echo -e "${GREEN}  Web:        http://localhost:${APP_PORT:-8080}${NC}"
    echo -e "${GREEN}  Swagger:    http://localhost:${APP_PORT:-8080}/swagger${NC}"
    echo -e "${GREEN}  PostgreSQL: localhost:${POSTGRES_EXTERNAL_PORT:-5433}${NC}"
    echo -e "${GREEN}  Redis:      localhost:${REDIS_EXTERNAL_PORT:-6380}${NC}"
    echo -e "${GREEN}════════════════════════════════════════════════${NC}"
    echo ""
}

cmd_down() {
    echo -e "${YELLOW}Servisler durduruluyor...${NC}"
    docker compose -p $PROJECT_NAME -f $COMPOSE_FILE down
    echo -e "${GREEN}Tüm servisler durduruldu.${NC}"
}

cmd_restart() {
    cmd_down
    cmd_up
}

cmd_logs() {
    local service=${2:-""}
    if [ -n "$service" ]; then
        docker compose -p $PROJECT_NAME -f $COMPOSE_FILE logs -f "$service"
    else
        docker compose -p $PROJECT_NAME -f $COMPOSE_FILE logs -f
    fi
}

cmd_status() {
    docker compose -p $PROJECT_NAME -f $COMPOSE_FILE ps
}

cmd_rebuild() {
    echo -e "${CYAN}İmajlar yeniden build ediliyor (cache'siz)...${NC}"
    docker compose -p $PROJECT_NAME -f $COMPOSE_FILE build --no-cache --parallel
    docker compose -p $PROJECT_NAME -f $COMPOSE_FILE up -d
    echo -e "${GREEN}Rebuild tamamlandı.${NC}"
}

cmd_clean() {
    echo -e "${RED}[UYARI] Bu işlem tüm verileri (DB, cache) SİLECEK!${NC}"
    read -p "Devam etmek istiyor musunuz? (y/N): " confirm
    if [ "$confirm" = "y" ] || [ "$confirm" = "Y" ]; then
        docker compose -p $PROJECT_NAME -f $COMPOSE_FILE down -v
        echo -e "${GREEN}Tüm servisler ve volume'ler silindi.${NC}"
    else
        echo "İptal edildi."
    fi
}

# ─── Komut Yönlendirmesi ────────────────────────────────────
case "${1:-up}" in
    up)       cmd_up ;;
    down)     cmd_down ;;
    restart)  cmd_restart ;;
    logs)     cmd_logs "$@" ;;
    status)   cmd_status ;;
    rebuild)  cmd_rebuild ;;
    clean)    cmd_clean ;;
    *)
        echo "Kullanım: $0 {up|down|restart|logs|status|rebuild|clean}"
        echo ""
        echo "  up       — Tüm servisleri başlat (varsayılan)"
        echo "  down     — Tüm servisleri durdur"
        echo "  restart  — Durdur ve tekrar başlat"
        echo "  logs     — Log'ları izle (opsiyonel: logs web)"
        echo "  status   — Servis durumlarını göster"
        echo "  rebuild  — Cache'siz yeniden build et"
        echo "  clean    — Her şeyi sil (DB dahil!)"
        exit 1
        ;;
esac
