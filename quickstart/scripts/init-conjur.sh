#!/bin/sh
set -e

CONJUR_ACCOUNT="${CONJUR_ACCOUNT}"
CONJUR_URL="${CONJUR_APPLIANCE_URL}"
USER_PASSWORD="${CONJUR_USER_PASSWORD}"
CONJUR_SECRET_VALUE="${CONJUR_SECRET_VALUE}"


# Install curl and jq for API calls (docker:cli is Alpine-based)
apk add --no-cache curl bash jq

echo "Starting Conjur initialization..."
echo "Conjur URL: ${CONJUR_URL}"
echo "Account: ${CONJUR_ACCOUNT}"

# Docker's healthcheck ensures Conjur is ready, but give it a moment
sleep 2

# Create account using conjurctl on the Conjur container
echo "Creating Conjur account: ${CONJUR_ACCOUNT}"
ACCOUNT_OUTPUT=$(docker exec conjur-oss conjurctl account create "${CONJUR_ACCOUNT}" 2>&1)

# Extract the admin API key from the output
ADMIN_API_KEY=$(echo "$ACCOUNT_OUTPUT" | grep "API key for admin:" | awk '{print $NF}')

if [ -z "$ADMIN_API_KEY" ]; then
  # Check if account already exists
  if echo "$ACCOUNT_OUTPUT" | grep -q "already exists"; then
    echo "Account ${CONJUR_ACCOUNT} already exists"
    echo "Note: Admin API key is only shown once during creation"
    echo "If you need the admin key, restart with: docker-compose down && docker-compose up -d"
    exit 1
  else
    echo "Failed to create account or retrieve API key"
    echo "Output: $ACCOUNT_OUTPUT"
    exit 1
  fi
else
  echo "Account created successfully!"
  echo "Admin API Key: ${ADMIN_API_KEY}"
fi

# Authenticate as admin and get token
echo "Authenticating as admin..."
ADMIN_TOKEN=$(curl -s -k \
  --data "${ADMIN_API_KEY}" \
  "${CONJUR_URL}/authn/${CONJUR_ACCOUNT}/admin/authenticate" | base64 | tr -d '\r\n')

echo "Loading policy..."
POLICY_RESPONSE=$(curl -s -k \
  -H "Authorization: Token token=\"${ADMIN_TOKEN}\"" \
  -X POST \
  --data-binary @/policy/Test.yml \
  "${CONJUR_URL}/policies/${CONJUR_ACCOUNT}/policy/root")

echo "Policy loaded successfully"

echo $POLICY_RESPONSE

# The policy response contains created resources including the host with its API key
TEST_USER_API_KEY=$(echo "${POLICY_RESPONSE}" | jq -r '.created_roles | to_entries[] | select(.key | contains("TestUser@local")) | .value.api_key')

if [ -z "$TEST_USER_API_KEY" ] || [ "$TEST_USER_API_KEY" = "null" ]; then
  echo "Failed to extract API key from policy response"
  echo "Policy response was: ${POLICY_RESPONSE}"
  exit 1
fi

echo "Test User API Key obtained: ${TEST_USER_API_KEY}"

# # Set password for test user
# echo "Setting password for test user..."
# curl -s -k \
#   -H "Authorization: Token token=\"${TEST_USER_API_KEY}\"" \
#   -X PUT \
#   --data "${USER_PASSWORD}" \
#   "${CONJUR_URL}/authn/${CONJUR_ACCOUNT}/password"

# echo "Test user password set"

# Set password for test user using Basic auth (username:api_key)
echo "Setting password for test user..."
HTTP_STATUS=$(curl -s -k -w "%{http_code}" -o /dev/null \
  -u "TestUser@local:${TEST_USER_API_KEY}" \
  -X PUT \
  --data "${USER_PASSWORD}" \
  "${CONJUR_URL}/authn/${CONJUR_ACCOUNT}/password")

if [ "$HTTP_STATUS" -eq 200 ] || [ "$HTTP_STATUS" -eq 204 ]; then
  echo "Test user password set successfully"
else
  echo "Failed to set test user password. HTTP status: ${HTTP_STATUS}"
  exit 1
fi

echo "=========================================="
echo "Setup Complete!"
echo "=========================================="
echo "Account: ${CONJUR_ACCOUNT}"
echo "Conjur URL: http://localhost:8080 (or your mapped port if different)"
echo ""
echo "Admin Credentials:"
echo "  Username: admin"
echo "  API Key:  ${ADMIN_API_KEY}"
echo ""

echo "Test User Credentials:"
echo "  Username: TestUser@local"
echo "  Password: ${USER_PASSWORD}"
echo "  API Key:  ${TEST_USER_API_KEY}"
echo ""
echo "Test Variable: local/MyTestVar"
echo "=========================================="

# Set a value for the test variable
echo "Setting value for test variable..."
curl -s -k \
  -H "Authorization: Token token=\"${ADMIN_TOKEN}\"" \
  -X POST \
  --data "${CONJUR_SECRET_VALUE}" \
  "${CONJUR_URL}/secrets/${CONJUR_ACCOUNT}/variable/local%2FMyTestVar"

# Verify the setup by trying to authenticate as the test user and read the variable
echo ""
echo "Verifying setup..."
echo "Authenticating as TestUser@local..."

# Get auth token for test user
AUTH_RESPONSE=$(curl -s -k \
  --data "${TEST_USER_API_KEY}" \
  "${CONJUR_URL}/authn/${CONJUR_ACCOUNT}/TestUser%40local/authenticate")

if [ -z "$AUTH_RESPONSE" ]; then
  echo "Failed to authenticate as test user"
  exit 1
fi

echo "Successfully authenticated as TestUser@local"

# Try to retrieve the variable value
echo "Retrieving variable value as TestUser@local..."
VARIABLE_VALUE=$(curl -s -k \
  -H "Authorization: Token token=\"$(echo -n $AUTH_RESPONSE | base64 | tr -d '\r\n')\"" \
  "${CONJUR_URL}/secrets/${CONJUR_ACCOUNT}/variable/local%2FMyTestVar")

if [ "$VARIABLE_VALUE" = "${CONJUR_SECRET_VALUE}" ]; then
  echo "✓ Successfully retrieved variable value: ${VARIABLE_VALUE}"
  echo "✓ Setup verification complete!"
else
  echo "✗ Failed to retrieve correct variable value"
  echo "Expected: ${CONJUR_SECRET_VALUE}"
  echo "Got: ${VARIABLE_VALUE}"
  exit 1
fi

echo ""
echo "=========================================="
echo "Integration test environment is ready!"
echo "=========================================="

echo ""
echo "==========================================="
echo "ConjurUrl: http://localhost:8080 (or your mapped port if different)"
echo "AccountId: ${CONJUR_ACCOUNT}"

echo ""
echo "Admin Login: admin"
echo "Admin API Key: ${ADMIN_API_KEY}"

echo ""
echo "Test User Login: TestUser@local"
echo "Test User Password: ${USER_PASSWORD}"
echo "Test User API Key: ${TEST_USER_API_KEY}"

echo ""
echo "Test Variable: local/MyTestVar"
echo "Test Variable Value: ${CONJUR_SECRET_VALUE}"
echo "==========================================="

# # Keep container running for logs
# tail -f /dev/null