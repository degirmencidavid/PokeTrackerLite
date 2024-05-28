<h1>PokeTrackerLite</h1>

PokeTrackerLite is a simple early build of the PokeTracker that I developed for my own investment and collection management. (The "full" version will not be released publicly). There are some minor bugs, but it is functionally sound.

The purpose of this repository is to provide a learning tool for asynchronous programming, API integration and webscraping that can be used as a starting point to build from.

The key features that this does not include are price management and tracking options (other than gathering prices upon opening up a card).

The UI is built with windows forms.

<h2>Functionality</h2>

<h3>Login Screen</h3>

![image](https://github.com/degirmencidavid/PokeTrackerLite/assets/101801691/c06a65a2-bfd3-4791-81cb-842d43d2eaa6)

As you may notice, there are two separate login buttons. The upper login button is an autologin button, which uses the default username and password "admin" and "a" respectively. If you do some digging within the application, you'll see that the admin has some extra functionality. Logging in and registering are both fully functional, given you have set up your database (see section X).

<h3>Main Form</h3>

![image](https://github.com/degirmencidavid/PokeTrackerLite/assets/101801691/d241bd8b-5bf1-4686-ac68-8472509c12fd)

Here is the main form. As you can see, almost everything in the UI is placeholder. UI elements are very easy to replace and edit, so do it yourself.

You can search for a card or card set in either English or Japanese. The functionality to search a set by name/set no. is redundant and so not possible. The search will return a list of cards like so:

![image](https://github.com/degirmencidavid/PokeTrackerLite/assets/101801691/f00f6c73-ae20-491c-af07-a60e69370615)

Double clicking a card in a list like this will open the card properties:

![image](https://github.com/degirmencidavid/PokeTrackerLite/assets/101801691/cda0cfb0-3c14-4b78-8215-ad1315fe0a52)

Upon opening a Japanese card, prices will be obtained from 3 Japanese card storesCardRush, Furu1, and ManaSource. Blue prices are in stock, and grey prices are low/empty stock. A somewhat unpleasant to view graph of the price history of the card in various conditions is also presented with the current estimated price for these conditions shown on the left of the graph (in the same order as the legend on the graph); this data is obtained from pricecharting.com.
Feel free to implement a data visualisation tool to present this data more pleasantly.

The "Cards" button allows the user to manually add in a card along with properties and an image:

![image](https://github.com/degirmencidavid/PokeTrackerLite/assets/101801691/319c44fa-11e0-4257-bb39-2b26117d91be)

This is mostly redundant, but has its use cases in fixing errors with set scraping (see Set Scraping section).

The "Sets" button allows the user to access a list of all the sets:

![image](https://github.com/degirmencidavid/PokeTrackerLite/assets/101801691/2c8dba52-c5a8-4210-ac6a-c483054d9d86)

By double clicking on a set in the table, the user can access a list of cards in that set:

![image](https://github.com/degirmencidavid/PokeTrackerLite/assets/101801691/a8730f10-4a28-4b96-bb3a-1abc956e5b3d)

Back to the Main Form, the "Users" button is purely for debugging (admin login access only) and insecurely reveals user information, with the option to add, edit, and delete users.
Here is a sample with dummy data (the passwords are encrypted):

![image](https://github.com/degirmencidavid/PokeTrackerLite/assets/101801691/2e02b53f-cc58-4b87-a258-dd40db4c33d1)

The "My Collection" button allows a user to view a table of the cards in their collection along with conditions:

![image](https://github.com/degirmencidavid/PokeTrackerLite/assets/101801691/c64d3663-90d4-4554-925f-9af752da5169)

The "Types" and "Valuation" buttons are left unimplemented.

<h3>Set Scraping</h3>
Now for the huge "Set Scraper" button. This is a tool to download all the cards in a set, along with their metadata, from pokellector.com and/or jp.pokellector.com (depending on language).
It will download up to 10 cards simultaneously (this is the sweet spot before the site starts to throttle traffic).

![image](https://github.com/degirmencidavid/PokeTrackerLite/assets/101801691/088c72de-fd3f-4085-9a87-3c6a6a66d635)

The set will now be viewable in the sets form (as shown before).

<h2>Setup:</h2>

You'll need to set up your own database, the SQL syntax is Microsoft SQL Server. You'll need to connect to your database, the connection string is set in PokeTrackerLite/PokemonCollection/Utilities
/DataAccess.cs

If you are unfamiliar with how to set up your database, I encourage you to stop here and learn how to do it.

<h3>Section X - The structure of the database:</h3>

There are 4 tables that you can generate using the following SQL:

Cards Table:
``` sql
CREATE TABLE [dbo].[tbCards] (
    [CardID]       AS              (CONVERT([varchar](255),[SetID])+[SetNumber]) PERSISTED NOT NULL,
    [CardName]     VARCHAR (255)   NOT NULL,
    [CardPrice]    VARCHAR (255)   NULL,
    [CardLanguage] VARCHAR (50)    NOT NULL,
    [SetNumber]    VARCHAR (20)    NOT NULL,
    [SetID]        VARCHAR (50)    NOT NULL,
    [CardImage]    VARBINARY (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([CardID] ASC),
    FOREIGN KEY ([SetID]) REFERENCES [dbo].[tbCardSets] ([SetID])
);
```

``` sql
CREATE TABLE [dbo].[tbCardSets] (
    [SetID]      VARCHAR (50)    NOT NULL,
    [SetName]    VARCHAR (255)   NOT NULL,
    [TotalCards] INT             NOT NULL,
    [Released]   VARCHAR (50)    NOT NULL,
    [SetImage]   VARBINARY (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([SetID] ASC)
);
```

``` sql
CREATE TABLE [dbo].[tbUser] (
    [username]     VARCHAR (50)  NOT NULL,
    [fullname]     VARCHAR (50)  NOT NULL,
    [password]     VARCHAR (MAX) NOT NULL,
    [phone]        VARCHAR (50)  NOT NULL,
    [UserRole]     VARCHAR (50)  DEFAULT ('user') NOT NULL,
    [UserID]       INT           IDENTITY (1, 1) NOT NULL,
    [SessionToken] VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([UserID] ASC)
);
```

``` sql
CREATE TABLE [dbo].[tbUserCollection] (
    [UserID]         INT           NOT NULL,
    [CardID]         VARCHAR (255) NOT NULL,
    [ConditionA]     INT           NULL,
    [ConditionAM]    INT           NULL,
    [ConditionB]     INT           NULL,
    [ConditionC]     INT           NULL,
    [ConditionPSA10] INT           NULL,
    [ConditionPSA9]  INT           NULL,
    [ConditionPSA8]  INT           NULL,
    CONSTRAINT [PK_tbUserCollection] PRIMARY KEY CLUSTERED ([CardID] ASC)
);
```
