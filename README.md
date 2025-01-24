#Name
HryvniaRateBot

#Description
This bot is designed to retrieve and display the exchange rate of foreign currencies to UAH on a specific date. It uses the PrivatBank public API to fetch data and is built with the Telegram.Bot library.
The bot supports error handling for invalid inputs such as incorrect currency codes, invalid date formats, or missing data for a particular request. It`s also can be fast modified for another bank API

#Usage
Users can interact with the bot by sending:

A currency code (e.g., USD).
A specific date in the format dd.MM.yyyy.
The bot will respond with the exchange rate for the specified currency and date.

It provides clear error messages when:

The currency code is invalid.
The date format is incorrect.
No data exists for the requested parameters.

#Roadmap
Add support for multi-language responses.
Implement advanced filtering and sorting options.
Extend functionality to support historical exchange rate trends.
Introduce a web-based UI for easier interaction.

#Contributing
Contributions are welcome! You can open issues or create pull requests to improve the bot's functionality or documentation.

#Authors and Acknowledgment
Author: Nikita Shynkar
Acknowledgment: Foxminded student

#Project Status
In progress.
