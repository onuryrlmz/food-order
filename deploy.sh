#!/bin/bash
set -e
cd "$(dirname "$0")"

# .env'den sunucu bilgilerini oku
source .env

HOST=$SERVER_HOST
USER=$SERVER_USER
PASS=$SERVER_PASS
REMOTE=$REMOTE_DIR
REGISTRY="ghcr.io/onuryrlmz"

# Renkler
G='\033[0;32m' R='\033[0;31m' Y='\033[1;33m' C='\033[0;36m' N='\033[0m'
log() { echo -e "${G}[DEPLOY]${N} $1"; }
warn() { echo -e "${Y}[WARN]${N} $1"; }
err() { echo -e "${R}[ERROR]${N} $1"; exit 1; }
step() { echo -e "\n${C}━━━ $1 ━━━${N}"; }

# sshpass kontrolü
if ! command -v sshpass &> /dev/null; then
    warn "sshpass yüklü değil, yükleniyor..."
    brew install hudochenkov/sshpass/sshpass 2>/dev/null || err "sshpass yüklenemedi."
fi

run_ssh() {
    sshpass -p "$PASS" ssh -o StrictHostKeyChecking=no "${USER}@${HOST}" "$1"
}

run_scp() {
    sshpass -p "$PASS" scp -o StrictHostKeyChecking=no "$1" "${USER}@${HOST}:${2}"
}

# GHCR'daki eski untagged image'ları sil
cleanup_ghcr() {
    local IMAGE_NAME="$1"
    step "GHCR temizliği — $IMAGE_NAME eski image'lar siliniyor"

    # Untagged (dangling) versiyonları al ve sil
    local VERSIONS=$(gh api --paginate "user/packages/container/${IMAGE_NAME}/versions" \
        --jq '.[] | select(.metadata.container.tags | length == 0) | .id' 2>/dev/null)

    if [ -z "$VERSIONS" ]; then
        log "Silinecek eski image yok."
        return
    fi

    local COUNT=0
    for VERSION_ID in $VERSIONS; do
        gh api --method DELETE "user/packages/container/${IMAGE_NAME}/versions/${VERSION_ID}" 2>/dev/null && ((COUNT++)) || true
    done
    log "$COUNT eski image silindi."
}

# Monorepo service/ dizinini web projesine kopyala (Docker build için)
prep_service() {
    local PROJECT_DIR="$1"
    cp -r service/ "${PROJECT_DIR}/service/"
    # ../../service → ./service (Docker container içinde)
    sed -i.bak "s|../../service/|../service/|g" "${PROJECT_DIR}/lib/service.js"
    rm -f "${PROJECT_DIR}/lib/service.js.bak"
}

cleanup_service() {
    local PROJECT_DIR="$1"
    rm -rf "${PROJECT_DIR}/service/"
    git checkout -- "${PROJECT_DIR}/lib/service.js" 2>/dev/null
}

# Hangi servisleri deploy edeceğini belirle
TARGET="${1:-all}"

deploy_backend() {
    step "1/5 — Backend image build ediliyor (linux/amd64, no-cache)"
    docker build --platform linux/amd64 --no-cache \
        -t $REGISTRY/food-order-api:latest \
        -f backend/WebAPI/Dockerfile \
        ./backend || err "Backend build başarısız!"

    step "2/5 — GHCR'a push ediliyor"
    docker push $REGISTRY/food-order-api:latest || err "Push başarısız!"

    step "3/5 — Sunucuya dosyalar gönderiliyor"
    run_ssh "mkdir -p $REMOTE"
    run_scp ".env" "${REMOTE}/.env"
    run_scp "dcc-backend.yml" "${REMOTE}/dcc-backend.yml"

    step "4/5 — Sunucuda backend ayağa kalkıyor"
    run_ssh "cd $REMOTE && docker compose -f dcc-backend.yml pull && docker compose -f dcc-backend.yml up -d --force-recreate"

    cleanup_ghcr "food-order-api"

    log "Backend deploy tamamlandı!"
}

deploy_infra() {
    step "1/2 — Sunucuya infra dosyaları gönderiliyor"
    run_ssh "mkdir -p $REMOTE"
    run_scp ".env" "${REMOTE}/.env"
    run_scp "dcc-infra.yml" "${REMOTE}/dcc-infra.yml"

    step "2/2 — Sunucuda MySQL + Redis ayağa kalkıyor"
    run_ssh "docker network create proxy-net 2>/dev/null || true"
    run_ssh "cd $REMOTE && docker compose -f dcc-infra.yml up -d"

    log "Infra deploy tamamlandı! MySQL + Redis çalışıyor."
    log "MySQL'in hazır olması için 15-20 saniye bekleyin."
}

deploy_admin() {
    prep_service "front-admin"
    step "1/5 — Admin panel image build ediliyor (linux/amd64, no-cache)"
    docker build --platform linux/amd64 --no-cache \
        --build-arg NEXT_PUBLIC_API_BASE_URL=https://food-order-api.yrlmzteknoloji.com \
        -t $REGISTRY/food-order-admin:latest \
        ./front-admin || { cleanup_service "front-admin"; err "Admin build başarısız!"; }
    cleanup_service "front-admin"

    step "2/5 — GHCR'a push ediliyor"
    docker push $REGISTRY/food-order-admin:latest || err "Push başarısız!"

    step "3/5 — Sunucuya dosyalar gönderiliyor"
    run_ssh "mkdir -p $REMOTE"
    run_scp "dcc-admin.yml" "${REMOTE}/dcc-admin.yml"

    step "4/5 — Sunucuda admin panel ayağa kalkıyor"
    run_ssh "cd $REMOTE && docker compose -f dcc-admin.yml pull && docker compose -f dcc-admin.yml up -d --force-recreate"

    cleanup_ghcr "food-order-admin"

    log "Admin panel deploy tamamlandı!"
}

deploy_seller() {
    prep_service "front-seller"
    step "1/5 — Seller panel image build ediliyor (linux/amd64, no-cache)"
    docker build --platform linux/amd64 --no-cache \
        --build-arg NEXT_PUBLIC_API_BASE_URL=https://food-order-api.yrlmzteknoloji.com \
        -t $REGISTRY/food-order-seller:latest \
        ./front-seller || { cleanup_service "front-seller"; err "Seller build başarısız!"; }
    cleanup_service "front-seller"

    step "2/5 — GHCR'a push ediliyor"
    docker push $REGISTRY/food-order-seller:latest || err "Push başarısız!"

    step "3/5 — Sunucuya dosyalar gönderiliyor"
    run_ssh "mkdir -p $REMOTE"
    run_scp "dcc-seller.yml" "${REMOTE}/dcc-seller.yml"

    step "4/5 — Sunucuda seller panel ayağa kalkıyor"
    run_ssh "cd $REMOTE && docker compose -f dcc-seller.yml pull && docker compose -f dcc-seller.yml up -d --force-recreate"

    cleanup_ghcr "food-order-seller"

    log "Seller panel deploy tamamlandı!"
}

echo -e "${C}"
echo "╔══════════════════════════════════════╗"
echo "║     Food Order Deploy Script         ║"
echo "║     Target: $TARGET"
echo "╚══════════════════════════════════════╝"
echo -e "${N}"

case "$TARGET" in
    all)
        deploy_infra
        echo ""
        log "MySQL hazır olması için 20 saniye bekleniyor..."
        sleep 20
        deploy_backend
        echo ""
        deploy_admin
        echo ""
        deploy_seller
        ;;
    infra)
        deploy_infra
        ;;
    backend)
        deploy_backend
        ;;
    admin)
        deploy_admin
        ;;
    seller)
        deploy_seller
        ;;
    *)
        echo "Kullanım: ./deploy.sh [all|infra|backend|admin|seller]"
        echo ""
        echo "  all      — Hepsini deploy et (infra → backend → admin → seller)"
        echo "  infra    — Sadece MySQL + Redis"
        echo "  backend  — Build + push + deploy API"
        echo "  admin    — Build + push + deploy Admin Panel"
        echo "  seller   — Build + push + deploy Seller Panel"
        exit 1
        ;;
esac

echo ""
echo -e "${G}╔══════════════════════════════════════╗${N}"
echo -e "${G}║     Deploy tamamlandı!               ║${N}"
echo -e "${G}╚══════════════════════════════════════╝${N}"
