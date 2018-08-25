# unity-localization :earth_americas:
A small framework for localization within Unity (or other C# applications)

## Known Limitations
* Plurals (ie. *Apple*/*Apples*) are not currently handled by the system. Support for this will be added later.
* Localization of units (Imperial/Metric) is not handled (also to be supported later)
* Renaming a key with `autoGenerateLanguageClass` turned on will cause compiler errors. To get around this, either refactor/rename the key in code before updating the .csv, or do a bulk *find and replace*.

## To add translations
1. Open the localization config `Config -> Localization` from the menu at the top of the Unity editor
2. Add any .csv files with translations you wish to include to the list

Translations should have a key colum as the first column, each following column should be decorated with the code for the language (ie. en for english). You can also provide a country code (ie. en-US) for greater control over translations within a single language.

Your translation file might look something like this:

| key | en | en-AU | fr | de | es |
| :--- | :--- | :--- | :--- | :--- | :--- |
| general.greeting | Hello | G'Day | Arrière | Zurück | Espalda |

## Placeholder Values
Any text you place inside curly braces (ie. `{name}`) will be considered a placeholder value. Placeholders will be filled in when creating a translated string.  You can have as many placeholder values as you like, and placeholder values with the same name will be considered the same for population purposes. *see below for usage*

## Using Translated Text
To use translated text, you'll be using the `localstring` class. This acts as a container for a key/argument pair, and will resolve to an actual translated string when needed.

By default the option `autoGenerateLanguageClass` is turned on. This will generate a file in your project that will contain every key for the translations you have added (So you don't need to litter string literals throughout your code). This also comes bundled with added intellisense for the first translation provided to make it easier to use the correct translation.

Once you've added the translations, using translated text is easy:
```cs
localstring simpleText = keys.general.back;
Debug.Log(simpleText); // 'Back'
```

If the text has placeholder values, then you will need to provide the values for each unique value. You can also provide a `localstring` as a parameter, it will be recursively translated before being converted to a string.
```cs
localstring greetingText = keys.greetings.personal(name: "Tony"); // 'Hello {name}, how are you'
Debug.Log(greetingText); // 'Hello Tony, how are you?'

localstring nestedText = keys.requests.fetch_food(quantity: 12, food: keys.food.apples); // 'I need {quantity} {food}, do you think you can get the {food} for me?'
Debug.Log(nestedText); // 'I need 12 Apples, do you think you can get the Apples for me?'
```

If you wish to turn off `autoGenerateLanguageClass`, you can still use the localization system with the following:
```cs
localstring translatedText = "{YOUR KEY HERE}"; // Without Parameters
localstring translatedText = new localstring("{YOUR KEY HERE}", arg0, arg1 ... argX) // With parameters
```

## Setting Locale
To set the current language, you have a few options, you can either use Unity's inbuilt `SystemLanguage`, or use the iso code for the language and (optionally) country. If selecting a precise locale with no corresponding entries in the translation files, it will fall back to the imprecise locale (ie. if set to *en-GB*, and no column labeled *en-GB* is not present, returned translations will instead be from the column labeled *en*, if available)
```cs
Localization.SetLanguage(SystemLanguage.English); // Unity SystemLanguage (Not Precise)
Localization.SetLanguage("en"); // Language Only (Not Precise)
Localization.SetLanguage("en-AU"); // Combined Language/Country (Precise)
Localization.SetLanguage("en", "AU"); // Split Language/Country (Precise)
```