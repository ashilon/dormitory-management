# תשובות לשאלות ידע — חלק ב' וחלק ג'

---

## חלק ב' — ארכיטקטורה ומנהיגות

---

### שאלה 1: שיפור ביצועי שאילתה איטית ב-SQL Server

**אבחון:**

הצעד הראשון הוא **מדידה לפני אופטימיזציה**. אדליק `SET STATISTICS TIME, IO ON` לפני הרצת השאילתה ואקרא את ה-Output — כמה logical reads יש? כמה זמן CPU לעומת elapsed? לאחר מכן אשתמש ב-**Actual Execution Plan** (Ctrl+M ב-SSMS) כדי לאתר אופרטורים יקרים: Table Scan / Index Scan במקום Index Seek מרמזים על חסרון אינדקס. נוסף על כך, אשתמש ב-`sys.dm_db_missing_index_details` ו-`sys.dm_exec_query_stats` כדי לאתר שאילתות יקרות בסביבת Production ללא השפעה על המשתמשים.

**שיפור:**

לאחר שאיתרנו את צוואר הבקבוק — אוסיף אינדקס מכסה (Covering Index) הכולל את עמודות ה-`WHERE` ו-`GROUP BY` ב-key columns, ואת עמודות ה-`SELECT` ב-`INCLUDE`. אם הבעיה היא **Parameter Sniffing** — ה-SQL Server "בחר" תוכנית הרצה גרועה בהתבסס על פרמטר לא ייצוגי — אשתמש ב-`OPTION (OPTIMIZE FOR UNKNOWN)` או ב-`OPTION (RECOMPILE)` לשאילתות עם variance גבוה.

**דגשים ל-Stored Procedures שכדאי להימנע מהם:**

- **`SELECT *`** — תמיד לציין עמודות מפורשות; מונע key lookups מיותרים.
- **Scalar UDFs בתוך WHERE / JOIN** — מונעים parallelism ומוערכים שורה-שורה.
- **המרות implicit** — למשל `WHERE VarcharCol = 1` יגרום ל-full scan. להתאים types.
- **Cursors** — כמעט תמיד ניתן להחליף ב-set-based operation.
- **`NOLOCK`** — פשוט להשתמש בו, מסוכן לנתונים — יכול להחזיר dirty reads ו-phantom rows. לשקול Read Committed Snapshot Isolation ברמת ה-DB.

---

### שאלה 2: מעבר מ-AngularJS ל-Angular מודרני תוך כדי Agile

**אני בוחר בגישה ההיברידית — ngUpgrade / Strangler Fig.**

**מדוע לא "מפץ גדול" (Big Bang)?**
Big Bang rewrite פירושו הקפאת פיתוח פיצ'רים עסקיים חדשים לחודשים ארוכים, סיכון גבוה של regression בכל פיצ'ר שנדרש ולמעשה מסירת גרסה אחרת לחלוטין ביום אחד ללא הדרגתיות. עם מערכת ליבה גדולה — זה סיכון קריטי שרוב ארגונים לא יכולים להרשות לעצמם.

**מדוע לא Micro-Frontend?**
Micro-frontend מוסיף מורכבות תשתיתית משמעותית (Module Federation, shared state, routing בין apps) שמתאימה יותר כשכבר יש ריבוי צוותים עצמאיים ותחומי דומיין ברורים. לארגון בגודל זה — over-engineering.

**תוכנית ה-ngUpgrade:**

1. **ספרינט 1-2: תשתית** — הגדרת Angular CLI לצד AngularJS, הוספת `@angular/upgrade`, הפעלת `UpgradeModule`, וודא שה-build עובד. אין שינוי פונקציונלי.

2. **ספרינטים 3-N: מיגרציה מ-Leaf ל-Root** — מתחילים מה-Components הכי קטנים ועלים (שאין להם dependencies על AngularJS אחרים), ממירים אותם ל-Angular component, ומשתמשים ב-`downgradeComponent()` כדי לשמור שהם עובדים בתוך ה-AngularJS shell. במקביל, ממשיכים לפתח פיצ'רים עסקיים חדשים — **ישירות ב-Angular**.

3. **Services לפני Components** — ממירים shared services ל-Angular Providers ומשתמשים ב-`downgradeInjectable()`. זה הכי קריטי לפני שמסירים את AngularJS.

4. **הסרת AngularJS** — רק לאחר שה-root component הוסב ל-Angular ואין `ng-app` יותר — מסירים את `UpgradeModule` ואת AngularJS לחלוטין.

**אמצעי בטיחות:** E2E tests עם Cypress מכסים את הפיצ'רים הקריטיים לפני כל ספרינט; Feature Flags מאפשרים rollback מהיר בכל שלב.

---

### שאלה 3: 3 Best Practices ל-API בעומס גבוה ב-C#

**1. Caching מרובד**

נתונים שנקראים הרבה ואינם משתנים לעיתים קרובות (כמו רשימת פנימיות) לא צריכים ללכת ל-DB בכל בקשה. אשתמש ב-`IMemoryCache` לנתונים קטנים ב-RAM, וב-**Redis** (דרך `IDistributedCache`) לכיסוי סביבת multi-instance/load-balancing. בנוסף, `[ResponseCache]` attribute ב-controller מוסיף `Cache-Control` headers ומאפשר ל-CDN / reverse proxy לקאשש GET responses ולהפחית עומס על ה-API לחלוטין.

**2. Async/Await עד הסוף + Connection Pooling**

כל Thread שחסום על I/O הוא Thread שלא יכול לשרת בקשה אחרת. יש להשתמש ב-`async/await` בכל שכבה — Controllers, Services, Repositories — ולהגיע ל-non-blocking I/O עד לרמת ה-`SqlConnection`. Dapper תומך ב-`QueryAsync` / `ExecuteAsync` מקוריות. ה-`SqlConnection` pool של .NET כבר מנוהל אוטומטית אך יש לוודא ש-`using` (או `await using`) נסגר כראוי כדי להחזיר connections ל-pool. יש להימנע מ-`Task.Result` / `.Wait()` שגורמים ל-thread starvation.

**3. Rate Limiting + Circuit Breakers + Health Checks**

בASP.NET Core 7+ יש Rate Limiting middleware מובנה — אשתמש בו כדי להגביל מספר בקשות per IP / per user ולמנוע abuse ו-accidental DDoS. לשירותים חיצוניים (DB, third-party APIs) אשתמש ב-**Polly** למימוש Circuit Breaker pattern — שירות שנכשל חוזרות לא יציף ה-app בחריגות; אחרי N כשלים, ה-circuit נפתח ומחזיר תשובה מהירה (fail-fast) עד שהשירות מתאושש. לבסוף, Health Checks (`/health`, `/ready`) מאפשרים ל-load balancer להסיר instance כושל מהרוטציה אוטומטית.

---

## חלק ג' — שאלת בונוס (שאלה 5)

### שאלה 5: מעבר לענן GCP + CI/CD

**אסטרטגיית CI/CD**

אטמיע Pipeline ב-**GitLab CI/CD** (או **Cloud Build** אם כבר עובדים עם GCP) עם 4 שלבים:

1. **Build & Test** — `dotnet build` + `dotnet test` + Static Code Analysis (SonarQube / dotnet-format). כל PR חייב לעבור שלב זה לפני merge.
2. **Docker Build & Push** — בניית Docker image ודחיפה ל-**Google Artifact Registry** עם tag לפי commit SHA.
3. **Deploy to Staging** — עדכון ה-image ב-**Cloud Run** (או GKE) של סביבת staging אוטומטית. הרצת Smoke Tests ו-E2E Tests מול staging.
4. **Deploy to Production** — רק מה-main/release branch, דורש אישור ידני (Manual Gate) + Canary deployment.

כל Infrastructure מוגדר ב-**Terraform** — אין שינויים ידניים בענן. כל שינוי תשתיתי עובר code review כמו קוד רגיל.

**אסטרטגיית הפריסה — Canary + Blue-Green**

להבטחת zero-downtime:

- **Cloud SQL** — המעבר ל-Managed Cloud SQL מתחיל עם **Database Migration Service** של GCP — מגדירים CDC (Change Data Capture) replication מ-SQL Server On-Premise ל-Cloud SQL בזמן אמת. רק לאחר שה-lag מגיע ל-0 — עושים cutover ב-maintenance window קצר.
- **Canary Deployment** — ב-Cloud Run ניתן להגדיר traffic splitting: 5% מהתנועה הולכת ל-revision החדש, 95% לישן. אם ה-error rate לא עולה ב-15 דקות — מעבירים 100%. אם יש בעיה — rollback אוטומטי ב-2 שניות.
- **Blue-Green** — לגרסאות גדולות עם DB migrations: מקימים סביבה ירוקה מלאה (תשתית + קוד חדש), מחממים אותה, ומחליפים את ה-Load Balancer בבת-אחת. הסביבה הכחולה נשארת live עוד 24 שעות כ-fallback.

**אסטרטגיית המעבר לענן בלי השבתה:**

1. הקמת Cloud SQL עם replication רציף מ-On-Premise.
2. Deploy של ה-API ב-Cloud Run **עם connection string שמצביע עדיין ל-On-Premise DB**.
3. בדיקות ב-Production traffic (Canary 5%) מול Cloud Run → On-Premise DB.
4. Cutover ה-DB (Cloud SQL) ב-maintenance window — שינוי connection string דרך Secret Manager.
5. הפניית כל ה-DNS ל-Cloud Run. ביטול On-Premise gracefully.

כך בכל שלב יש נקודת rollback ברורה ואין השבתה מתוכננת של המערכת למשתמשי הקצה.
