# Getting Started

## Contents

1. [Create the localisation key schema](#create-the-localisation-key-schema)
2. [Generate code](#generate-code)
3. [Populate Data](#populate-data)
4. [Initialization](#initialization)
5. [Next Steps](#next-steps)

## Create the localisation key schema
**Step 1**

Open the Localisation Window via the toolbar. `DUCK -> Localisation`

![Open Localisation Window](./open-window.png)

**Step 2**

In the localisation window click "Create new LocalisationSettgings". 

![Create new settings](./create-new-settings.png)

This creates a new `LocalisationSettings.asset` config file in root of the project. Move this a suitable location and rename it if you like. **You will only need one of these for your project.**

The settings object is a configuration file for localisation in your project.
* It defines a code generation file path (for generating an enum with all localisation keys).
* It defines the path to the folder that will contain your localisation tables.
* It defines a schema. The schema stores all the categories and localisation keys that you will use in your project. Each piece of localised content should have a key in here and belong to a category.

**Step 3**

Select the newly created `LocalisationSettings` object, Add some categories via the inspector. A category is a set of related localisation keys. They are related by the type content or area of the game (eg DialogueTexts, ButtonTexts, ErrorMessages, CharacterNames etc..).
Some Example Categories are shown below:
* MainMenu
* OptionsMenu
* DialogueText
* CharacterNames
* ItemsNames
* LevelNames

Create a new category

![Create category](./create-category.png)

Expand the category

![Expand category](./expand-category.png)

Congfigure and add keys to the category

![Expanded category](./expanded-category.png)

Example of populated category "MainMenuText"

![Example category](./example-category.png)

## Generate code
Once the schema is populated with meaningful categories keys. It's time to generate a little bit of code. The system generates a class containing all categories/keys as enums, whose values are encoded as crc values for faster access. Through this class your code can refer to any localisation key, in a type safe manner.

Any time you change the schema you will need to regenerate this file.

To generate the file, in the Localisation window under "Generate localisation consts", you can click "Generate Consts" (1).
You can also specify your own template (2), although one is provided. 

It will be created in the file path defined in the settings.

![Generate Consts](./generate-consts.png)

## Populate Data
Now we need to populate some localisation tables, so our keys actually have some values against them.

In the localisation window click "Find/Refresh."
 
![Find and refresh](./find-and-refresh.png)

This automatically searches the project for existing tables and checks them for missing values. It will show any tables that have missing values. This can happen when you add new keys but haven't added the values yet.

To create a new table click "Create new"

![Create new table](./create-new-table.png)

This creates a table in the location specified in the settings location.

This table asset represents the entire set of translation data for a set of locales. Name it "English" (for example).

Select the table and then in the inspector there are a few options. Shown below:

![Table config](./table-config.png).

1) Configure which locales are supported by this table. In many cases this is only a single locale, however it allows multiple so a single set of translations can be used to support multiple regions. In our example the "English" table supports `en-US` & `en-GB`. It could also support any number of english speaking countries, however there isn't anything stopping us from splitting it and having unique tables for American & UK English. Each value in here should be a valid [Culture name](https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.name?view=netframework-4.8#property-value).

2) Save & Load options allow us to import data and export data from CSVs. There is also an option "Empty values only" which allows us to generate a CSV showing the keys that are missing.

3) Allows us to view & edit the values stored agains the keys, grouped by categories. The options above also allow us to toggle viewing CRC values for each key, and shows us the CRC version used.

## Initialization

At the start of the app call the following function.

`Localiser.Initialise("Localisation"); // default: looks in Resources/Localisation and starts in the user's device language`

or alternatively...

`Localiser.Initialise("OtherFolderNameInResources"); // to look for tables in a folder other than Resources/Localisation`
`Localiser.Initialise("Localisation", "en-GB"); // to set a specific starting language`

In most cases, your default table should have both en-GB and en-US supported, and the localiser will retrieve all compatible tables from this folder.

## Next steps
It's now ready to use. See [Basic Usage](./BasicUsage.md).
