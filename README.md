# SWE3_ORMapper

Kurztutorial im Falle von Unklarheiten: 
(Ausführlichere Dokumentation ist im Code bei den einzelnen Methoden zu finden)

Der ORMapper erstellt die Tabellen mittels der CreateTable() Methode für alle gewünschten Klassen. 
Vererbung wird in einer einzelnen Tabelle dargestellt und hier ist es unwichtig welche dieser Klassen angegeben wird, der ORMapper erstellt eine Tabelle die alle Klassen fässt.
Nachdem alle Tabellen erstellt wurden ist es hier notwendig im Anschluss noch die AddRelationshipConstrains() Methode mit allen gewünschten Klassen aufzurufen, sodass foreign key constraints und Zwischentabellen vom Mapper generiert werden.
Siehe: Showcase.CreateTables()

Alternativ kann die Datenbank auch manuell mit den Ausdrücken im mitgelieferten File "database_scripts.txt" erstellt werden.

Daten können in die passende Tabelle in der Datenbank gespeichert werden, falls diese bereits erstellt wurde (automatisch oder manuell). Dafür ist die Create() Methode zuständig und ihre muss das gewünschte Objekt als Parameter übergeben werden.
siehe z.B.: Showcase.CreateTeacher()

Daten können aktualisiert werden, falls ein Object mit dem selben Primary key bereits in der Datenbank existiert. Dafür ruft man die Update() Methode mit dem gewünschten Object als Parameter auf.
siehe z.B.: Showcase.AddStudentsToCourses()

Falls das Objekt in der Datenbank existiert kann es mittels der Remove() Methode auch wieder entfernt werden.
siehe z.B.: Showcase.RemoveCourse()

Objekte können auf drei verschiedene Arten aus der Datenbank ausgelesen werden:
- Durch den Primary Key: Man ruft die GetByPK() Methode mit dem gewünschten primary key und dem Typen der Klasse der das Objekt angehört auf. 
siehe z.B.: Showcase.SelectTeacherWithClasses()
- Durch Parameter: Man ruft die GetByParams<T>() Methode mit T als der Klasse des Objektes und einer Liste an Parameter Tuples auf. Diese haben die Form (Spaltenname, Wert). Außerdem kann man optional den Operator flag auf true oder false setzen, sodass die Parameter durch AND oder OR getrennt werden. Standardmäßig wird hier AND verwendet.
siehe z.B.: Showcase.SelectStudentsByParamters()
- Durch SQL: Man ruft die GetBySql<T>() Methode mit T als der Klasse des Objektes und einem Dictionary an Parametern auf. Ein Eintrag im Dictionary hat die Form (Referenz im Sql, Wert).
siehe z.B.: Showcase.SelectTeacherBySql()

Um sicherzugehen, dass Ergebnisse immer up to date sind und sich im Cache nicht veraltete Objekte herumschwirren ist zu empfehlen nach Änderungen die Update() Methode für alle betroffenen Objekte zu verwenden. Dadurch werden die Änderungen getrackter Objekte auch im Cache angewandt. 
Alternativ kann man den Cache vor dem Auslesen nach Veränderungen mit der ClearCache() Methode zu leeren.
siehe z.B.: Showcase.SelectTeacherWithClasses()

-- Testing --
Unit tests laufen unabhängig von der Datenbank, das heißt Methoden die rein Modifizierung der Datenbank durchführen werden nicht von den Unit tests abgedeckt. Unit Tests testen hauptsächlich Funktionalitäten die unabhängig von der Datenbank funktionieren. CRUD Funktionalitäten werden mit einer Mock Datenbank getestet und testen ob sie funktionieren, falls sich die Datenbank wie erwartet verhält.