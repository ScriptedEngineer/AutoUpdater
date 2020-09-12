# Auto Updater
## Command line: 
Set what EXE file need to run - `/RunApp "TwitchBot.exe"`<br>
Set Repository - `/GitHub "Author/Repository"`<br>
Set update log language (if need) - `/LogLang RU`<br>

**in end of the command line**<br>
Set version - `/Version "0.1.0.7"`<br>
Or set just download option - `/JustDownload`

Русификация(добавить в начало параметров) - `/Title "Обновление" /WhatNew "Что нового:" /Available "Доступна версия" /Current "Установленная версия" /RemindLater "Напомнить позже" /UpdateNow "Обновить сейчас" /DownloadingFile "Загрузка обновления" /ExtractingUpdate "Распаковка обновления" /PleaseWait "Пожалуйста подождите..."`

## GitHub Releases description example
### Multi language
```
# [EN] Changes since 1.0.0.0
### Fixes
- Fixed: Some problem

# [RU] Изменения с версии 1.0.0.0
### Исправления
- Исправлено: Какая-то проблема
```
Syntax
```
# [Language] TEXT
### Ingored
- TEXT
```
### Single language
```
# Changes since 1.0.0.0
### Fixes
- Fixed: Some problem
```
Syntax
```
# TEXT
### Ingored
- TEXT
```

