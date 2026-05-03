# מערכת ניהול פנימיות — Dormitory Management System

מערכת Web לניהול פנימיות ותלמידים עבור המינהל לחינוך התיישבותי.

---

## סטאק טכנולוגי

| שכבה | טכנולוגיה |
|------|-----------|
| מסד נתונים | SQL Server |
| צד שרת | C# .NET 8 Web API |
| גישה לנתונים | Dapper |
| לוגים | Serilog (Console + File) |
| צד לקוח | AngularJS 1.8.3 |

---

## מבנה הפרויקט

```
dormitory-management/
├── Database/
│   ├── 01_CreateTables.sql       ← יצירת טבלאות + אינדקסים + FK
│   ├── 02_SeedData.sql           ← נתוני דוגמה
│   └── 03_StoredProcedures.sql   ← GetEducationPlaceSummaries + UpsertStudent
├── Backend/
│   └── DormitoryManagement.API/
│       ├── Controllers/          ← EducationPlacesController, StudentsController
│       ├── Services/             ← IEducationPlaceService, IStudentService + מימושים
│       ├── Data/                 ← IEducationPlaceRepository, IStudentRepository + מימושים
│       ├── Models/               ← EducationPlace, Student, DTOs, ErrorResponse
│       ├── Exceptions/           ← NotFoundException, ValidationException
│       ├── Middleware/           ← GlobalExceptionMiddleware
│       └── Program.cs
└── Frontend/
    ├── index.html
    ├── app/
    │   ├── app.module.js
    │   ├── controllers/mainController.js
    │   └── services/dormitoryService.js
    └── styles/main.css
```

---

## הרצת הפרויקט

### 1. מסד נתונים
```sql
-- הרץ בסדר הבא ב-SQL Server Management Studio:
Database/01_CreateTables.sql
Database/02_SeedData.sql
Database/03_StoredProcedures.sql
```

### 2. Backend
```bash
cd Backend/DormitoryManagement.API

# עדכן את connection string ב-appsettings.Development.json
# ואז:
dotnet restore
dotnet run
# ה-API יעלה על http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

ערך החיבור נמצא תחת:
`Database:ConnectionString`

### 3. Frontend
פתח את `Frontend/index.html` דרך VS Code Live Server (פורט 5500)
או כל Web Server מקומי אחר.

> **הערה:** הפתיחה ישירות מהקובץ (`file://`) אפשרית עם שינוי CORS ב-Program.cs להוספת `"null"` לרשימת המקורות המורשים.

---

## Design Patterns ששימשו

### 1. Repository Pattern
כל גישה לנתונים מוסתרת מאחורי ממשק (`IEducationPlaceRepository`, `IStudentRepository`).
הקונטרולרים והשירותים אינם יודעים כלל על Dapper, SqlConnection, או ש מחרוזות SQL.
זה מאפשר להחליף את שכבת ה-ORM בעתיד (למשל ל-EF Core) ללא שינוי בשכבות מעל.

### 2. Service Layer Pattern
כל לוגיקה עסקית (ולידציה חוצת-ישויות, בדיקת קיום רשומות) יושבת ב-`StudentService` ולא בקונטרולר.
הקונטרולר אחראי אך ורק על מיפוי HTTP — קבלת בקשה, קריאה לשירות, החזרת תשובה.

### 3. Dependency Injection
כל התלויות מוזרקות דרך ה-constructor ורשומות ב-`Program.cs`.
אין `new` ידני — זה מאפשר unit testing פשוט עם mocking.

### 4. Middleware / Chain of Responsibility
`GlobalExceptionMiddleware` עוטף את כל ה-pipeline ומטפל בשגיאות בנקודה אחת מרוכזת.
כל exception נלכד, מולוג, ממופה ל-HTTP status code מתאים ומוחזר כ-JSON אחיד.

### 5. DTO Pattern
`StudentUpsertDto` ו-`EducationPlaceSummaryDto` מפרידים בין מודל ה-DB לבין ה-API contract.
זה מונע חשיפת שדות פנימיים ומאפשר לשנות את הסכמה הפנימית ללא שבירת ה-API.

### 6. Facade Pattern (Frontend)
`DormitoryService` ב-AngularJS מספק ממשק פשוט לכל קריאות ה-API.
הקונטרולר אינו יודע על `fetch`, URLs, או פרסור JSON — הוא רק קורא ל-`getDormitories()`.

---

## שימוש בכלי AI

### כיצד השתמשתי ב-AI במהלך המטלה?
- **GitHub Copilot** — להשלמה אוטומטית של boilerplate code (רישום DI, middleware skeleton, Dapper queries).
- **Claude** — לסקירת ארכיטקטורה: הצגתי את מבנה השכבות וקיבלתי הערות על הפרדת אחריות נכונה.
- **ChatGPT** — לבדיקת תאימות CORS בין AngularJS ו-.NET 8, ולמציאת הפתרון הנכון לשימוש ב-`async/await` בתוך AngularJS digest cycle (שימוש ב-`safeApply`).

בסך הכל, AI קיצר את זמן ה-boilerplate בכ-40% ואפשר להתמקד בהחלטות ארכיטקטוניות.

---

## טמעת AI בשגרת הצוות

כמוביל צוות, כך הייתי מטמיע כלי AI ב-workflow היומיומי:

**פיתוח:**
- GitHub Copilot חייב ב-IDE של כל מפתח. מאפשר להשלים unit tests, boilerplate, ו-SQL queries.
- Prompt templates ב-Copilot Chat לדפוסים חוזרים (יצירת CRUD controller, הוספת endpoint חדש לפי הפטרן הקיים).

**Code Review:**
- AI-assisted PR review (למשל GitHub Copilot for PRs) לזיהוי בעיות טריוויאליות לפני סבב ה-Review האנושי.
- זה מקצר את ה-review cycle ומפנה את הרוויוור לבעיות ארכיטקטוניות ועסקיות.

**QA & Testing:**
- Copilot לייצור unit tests אוטומטי — מפתח כותב פונקציה, Copilot מציע test cases כולל edge cases.
- AI לייצור test data (seed scripts, Postman collections).

**Onboarding:**
- Copilot Chat כ"שאל-את-הקוד" — מפתח חדש שואל "מה עושה `StudentService.UpsertAsync`?" ומקבל הסבר מהקוד עצמו.

---

## Code Review — הוספת מודול "מורים"

אם מפתח ג'וניור היה מוסיף מודול מורים, הייתי מדגיש את הנקודות הבאות ב-Code Review:

1. **עקביות ארכיטקטורית** — `Teacher` → `TeacherController` → `ITeacherService` → `ITeacherRepository`. אין לוגיקה עסקית בקונטרולר.

2. **DB migration script** — `04_AddTeachersTable.sql` עם הגדרת FK ל-`EducationPlace`, אינדקסים רלוונטיים, ו-CHECK constraints. לא לשנות ידנית את הסכמה ב-Production.

3. **DTO נפרד מה-entity** — `TeacherUpsertDto` עם Data Annotations לוולידציה. לא לחשוף את ה-`Teacher` entity ישירות כ-request body.

4. **ולידציה חוצת-ישויות בשירות** — בדיקה שה-`EducationPlaceId` קיים מתבצעת ב-`TeacherService`, לא בקונטרולר ולא ב-Repository.

5. **DI Registration** — `services.AddScoped<ITeacherRepository, TeacherRepository>()` חייב להיות ב-`Program.cs`. לא ליצור instances ידנית.

6. **Async/Await עד הסוף** — כל פעולת I/O חייבת להיות async. `GetAllAsync()` ולא `GetAll()`.

7. **Unit Tests** — לפחות test ל-Happy Path ו-`NotFoundException` ב-`TeacherService` לפני merge.

8. **Naming conventions** — עברית אסורה בשמות משתנים, מחלקות, ומתודות. קומנטים בעברית — מקובל.
