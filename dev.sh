#!/bin/bash

# Food Order - Tüm servisleri ayrı Terminal pencerelerinde başlat

BASEDIR="$(cd "$(dirname "$0")" && pwd)"

# Backend (.NET)
osascript -e "
tell application \"Terminal\"
    do script \"cd '$BASEDIR/backend/WebAPI' && dotnet run --launch-profile http\"
    set custom title of front window to \"Backend\"
end tell
"

# Seller Panel (Next.js - port 3000)
osascript -e "
tell application \"Terminal\"
    do script \"cd '$BASEDIR/front-seller' && npm run dev -- -p 3000\"
    set custom title of front window to \"Seller\"
end tell
"

# Admin Panel (Next.js - port 3001)
osascript -e "
tell application \"Terminal\"
    do script \"cd '$BASEDIR/front-admin' && npm run dev -- -p 3001\"
    set custom title of front window to \"Admin\"
end tell
"

# Müşteri App (React Native - port 8081)
osascript -e "
tell application \"Terminal\"
    do script \"cd '$BASEDIR/front-app' && npx react-native start --port 8081\"
    set custom title of front window to \"App Metro (8081)\"
end tell
"
sleep 3
osascript -e "
tell application \"Terminal\"
    do script \"cd '$BASEDIR/front-app' && npx react-native run-ios --port 8081 --simulator 'iPhone 16 Pro'\"
    set custom title of front window to \"App Build\"
end tell
"

# Kurye App (React Native - port 8082)
osascript -e "
tell application \"Terminal\"
    do script \"cd '$BASEDIR/front-courier' && npx react-native start --port 8082\"
    set custom title of front window to \"Courier Metro (8082)\"
end tell
"
sleep 3
osascript -e "
tell application \"Terminal\"
    do script \"cd '$BASEDIR/front-courier' && npx react-native run-ios --port 8082 --simulator 'iPhone 15 Pro'\"
    set custom title of front window to \"Courier Build\"
end tell
"

echo "✅ 5 terminal penceresi açıldı:"
echo "   • Backend    → http://localhost:3762"
echo "   • Seller     → http://localhost:3000"
echo "   • Admin      → http://localhost:3001"
echo "   • App Metro  → port 8081 (iPhone 16 Pro)"
echo "   • Courier Metro → port 8082 (iPhone 15 Pro)"
