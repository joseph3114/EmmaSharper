# EmmaSharper

A .NET wrapper for the [Emma API](http://api.myemma.com/).

Targeting Frameworks

- `netcoreapp3.1`
- `net5.0`
- `net6.0`
- `net7.0`

## Sample Usage

The examples below shows how to register the Emma providers with Microsoft DI.

```C#
    using EmmaSharper;

    ServiceCollection.AddEmmaApiProviders(options => {
        AccountId = "Your Account ID";
        PublicKey = "Your Public Key";
        SecretKey = "Your Secret Key";
        BaseUrl = "API URL"; // This is optional and defaults to https://api.e2ma.net
    });
```

The following providers are available:

### `IAutomationProvider`

Provides access to automation APIs

### `IFieldsProvider`

Provides access to custom fields in your account. Of particular interest is the ClearField method which lets you clear out all the data in a single field for all members in your account. This makes it easy to re-initialize a dataset if you’re looking to correct an import error or syncing issue

### `IGroupProvider`

Provides access to manage all aspects of the groups in your account. In addition to various CRUD methods, you can also use these endpoints to manage the members of your groups. You’ll want to use these methods if you’re managing group membership for more than one member at a time. For dealing with single members, there are better methods in the members endpoints.

### `IMailingProvider`

Provides a way to retrieve information about your mailings including their HTML contents. You can retrieve the members to whom the mailing was sent. You can also pause mailings and cancel mailings that are pending or paused.

### `IMemberProvider`

| In addition to the various CRUD endpoints here related to members, you can also change the status of members, including opting them out. You’ll notice that there are calls related to individual members, but we also provide quite a few calls to deal with bulk updates of members. Please try to use these whenever possible as opposed to looping through a list of members and calling the individual member calls. Where this is especially important is when adding new members. To do a bulk import, you’ll POST to the `AddNewMembers` method. In return, you’ll receive an import ID. You can use this ID to check the status and results of your import. Imports are generally pretty fast, but the time to completion can vary with greater system usage.

### `IResponseProvider`

Provides access to response data. You can get overview numbers for all of your mailings and also drill down into finding out the actual members who opened a particular mailing.

### `ISearchProvider`

Provides access to create, edit, and delete searches. You can also retrieve the members matching any search created in your account.

### `ISignupFormProvider`

Provides a list of all of your sign-up forms

### `ISubscriptionProvider`

Provides access to subscriptions and subscription members

### `IWebhookProvider`

Provides access to webhooks

## Getting Started

This version of EmmaSharper is available on NuGet under [EmmaSharper](https://www.nuget.org/packages/EmmaSharper/latest)

```cmd
Install-Package EmmaSharper
```

## Status of the Project

The original project, EmmaSharp, created by [kylegregory](https://github.com/kylegregory/EmmaSharp) appears to be abandoned. This new project is a living fork from the original work. Feel free to contribute. Most API endpoints have methods in the library. If you have any questions, feel free to submit an issue for a particular entity or endpoint. Emma API documentation can be found at https://api.myemma.com.

### Making contributions

This project is not affiliated with [Emma](http://myemma.com/meet-us). All contributors to this project are unpaid average folks (just like you!) who choose to volunteer their time. If you like Emma and want to contribute, we would appreciate your help! To get started, just [fork the repo](https://help.github.com/articles/fork-a-repo), make your changes and submit a pull request.
