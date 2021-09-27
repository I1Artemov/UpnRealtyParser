# UpnRealtyParser
UPN and N1 sites content parser and visualizer

## Overall description
This project is created for collecting, aggregating and analyzing realty data in Yekaterinburg, Russia.
For example, Yandex have a functionality of the payback map, which shows the profitability of buing apartments in different locations. But there is a problem: it is only accessible for Moscow. So I decided to implement something similar for my region (it's one of many features in UpnRealtyParser.Fronted).

Technologies used in this project: C#, Asp.Net Core 2.1, React+Redux, AntDesign Framework, Entity Framework Core.

## Solution structure
The Solution consists of 7 projects:

1. **UpnRealtyParser.Frontend** - Web-interface written using Asp.Net Core, React, Redux and AntDesign as dwsign framework.

2. **UpnRealtyParser.Business** - Contains all data models, helper classes with business logic and DB infrastructure code. Entity framework is used as ORM.

3. **UpnRealtyParser.Service** - Console application for gathering UPN fltas for sell and rent.

4. **UpnRealtyParser.Service.N1** - Console application for collecting N1 flats.

5. **UpnRealtyParser.Service.Photo** - Console app, that downloads UPN flat photos. It uses previously saved image links, works through simple HTTP requests.

6. **SeleniumPhotoService** - Separate service for gathering UPN house photos using browser automatization (because of URL deprecation for sessions problem).
