# Requirements Summary

## User

The system should display each team member grouped by a category

Each category should be presented in separate section

"Each section should display:
- the category name,
- short description of a category,
- the team member card(in a specific, pre-defined order)"

"Categories should be displayed in the following order from top to bottom:
- Основна команда
- Нагладова рада
- Радники"

"Team member card should display:
- image of a team member,
- name of a team member,
- descriptions of a team member"

"When a User opens the window ‘Команда” the system should display records in the view list:
- grouped by categories
- ordered by their configured position on the 'Team' page within each category"

When a the search bar is empty the system should display all team members stored in the system

When a user starts typing a team member’s name in the search bar, the system should display records in the view list that match the entered text in the “ПІБ” column.

When the Team page is uploaded the system should play the GIF-file in the quote section and display the quote text in front

## Admin

"When an Admin clicks on the button ‘Додати в команду’ (modal window appears) with the following fields:

| Поле       | Тип введення        |
|------------|---------------------|
| Категорія  | drop-down list      |
| ПІБ        | text input          |
| Опис       | text input          |
| Image      | file                |


The ‘Категорія’ field should display the placeholder ‘Оберіть категорію”
Fields ПІБ , Опис should be initially empty

Validation:
| Поле       | Обов’язковість | Обмеження                          |
|------------|----------------|------------------------------------|
| Категорія  | mandatory      | -                                  |
| ПІБ        | mandatory      | 50 characters                      |
| Опис       | optional       | 200 characters                     |
| Image      | mandatory      | 100KB, pdf, jpeg, jpg              |


"Team members details should be displayed in the following columns:
- ПІБ > name of a team member
- Статус >publication status assigned to the member Не опубліковано/Опубліковано
- Категория > category assigned to a member"

The windows should display maximum of 10 team members records per page

If there are more than 10 team members stored in the database, the system should display a page-counter (pagination) matching the mock-up

Admin should be able to edit/delete all fields

The ‘Категорія’ field should display the placeholder ‘Оберіть категорію”

"The field with placeholder “Cтатус” should be a drop-down list:
- Всі записи
- Опубліковано
- Неопубліковано"

"The field with placeholder“Категорія”  should be a drop-down list:
- 'Всі категоріі'
- Categories stored in the system"


## Messages 

- “Помилка збереження” : “Оберіть категорію” Ok
- “Помилка збереження” : “Введіть ПІБ” Ok
- “Помилка збереження” : “Перевищення символів для ПІБ” Ok
- “Помилка збереження” : “Завантажте фото” Ok
- “Помилка збереження” : “Перевищення символів для Опису” Ok
- ‘Помилка збереження’ ”Недопустимий формат фото, завантажте pdf, jpeg, jpg” Ok
- ‘Помилка збереження’ ”Розмір фото більше 100 KB” Ok
- “Зберегти нового члена команди?”  “НІ”  “ТАК”
- “Видалити члена команди?”  “НІ” “ТАК”
- " Зберегти зміни?" “НІ” “ТАК”

