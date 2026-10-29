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
  <a href="#overview">
    <b>Overview</b>
  </a>
  ·
  <a href="#installation">
    <b>Installation</b>
  </a>
  ·
  <a href="#usage">
    <b>Usage</b>
  </a>
  ·
  <a href="#support">
    <b>Support</b>
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

## Overview
The CyberArk Conjur PAM integration for Keyfactor enables secure retrieval of secrets (variables) from CyberArk Conjur instances. This integration allows Keyfactor Command and Universal Orchestrator to authenticate to Conjur and retrieve credentials on-demand for certificate management operations.

This PAM integration can run on either Keyfactor Command or Universal Orchestrator.

## Additional Support and Resources

### CyberArk Conjur Documentation
- [Conjur OSS Documentation](https://docs.cyberark.com/conjur-open-source/latest/en/)
- [Policy Reference](https://docs.cyberark.com/conjur-open-source/latest/en/content/operations/policy/policy-overview.htm)
- [REST API Documentation](https://docs.cyberark.com/conjur-open-source/latest/en/content/developer/lp_rest_api.htm)
- [Secrets Manager CLI Documentation](https://docs.cyberark.com/conjur-open-source/latest/en/content/developer/cli/cli-lp.htm)

## Development

Please see our [CONTRIBUTING guide](./CONTRIBUTING.md) for documentation on how to contribute to this project.

## Getting Started

The CyberArk Conjur PAM Provider is used by Command to resolve PAM-eligible credentials for Universal Orchestrator extensions and for accessing Certificate Authorities. When configured, Command will use the CyberArk Conjur PAM Provider to retrieve credentials needed to communicate with the target system. There are two ways to install the CyberArk Conjur PAM Provider, and you may elect to use one or both methods:

1. **Locally on the Keyfactor Command server**: PAM credential resolution via the CyberArk Conjur PAM Provider will occur on the Keyfactor Command server each time an eligible credential is needed.
2. **Remotely on Universal Orchestrators**: When Jobs are dispatched to Universal Orchestrators, the associated Certificate Store extension assembly will use the CyberArk Conjur PAM Provider to resolve eligible PAM credentials.

Before proceeding with installation, you should consider which pattern is best for your requirements and use case.

### Installation

> [!IMPORTANT]
> For the most up-to-date and complete documentation on how to install a PAM provider extension, please visit our [product documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/ReferenceGuide/Preparing%20Third%20Party%20PAM%20Providers%20to%20Work%20with.htm?Highlight=pam%20provider#InstallingCustomPAMProviderExtensions).

To install CyberArk Conjur PAM Provider, it is recommended you install [kfutil](https://github.com/Keyfactor/kfutil). `kfutil` is a command-line tool that simplifies the process of creating PAM Types in Keyfactor Command.

#### Overview

The CyberArk Conjur PAM retrieves secrets (variables) from CyberArk Conjur instances using either API keys or passwords for a given login identity.

#### Requirements

   ### CyberArk Conjur
   - **Tested with**: CyberArk Conjur OSS v1.24 (latest as of writing)
   - **Minimum version**: CyberArk Conjur OSS v1.19.0
   - **Supported products**: Conjur Open Source, Conjur Enterprise

   ### Keyfactor Platform
   - **Minimum .NET Runtime**: .NET 8
   - **Keyfactor Command**: Version 12.0 or later
   - **Universal Orchestrator**: Version 12.3 or later

#### Create PAM type in Keyfactor Command

##### Using `kfutil`

Create the required PAM Types in the connected Command platform.

```shell
# CyberArk-Conjur
kfutil pam-types create -r cyberark-conjur-pam -n CyberArk-Conjur
```

##### Using the API

For full API docs please visit our [product documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/WebAPI/KeyfactorAPI/PAMProvidersPOSTTypes.htm?Highlight=pam%20type).

Below is the payload to `POST` to the Keyfactor Command API:

```json
{
  "Name": "CyberArk-Conjur",
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
      "Description": "The login that will access the Conjur account and variable within the account. The login must have a policy with “read” and “execute” permissions on the target variable"
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

1. On the server that hosts Keyfactor Command, download and unzip the latest release of the CyberArk Conjur PAM Provider from the [Releases](../../releases) page.

2. Copy the assemblies to the appropriate directories on the Keyfactor Command server:

    <details><summary>Keyfactor Command 11+</summary>

    1. Copy the unzipped assemblies to each of the following directories:

        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\Extensions\cyberark-conjur-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\Extensions\cyberark-conjur-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\Extensions\cyberark-conjur-pam`

    </details>

    <details><summary>Keyfactor Command 10</summary>

    1. Copy the assemblies to each of the following directories:

        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\bin\cyberark-conjur-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\bin\cyberark-conjur-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\bin\cyberark-conjur-pam`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\Service\cyberark-conjur-pam`

    2. Open a text editor on the Keyfactor Command server as an administrator and open the `web.config` file located in the `WebAgentServices` directory.

    3. In the `web.config` file, locate the `<container> </container>` section and add the following registration:

        ```xml
        <container>
            ...
            <!--The following are PAM Provider registrations. Uncomment them to use them in the Keyfactor Product:-->

            <!--Add the following line exactly to register the PAM Provider-->
            <register type="IPAMProvider" mapTo="Keyfactor.Extensions.Pam.CyberArk.Conjur, cyberark-conjur-pam" name="CyberArk-Conjur" />
        </container>
        ```

    4. Repeat steps 2 and 3 for each of the directories listed in step 1. The configuration files are located in the following paths by default:

        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebAgentServices\web.config`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\KeyfactorAPI\web.config`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\WebConsole\web.config`
        * `C:\Program Files\Keyfactor\Keyfactor Platform\Service\CMSTimerService.exe.config`

    </details>

3. Restart the Keyfactor Command services (`iisreset`).

#### Install PAM provider on a Universal Orchestrator Host (Remote)

1. Install the CyberArk Conjur PAM Provider assemblies.

    * **Using kfutil**: On the server that hosts the Universal Orchestrator, run the following command:

        ```shell
        # Windows Server
        kfutil orchestrator extension -e cyberark-conjur-pam@latest --out "C:\Program Files\Keyfactor\Keyfactor Orchestrator\extensions"

        # Linux
        kfutil orchestrator extension -e cyberark-conjur-pam@latest --out "/opt/keyfactor/orchestrator/extensions"
        ```

    * **Manually**: Download the latest release of the CyberArk Conjur PAM Provider from the [Releases](../../releases) page. Extract the contents of the archive to:

        * **Windows Server**: `C:\Program Files\Keyfactor\Keyfactor Orchestrator\extensions\cyberark-conjur-pam`
        * **Linux**: `/opt/keyfactor/orchestrator/extensions/cyberark-conjur-pam`

2. Included in the release is a `manifest.json` file that contains the following object:
    ```json
    {
      "Keyfactor:PAMProviders:CyberArk-Conjur:InitializationInfo": {
        "ConjurUrl": "<ConjurUrl>",
        "AccountId": "<AccountId>",
        "Login": "<Login>",
        "ApiKey": "<ApiKey>"
      }
    }
    ```

    Populate the fields in this object with credentials and configuration data collected in the [requirements](docs/cyberark-conjur.md#requirements) section.

3. Restart the Universal Orchestrator service.

### Usage

#### From Keyfactor Command Host (Local)

##### Define a PAM provider in Command

1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Privileged Access Management**.

2. Select the **Add** button to create a new PAM provider. Click the dropdown for **Provider Type** and select **CyberArk-Conjur**.

   > [!IMPORTANT]
   > If you're running Keyfactor Command 11+, make sure `Remote Provider` is unchecked.

3. Populate the fields with the necessary information collected in the [requirements](docs/cyberark-conjur.md#requirements) section:

   | Initialization parameter | Display Name | Description |
   | --- | --- | --- |
   | ConjurUrl | Conjur URL | The URL of the Conjur server (e.g., https://conjur.example.com) |
   | AccountId | Account ID | The Conjur account identifier |
   | Login | Login | The login that will access the Conjur account and variable within the account. The login must have a policy with “read” and “execute” permissions on the target variable |
   | ApiKey | API Key / Password | The API key or password for the login |

4. Click **Save**. The PAM provider is now available for use in Keyfactor Command.

##### Using the PAM provider

Now, when defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-Conjur** will be available as a PAM provider option. When defining new Certificate Stores, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**.

Select the **Load From PAM Provider** tab, choose the **CyberArk-Conjur** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| VariablePath | Variable (Secret) Path | The full path to the variable (secret) in Conjur (e.g., 'my-app/db-password' or 'production/api-keys/service-token') |

#### From a Universal Orchestrator Host (Remote)

<details><summary>Keyfactor Command 11+</summary>

##### Define a remote PAM provider in Command

In Command 11 and greater, before using the CyberArk-Conjur PAM type, you must define a Remote PAM Provider in the Command portal.

1. In the Keyfactor Command Portal, hover over the ⚙️  (settings) icon in the top right corner of the screen and select **Privileged Access Management**.

2. Select the **Add** button to create a new PAM provider.

3. Make sure that `Remote Provider` is checked.

4. Click the dropdown for **Provider Type** and select **CyberArk-Conjur**.

5. Give the provider a unique name.

6. Click "Save".

##### Using the PAM provider

When defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-Conjur** can be used as a PAM provider. When defining a new Certificate Store, the secret parameter form will display tabs for **Load From Keyfactor Secrets** or **Load From PAM Provider**.

Select the **Load From PAM Provider** tab, choose the **CyberArk-Conjur** provider from the list of **Providers**, and populate the fields with the necessary information from the table below:

| Instance parameter | Display Name | Description |
| --- | --- | --- |
| VariablePath | Variable (Secret) Path | The full path to the variable (secret) in Conjur (e.g., 'my-app/db-password' or 'production/api-keys/service-token') |

</details>

<details><summary>Keyfactor Command 10</summary>

When defining Certificate Stores (**Locations**->**Certificate Stores**), **CyberArk-Conjur** can be used as a PAM provider.

When entering Secret fields, select the **Load From Keyfactor Secrets** tab, and populate the **Secret Value** field with the following JSON object:

```json
{
  "VariablePath": "<VariablePath>"
}
```

Replace each `<placeholder>` with the value described for that instance parameter in the table above.

> We recommend creating this JSON object in a text editor, and copying it into the Secret Value field.

</details>

> [!NOTE]
> Additional information on CyberArk-Conjur can be found in the [supplemental documentation](docs/cyberark-conjur.md).

## Support

The CyberArk Conjur PAM Provider is supported by Keyfactor for Keyfactor customers. If you have a support issue, please open a support ticket via the Keyfactor Support Portal at https://support.keyfactor.com.

> To report a problem or suggest a new feature, use the **[Issues](../../issues)** tab. If you want to contribute actual bug fixes or proposed enhancements, use the **[Pull requests](../../pulls)** tab.

## License

Apache License 2.0, see [LICENSE](LICENSE)

## Related Integrations

See all [Keyfactor PAM Provider extensions](https://github.com/orgs/Keyfactor/repositories?q=pam).
