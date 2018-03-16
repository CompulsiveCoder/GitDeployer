echo "Retrieving required libraries..."

if [ ! -f nuget.exe ]; then
    echo "nuget.exe not found. Downloading..."
    wget http://nuget.org/nuget.exe
fi

echo "Installing libraries..."

mono nuget.exe install gitter -version 1.0.0 -pre

echo "Installation complete!"
