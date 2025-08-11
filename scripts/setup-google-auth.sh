#!/bin/bash

echo "Corporate CMS - Google Authentication Setup"
echo "=========================================="
echo ""
echo "This script will guide you through setting up Google Authentication."
echo ""

read -p "Do you want to configure Google Authentication now? (y/n): " answer

if [[ "$answer" != "y" && "$answer" != "Y" ]]; then
    echo "You can configure Google Authentication later."
    echo "See docs/google-auth-setup.md for instructions."
    exit 0
fi

echo ""
echo "Please follow these steps:"
echo "1. Go to https://console.developers.google.com/"
echo "2. Create a new project or select an existing one"
echo "3. Navigate to 'Credentials' section"
echo "4. Create OAuth 2.0 Client ID for web application"
echo ""

read -p "Enter your Google Client ID: " client_id
read -p "Enter your Google Client Secret: " client_secret

# Update appsettings.json
echo "Updating appsettings.json..."
sed -i "s/\"ClientId\": \"YOUR_GOOGLE_CLIENT_ID\"/\"ClientId\": \"$client_id\"/" appsettings.json
sed -i "s/\"ClientSecret\": \"YOUR_GOOGLE_CLIENT_SECRET\"/\"ClientSecret\": \"$client_secret\"/" appsettings.json

echo ""
echo "Configuration complete! Google Authentication has been set up."
echo "You can now use Google Sign-In on your login and registration pages."
echo ""
echo "Note: For production environments, consider using user secrets or environment variables instead."
echo "See docs/google-auth-setup.md for more information."
