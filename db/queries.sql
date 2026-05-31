-- 1. Выборка с условиями
SELECT first_name AS 'имя', last_name AS 'фамилия', salary AS 'зарплата' 
FROM teachers 
WHERE first_name LIKE 'Ж%' AND last_name LIKE 'Д%';

-- 2. Группировка и сортировка
SELECT g.group_name, COUNT(*) 'AS число в группе' 
FROM groups g
JOIN students s ON s.group_id = g.id
GROUP BY g.id, g.group_name
ORDER BY 'число в группе';


-- 3. Удаление
DELETE FROM teachers WHERE salary >= 15000;


-- 4. Обновление
UPDATE students 
SET phone_number = '88005555' 
WHERE first_name = 'Судак' AND last_name = 'Валерий';


SELECT 
    d.name AS 'Кафедра',
    d.budget AS 'Бюджет',
    g.group_name AS 'Группа',
    g.average_score AS 'Средний балл группы'
FROM departments d 
LEFT JOIN groups g ON g.department_id = d.id;


-- 6. Правое соединение
SELECT 
    g.group_name AS 'Группа',
    s.first_name AS 'Имя студента',
    s.last_name AS 'Фамилия студента'
FROM students s
RIGHT JOIN groups g ON s.group_id = g.id;


-- 7. Обычное соединение
SELECT 
    l.subject_name AS 'Предмет',
    l.lesson_time AS 'Время занятия',
    CONCAT(t.first_name, ' ', t.last_name) AS 'Преподаватель',
    g.group_name AS 'Группа'
FROM lessons l
INNER JOIN teachers t ON l.teacher_id = t.id
INNER JOIN groups g ON l.group_id = g.id;