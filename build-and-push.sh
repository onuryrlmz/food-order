#!/bin/bash
set -e

REGISTRY="ghcr.io/onuryrlmz"
PLATFORM="linux/amd64"
cd "$(dirname "$0")"

# Renkler
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

log() { echo -e "${GREEN}[BUILD]${NC} $1"; }
warn() { echo -e "${YELLOW}[WARN]${NC} $1"; }
err() { echo -e "${RED}[ERROR]${NC} $1"; exit 1; }

# Hangi servisleri build edeceğini belirle
SERVICES="${@:-all}"

build_backend() {
    log "Backend image build ediliyor..."
    docker build --platform $PLATFORM \
        -t $REGISTRY/food-order-api:latest \
        -f backend/WebAPI/Dockerfile \
        ./backend || err "Backend build başarısız!"
    log "Backend push ediliyor..."
    docker push $REGISTRY/food-order-api:latest || err "Backend push başarısız!"
    log "Backend tamamlandı!"
}

build_admin() {
    log "Admin panel image build ediliyor..."
    docker build --platform $PLATFORM \
        --build-arg NEXT_PUBLIC_API_BASE_URL=https://food-order-api.yrlmzteknoloji.com \
        -t $REGISTRY/food-order-admin:latest \
        ./front-admin || err "Admin build başarısız!"
    log "Admin push ediliyor..."
    docker push $REGISTRY/food-order-admin:latest || err "Admin push başarısız!"
    log "Admin tamamlandı!"
}

build_seller() {
    log "Seller panel image build ediliyor..."
    docker build --platform $PLATFORM \
        --build-arg NEXT_PUBLIC_API_BASE_URL=https://food-order-api.yrlmzteknoloji.com \
        -t $REGISTRY/food-order-seller:latest \
        ./front-seller || err "Seller build başarısız!"
    log "Seller push ediliyor..."
    docker push $REGISTRY/food-order-seller:latest || err "Seller push başarısız!"
    log "Seller tamamlandı!"
}

# GHCR login kontrol
docker pull $REGISTRY/food-order-api:latest > /dev/null 2>&1 || {
    warn "GHCR login gerekiyor..."
    docker login ghcr.io -u onuryrlmz || err "Login başarısız!"
}

log "Build başlatılıyor... [$SERVICES]"
echo ""

case "$SERVICES" in
    all)
        build_backend
        echo ""
        build_admin
        echo ""
        build_seller
        ;;
    backend)
        build_backend
        ;;
    admin)
        build_admin
        ;;
    seller)
        build_seller
        ;;
    *)
        echo "Kullanım: ./build-and-push.sh [all|backend|admin|seller]"
        echo ""
        echo "Örnekler:"
        echo "  ./build-and-push.sh           # Hepsini build et"
        echo "  ./build-and-push.sh backend   # Sadece backend"
        echo "  ./build-and-push.sh admin     # Sadece admin panel"
        echo "  ./build-and-push.sh seller    # Sadece seller panel"
        exit 1
        ;;
esac

echo ""
log "Tüm işlemler tamamlandı!"
