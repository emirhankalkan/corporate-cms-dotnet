# Google Authentication Configuration

## Google OAuth 2.0 Client ID Setup

To enable Google authentication in this CMS, follow these steps:

1. **Create a Google API project**:
   - Go to [Google Developer Console](https://console.developers.google.com/)
   - Create a new project or select an existing one
   - Navigate to "Credentials" section

2. **Configure OAuth consent screen**:
   - Set application name, authorized domains, etc.
   - Save the settings

3. **Create OAuth 2.0 Client ID**:
   - Click "Create Credentials" > "OAuth client ID"
   - Select "Web application" as application type
   - Add your domain to "Authorized JavaScript origins"
   - Add your callback URL to "Authorized redirect URIs" (e.g., https://yourdomain.com/signin-google)
   - Click "Create"

4. **Get your Client ID and Secret**:
   - After creation, Google will display your Client ID and Client Secret
   - Copy these values

5. **Update application settings**:
   - Open `appsettings.json` (and `appsettings.Production.json` for production)
   - Update the "Authentication:Google" section:
   ```json
   "Authentication": {
     "Google": {
       "ClientId": "YOUR_GOOGLE_CLIENT_ID",
       "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
     }
   }
   ```

6. **Restart the application**:
   - The Google sign-in button should now work properly

## Security Considerations

- Never commit your Client Secret to source control
- For production, use environment variables or secret manager
- Ensure you validate email addresses from Google authentication
- Consider implementing multi-factor authentication for additional security

## Testing

To test the Google authentication integration:
1. Start the application
2. Go to the Login or Register page
3. Click the "Google ile Giriş Yap" button
4. You should be redirected to Google's authentication page
5. After authentication, you'll be redirected back to the application
