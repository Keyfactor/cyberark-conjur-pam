# CyberArk Conjur PAM Integration

## Overview
The CyberArk Conjur PAM integration for Keyfactor enables secure retrieval of secrets (variables) from CyberArk Conjur instances. This integration allows Keyfactor Command and Universal Orchestrator to authenticate to Conjur and retrieve credentials on-demand for certificate management operations.

This PAM integration can run on either Keyfactor Command or Universal Orchestrator.

## Compatibility

### CyberArk Conjur
- **Tested with**: CyberArk Conjur OSS v1.24 (latest as of writing)
- **Minimum version**: CyberArk Conjur OSS v1.19.0
- **Supported products**: Conjur Open Source, Conjur Enterprise

### Keyfactor Platform
- **Minimum .NET Runtime**: .NET 8
- **Keyfactor Command**: Version 12.0 or later
- **Universal Orchestrator**: Version 12.3 or later

## Authentication

This integration currently only supports API Token / password authentication with Conjur.

**Note**: Only Conjur user identities (created with `!user` in policies) support password authentication. Host identities (created with `!host`) only support API key authentication.

## Additional Support and Resources

### CyberArk Conjur Documentation
- [Conjur OSS Documentation](https://docs.cyberark.com/conjur-open-source/latest/en/)
- [Policy Reference](https://docs.cyberark.com/conjur-open-source/latest/en/content/operations/policy/policy-overview.htm)
- [REST API Documentation](https://docs.cyberark.com/conjur-open-source/latest/en/content/developer/lp_rest_api.htm)
- [Secrets Manager CLI Documentation](https://docs.cyberark.com/conjur-open-source/latest/en/content/developer/cli/cli-lp.htm)

## Development

Please see our [CONTRIBUTING guide](./CONTRIBUTING.md) for documentation on how to contribute to this project.
