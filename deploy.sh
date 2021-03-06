echo "Deploying to:"
echo "  /app/"

mkdir -p app

cp -f bin/Release/*.exe app/
cp -f bin/Release/*.dll app/
# cp -f bin/Release/*.mdb app/

if [ ! -e "app/BridgeArduinoSerialToMqttSplitCsv.exe.config" ]; then
    cp bin/Release/BridgeArduinoSerialToMqttSplitCsv.exe.config app/
else
    echo "  Config file already exists. Skipping overwrite to preserve changes."
fi

echo "Deploy complete"
