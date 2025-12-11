## Overview

The CyberArk Conjur PAM retrieves secrets (variables) from CyberArkC onjur instances using either API keys or passwords for a given login identity.

## Requirements

### CyberArk Conjur
- **Tested with**: CyberArk Conjur OSS v1.24 (latest as of writing)
- **Minimum version**: CyberArk Conjur OSS v1.19.0
- **Supported products**: Conjur Open Source, Conjur Enterprise

### Keyfactor Platform
- **Minimum .NET Runtime**: .NET 8
- **Keyfactor Command**: Version 12.0 or later
- **Universal Orchestrator**: Version 12.3 or later

# Authentication and Authorization

This integration currently only supports API Token / password authentication with Conjur.

**Note**: Only Conjur user identities (created with `!user` in policies) support password authentication. Host identities (created with `!host`) **only** support API key authentication.

### Policy Requirements

In order to access CyberArk Conjur secrets via the API, the accessing identity ***must have*** at least the `execute` privileges on the secret resource.

Here is an example policy document that grants user `TestUser@local` the privileges to `execute` on the `local/MyTestVar` variable (secret). Note that `read` privileges are optional but not required:

```yaml
- !policy
  id: local
  body:
    # Define a human user, a non-human identity that represents an application, and a secret
    - !user TestUser
    # - !host myDemoApp
    - !variable MyTestVar

    # Give permission to the human user to execute on the secret
    - !permit
      role: !user TestUser
      privileges: [ execute ]
      resource: !variable MyTestVar
```
