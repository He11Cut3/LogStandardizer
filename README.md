# LogStandardizer

![Static Badge](https://img.shields.io/badge/Language-C%23-blue?logo=c-sharp)
![Static Badge](https://img.shields.io/badge/.NET-8.0-purple)

Консольное приложение для стандартизации лог-файлов, преобразующее записи разных форматов в единую структурированную форму.

## Описание

**LogStandardizer** выполняет:

- Преобразование логов из 2 форматов в стандартизированный вид
- Валидацию и обработку ошибок
- Сохранение проблемных записей в отдельный файл
- Поддержку русского языка в сообщениях

### Поддерживаемые форматы на входе:
1. **Формат 1**:  
   `10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'`
2. **Формат 2**:  
   `2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства...`

### Выходной формат:
`Дата[TAB]Время[TAB]Уровень[TAB]Метод[TAB]Сообщение`
