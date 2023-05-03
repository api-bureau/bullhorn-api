# Bullhorn API

This project aims to simplify your experience with the Bullhorn APIs by providing a C# and .NET Core implementation. It helps you explore Bullhorn APIs and speed up your development process.

## ⚠️ Important Note on Updating Bullhorn Entities

When updating Bullhorn entities, **make sure to update only the fields you want to change**. Do not use DTOs with multiple fields that you do not intend to update, as this will overwrite the existing Bullhorn entity with default values.

## ⚒️ Bullhorn API Browser

This Blazor application assists you in exploring the Bullhorn APIs. It uses the Bullhorn API client to interact with the Bullhorn APIs, allowing you to test the APIs through a user-friendly interface.

### 🔐 Setting Up secret.json

To get started, fill in the `secret.json` file with the Bullhorn credentials provided by Bullhorn. The following is a template of the `secret.json` file:

 ```json
 {
  "BullhornSettings": {
    "Soap": {
      "UserName": "",
      "Password": "",
      "ApiKey": ""
    },
    "RestApi": {
      "AuthorizeUrl": "https://auth.bullhornstaffing.com/oauth/authorize",
      "LoginUrl": "https://rest-emea.bullhornstaffing.com/rest-services/login",
      "TokenUrl": "https://auth.bullhornstaffing.com/oauth/token",
      "AuthorizationParameter": "code",
      "UserName": "",
      "Password": "",
      "ClientId": "",
      "Secret": ""
    }
  }
}
 ```

 Replace the empty strings with your specific Bullhorn credentials. Once you have completed the secret.json file, you will be able to use the Bullhorn API Browser to interact with Bullhorn APIs.

## Contributors
This project adheres following guidelines.
- https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md
- https://github.com/dotnet/runtime/tree/main/docs#coding-guidelines

## Code of Conduct
This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/).
