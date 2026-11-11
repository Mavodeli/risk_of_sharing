cd build
DEL "risk_of_sharing.zip"
cd ..
7z a -y -tzip "./build/risk_of_sharing.zip" "manifest.json"
7z a -y -tzip "./build/risk_of_sharing.zip" "icon.png"
7z a -y -tzip "./build/risk_of_sharing.zip" "README.md"
7z a -y -tzip "./build/risk_of_sharing.zip" "CHANGELOG.md"
7z a -y -tzip "./build/risk_of_sharing.zip" "./risk_of_sharing/bin/Debug/netstandard2.1/de.mavodeli.risk_of_sharing.dll" -mx9
echo done
exit