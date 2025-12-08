#!/bin/sh
set -e

# This script verifies that the test user password has been set correctly by attempting to authenticate
# against the Conjur API.

HTTP_STATUS=$(curl -s -k -w "%{http_code}" -o /dev/null \
  -u "TestUser@local:${CONJUR_PASSWORD}" \
  -X GET \
  "${CONJUR_URL}/authn/${CONJUR_ACCOUNT_ID}/login")

if [ "$HTTP_STATUS" -eq 200 ]; then
  echo "Test user password set successfully"
else
  echo "Failed to set test user password. HTTP status: ${HTTP_STATUS}"
  exit 1
fi