# How to Use the Google Login Endpoint

## 🚀 Quick Start - Two Options

### Option 1: Easy Testing (Recommended for Development) ⭐

Use the **DEV endpoint** that doesn't require a real Google token:

**Endpoint**: `POST /api/Auth/dev-google-login`

**Request Body**:
```json
{
  "email": "test@example.com"
}
```

**Example with cURL**:
```bash
curl -X 'POST' \
  'http://localhost:5281/api/Auth/dev-google-login' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "email": "your-email@example.com"
}'
```

**Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "[DEV MODE] Login successful",
  "userInfo": {
    "id": "42dcdadf-40da-4655-b231-ce2fdaba1d34",
    "firstName": "your-email",
    "lastName": "TestUser",
    "email": "your-email@example.com",
    "role": "User"
  }
}
```

**What it does**:
- ✅ Creates a user if they don't exist (with default address)
- ✅ Returns a JWT token immediately
- ✅ No Google OAuth setup needed
- ✅ Only available in DEBUG mode (safe for development)

---

### Option 2: Real Google OAuth (Production-Ready)

Use the **production endpoint** with a real Google ID token:

**Endpoint**: `POST /api/Auth/google-login`

**Request Body**:
```json
{
  "idToken": "YOUR_GOOGLE_ID_TOKEN_HERE"
}
```

---

## 📋 Step-by-Step: Getting a Real Google ID Token

### Prerequisites
1. You need a **Google Cloud Project** with OAuth 2.0 credentials
2. Go to [Google Cloud Console](https://console.cloud.google.com/)
3. Create OAuth 2.0 Client ID (if you don't have one)

### Method 1: Using Google OAuth Playground (Easiest)

1. **Go to**: https://developers.google.com/oauthplayground

2. **Click the gear icon** (⚙️) in the top-right corner

3. **Check**: "Use your own OAuth credentials"

4. **Enter your credentials**:
   - OAuth Client ID: (from Google Cloud Console)
   - OAuth Client Secret: (from Google Cloud Console)

5. **Select scopes** (Step 1):
   - Find "Google OAuth2 API v2"
   - Select: `https://www.googleapis.com/auth/userinfo.email`
   - Select: `https://www.googleapis.com/auth/userinfo.profile`

6. **Click**: "Authorize APIs"
   - You'll be redirected to sign in with Google
   - Grant the requested permissions

7. **Click**: "Exchange authorization code for tokens" (Step 2)

8. **Copy the `id_token`** (not the `access_token`!)
   - It's a long string starting with `eyJ...`

9. **Use it in Swagger**:
   ```json
   {
     "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6..."
   }
   ```

10. **Important**: ID tokens expire in ~1 hour, so get a fresh one if it fails

---

### Method 2: Using Postman

1. **Create a new request** in Postman
2. **Go to Authorization tab**
3. **Select**: OAuth 2.0
4. **Configure**:
   - Grant Type: Authorization Code
   - Auth URL: `https://accounts.google.com/o/oauth2/v2/auth`
   - Access Token URL: `https://oauth2.googleapis.com/token`
   - Client ID: (your Google Client ID)
   - Client Secret: (your Google Client Secret)
   - Scope: `openid email profile`
5. **Click**: "Get New Access Token"
6. **Copy the `id_token`** from the response

---

### Method 3: From a Frontend Application

If you have a React/Angular/Vue app:

```javascript
// Example with Google Sign-In JavaScript library
<script src="https://accounts.google.com/gsi/client" async defer></script>

<div id="g_id_onload"
     data-client_id="YOUR_GOOGLE_CLIENT_ID"
     data-callback="handleCredentialResponse">
</div>

<script>
function handleCredentialResponse(response) {
  const idToken = response.credential; // This is what you send to the backend
  
  // Send to your API
  fetch('http://localhost:5281/api/Auth/google-login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ idToken: idToken })
  })
  .then(res => res.json())
  .then(data => {
    console.log('JWT Token:', data.token);
    // Store the token and use it for authenticated requests
    localStorage.setItem('token', data.token);
  });
}
</script>
```

---

## 🧪 Testing in Swagger

### 1. Open Swagger UI
Navigate to: `http://localhost:5281/swagger`

### 2. Find the endpoint
Look for: `POST /api/Auth/google-login`

### 3. Click "Try it out"

### 4. Enter the request body
For **DEV mode** (using `dev-google-login`):
```json
{
  "email": "test@gmail.com"
}
```

For **PRODUCTION** (using `google-login`):
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjdhYmM..."
}
```

### 5. Click "Execute"

### 6. Check the response
**Success (200)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Google login successful.",
  "userInfo": {
    "userId": "guid-here",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@gmail.com",
    "role": "User"
  }
}
```

**Error (401 - Unauthorized)**:
```json
{
  "message": "Invalid Google token."
}
```

---

## 🔐 Using the JWT Token

After successful login, use the returned token for authenticated requests:

### Example: Get Current User Info

**cURL**:
```bash
curl -X 'GET' \
  'http://localhost:5281/api/Auth/me' \
  -H 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'
```

**In Swagger**:
1. Click the 🔒 "Authorize" button at the top
2. Enter: `Bearer YOUR_TOKEN_HERE`
3. Click "Authorize"
4. Now all authenticated endpoints will work

---

## 🎯 What Happens Behind the Scenes

### For DEV endpoint (`dev-google-login`):
1. ✅ Checks if user exists by email
2. ✅ If not, creates new user with:
   - Password: `null` (OAuth user)
   - AuthProvider: `"Google"`
   - Default address created
   - Default role: "User"
3. ✅ Generates JWT token
4. ✅ Returns token + user info

### For PRODUCTION endpoint (`google-login`):
1. ✅ Validates Google ID token with Google's servers
2. ✅ Extracts user info (email, name) from token
3. ✅ Checks if user exists by email
4. ✅ If not, creates new user (same as DEV)
5. ✅ Generates JWT token
6. ✅ Returns token + user info

---

## 📊 Database Changes

The user is stored with:
```sql
INSERT INTO user_service."Users" (
  Id, Email, FirstName, LastName, 
  Password,          -- NULL for OAuth users ✅
  AuthProvider,      -- "Google"
  ExternalUserId,    -- Google's user ID
  RoleId
)
```

---

## ⚠️ Common Issues & Solutions

### Issue 1: "Invalid Google token"
**Solution**: 
- Get a fresh ID token (they expire in ~1 hour)
- Make sure you're using `id_token`, not `access_token`

### Issue 2: "Default user role not found"
**Solution**: 
- Seed your database with default roles
- Run migrations: `dotnet ef database update`

### Issue 3: "An error occurred while saving"
**Solution**: 
- ✅ Already fixed! The migration made Password nullable
- If still seeing this, ensure migration was applied:
  ```bash
  dotnet ef database update
  ```

### Issue 4: DEV endpoint not showing in Swagger
**Solution**: 
- Only available in DEBUG mode
- Check that you're running in Development environment

---

## 🔄 Complete Flow Example

### 1. Development Testing (No Frontend)
```bash
# Step 1: Test with dev endpoint
curl -X 'POST' 'http://localhost:5281/api/Auth/dev-google-login' \
  -H 'Content-Type: application/json' \
  -d '{"email": "test@example.com"}'

# Step 2: Copy the token from response
TOKEN="eyJhbGciOiJIUzI1NiIs..."

# Step 3: Use token to access protected endpoint
curl -X 'GET' 'http://localhost:5281/api/Auth/me' \
  -H "Authorization: Bearer $TOKEN"
```

### 2. Production Flow (With Frontend)
```
User clicks "Sign in with Google"
  ↓
Frontend gets Google ID token
  ↓
Frontend sends token to: POST /api/Auth/google-login
  ↓
Backend validates token with Google
  ↓
Backend creates/finds user in database
  ↓
Backend returns JWT token
  ↓
Frontend stores JWT token
  ↓
Frontend uses JWT for all API requests
```

---

## 📝 Summary

**For Testing (No Frontend)**:
- Use: `POST /api/Auth/dev-google-login`
- Body: `{ "email": "test@example.com" }`
- ✅ Works immediately, no setup needed

**For Production (With Frontend)**:
- Use: `POST /api/Auth/google-login`
- Body: `{ "idToken": "..." }`
- Need: Google OAuth setup + ID token

**Both endpoints**:
- ✅ Create user if doesn't exist
- ✅ Handle OAuth (null password) correctly
- ✅ Return JWT token for authenticated requests
- ✅ Work with Swagger for testing

---

## 🎉 You're Ready!

Try it now in Swagger:
1. Navigate to `http://localhost:5281/swagger`
2. Find `POST /api/Auth/dev-google-login`
3. Click "Try it out"
4. Enter: `{ "email": "your-email@gmail.com" }`
5. Click "Execute"
6. Copy the token and use it! 🚀

