<h1 align="center" style="border-bottom: none">
    CyberArk Conjur PAM Provider
</h1>

<p align="center">
  <!-- Badges -->
<img src="https://img.shields.io/badge/integration_status-production-3D1973?style=flat-square" alt="Integration Status: production" />
<a href="https://github.com/Keyfactor/cyberark-conjur-pam/releases"><img src="https://img.shields.io/github/v/release/Keyfactor/cyberark-conjur-pam?style=flat-square" alt="Release" /></a>
<img src="https://img.shields.io/github/issues/Keyfactor/cyberark-conjur-pam?style=flat-square" alt="Issues" />
<img src="https://img.shields.io/github/downloads/Keyfactor/cyberark-conjur-pam/total?style=flat-square&label=downloads&color=28B905" alt="GitHub Downloads (all assets, all releases)" />
</p>

<p align="center">
  <!-- TOC -->
  <a href="#support">
    <b>Support</b>
  </a> 
  ·
  <a href="#getting-started">
    <b>Installation</b>
  </a>
  ·
  <a href="#license">
    <b>License</b>
  </a>
  ·
  <a href="https://github.com/orgs/Keyfactor/repositories?q=pam">
    <b>Related Integrations</b>
  </a>
</p>

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

## Support
The CyberArk Conjur PAM Provider is supported by Keyfactor for Keyfactor customers. If you have a support issue, please open a support ticket with your Keyfactor representative. If you have a support issue, please open a support ticket via the Keyfactor Support Portal at https://support.keyfactor.com. 

> To report a problem or suggest a new feature, use the **[Issues](../../issues)** tab. If you want to contribute actual bug fixes or proposed enhancements, use the **[Pull requests](../../pulls)** tab.

## Getting Started

The CyberArk Conjur PAM Provider is used by Command to resolve PAM-eligible credentials for Universal Orchestrator extensions and for accessing Certificate Authorities. When configured, Command will use the CyberArk Conjur PAM Provider to retrieve credentials needed to communicate with the target system. There are two ways to install the CyberArk Conjur PAM Provider, and you may elect to use one or both methods:

1. **Locally on the Keyfactor Command server**: PAM credential resolution via the CyberArk Conjur PAM Provider will occur on the Keyfactor Command server each time an elegible credential is needed.
2. **Remotely On Universal Orchestrators**: When Jobs are dispatched to Universal Orchestrators, the associated Certificate Store extension assembly will use the CyberArk Conjur PAM Provider to resolve eligible PAM credentials.

Before proceeding with installation, you should consider which pattern is best for your requirements and use case.

### Installation

> [!IMPORTANT]
> For the most up-to-date and complete documentation on how to install a PAM provider extension, please visit our [product documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/ReferenceGuide/Preparing%20Third%20Party%20PAM%20Providers%20to%20Work%20with.htm?Highlight=pam%20provider#InstallingCustomPAMProviderExtensions)


To install CyberArk Conjur PAM Provider, it is recommended you install [kfutil](https://github.com/Keyfactor/kfutil). `kfutil` is a command-line tool that simplifies the process of creating PAM Types in Keyfactor Command.







#### Requirements
   TODO Requirements is a required section

#### Create PAM type in Keyfactor Command


##### Using `kfutil`
Create the required PAM Types in the connected Command platform.

```shell
# CyberArk Conjur
kfutil pam types-create -r cyberark-conjur-pam -n CyberArk Conjur
```

##### Using the API
For full API docs please visit our [product documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/WebAPI/KeyfactorAPI/PAMProvidersPOSTTypes.htm?Highlight=pam%20type)

Below is the payload to `POST` to the Keyfactor Command API
```json
{
    "Name": "CyberArk Conjur",
    "Parameters": [
        {
            "Name": "ConjurUrl",
            "DisplayName": "Conjur URL",
            "DataType": 1,
            "InstanceLevel": false,
            "Description": "The URL of the Conjur server (e.g., https://conjur.example.com)"
        },
        {
            "Name": "AccountId",
            "DisplayName": "Account ID",
            "DataType": 1,
            "InstanceLevel": false,
            "Description": "The Conjur account identifier"
        },
        {
            "Name": "Login",
            "DisplayName": "Login",
            "DataType": 1,
            "InstanceLevel": false,
            "Description": "The login that will access the Conjur account and variable within the account. The login must have a policy with \u201cread\u201d and \u201cexecute\u201d permissions on the target variable"
        },
        {
            "Name": "ApiKey",
            "DisplayName": "API Key / Password",
            "DataType": 2,
            "InstanceLevel": false,
            "Description": "The API key or password for the login"
        },
        {
            "Name": "VariablePath",
            "DisplayName": "Variable (Secret) Path",
            "DataType": 1,
            "InstanceLevel": true,
            "Description": "The full path to the variable (secret) in Conjur (e.g., 'my-app/db-password' or 'production/api-keys/service-token')"
        }
    ]
}
```

#### Install PAM provider on Keyfactor Command Host (Local)


("TODO Platform Install is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)


#### Install PAM provider on a Universal Orchestrator Host (Remote)


("TODO Orchestrator Install is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)







### Usage






#### From Keyfactor Command Host (Local)


("TODO Platform Usage is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)


#### From a Universal Orchestrator Host (Remote)


("TODO Orchestrator Usage is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info",)




> [!NOTE]
> Additional information on CyberArk Conjur can be found in the [supplemental documentation](docs/cyberark conjur.md).



## License

Apache License 2.0, see [LICENSE](LICENSE)

## Related Integrations

See all [Keyfactor PAM Provider extensions](https://github.com/orgs/Keyfactor/repositories?q=pam).