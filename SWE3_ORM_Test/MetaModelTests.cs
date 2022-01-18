using NUnit.Framework;
using SWE3_ORM_Framework;
using SWE3_ORM_App.ModelClasses;
using System;
using SWE3_ORM_Framework.MetaModel;
using SWE3_ORM_Framework.Caching;

namespace SWE3_ORM_Test
{
    /// <summary>
    /// Tests the functionality of the meta model.
    /// </summary>
    public class MetaModelTests
    {
        public Table table;
        public Teacher teacher;
        public Class c;

        /// <summary>
        /// Creates a Column object that will be used in the tests.
        /// A person table will be used.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            table = new Table(typeof(Teacher));
            teacher = new Teacher()
            {
                Id = "t.0",
                Name = "Fritz",
                FirstName = "Ferdinand",
                Gender = Gender.Male,
                BDate = new DateTime(1990, 08, 26, 0, 0, 0),
                HDate = new DateTime(2020, 05, 04, 0, 0, 0),
                Salary = 50000
            };
            c = new Class()
            {
                Id = "c.0",
                Name = "class 1",
                kTeacher = teacher
            };
        }

        /// <summary>
        /// Constructor of the Table class builds the object.
        /// Test result will be positive if the created object consists of the correct values for the type.
        /// </summary>
        [Test]
        public void Table_CallsConstructorOfTableClassWithSpecificType_CreatesTableObject()
        {
            var expectedName = "persons";
            var expectedDisc = "Person";
            var table = new Table(typeof(Person));

            Assert.AreEqual(expectedName, table.Name);
            Assert.AreEqual(expectedDisc, table.Discriminator);
            Assert.AreEqual(typeof(Person), table.Member);
        }

        /// <summary>
        /// Values will be retrieved for specific column from object.
        /// Test result will be positive if the retrieved values are correct.
        /// </summary>
        [Test]
        public void GetObjectValue_GetsValueOfTheSpecificColumnFromTable_ReturnsValues()
        {
            var columnValue = table.TableCols[2].GetObjectValue(teacher);

            Assert.AreEqual("t.0", columnValue);

            columnValue = table.TableCols[5].GetObjectValue(teacher);

            Assert.AreEqual(Gender.Male, columnValue);
        }

        /// <summary>
        /// Sets value for specific column in object.
        /// Test result will be positive if the object has the correct value after the change.
        /// </summary>
        [Test]
        public void SetObjectValue_SetsValueForTheSpecificColumnAtTable_ChangesValues()
        {
            string expectedValueId = "t.1";
            table.TableCols[2].SetObjectValue(teacher, expectedValueId);

            Assert.AreEqual(expectedValueId, table.TableCols[2].GetObjectValue(teacher));

            int expectedValueSal = 45000;
            table.TableCols[1].SetObjectValue(teacher, expectedValueSal);

            Assert.AreEqual(expectedValueSal, table.TableCols[1].GetObjectValue(teacher));
        }

        /// <summary>
        /// Types will be adjusted to fit the database.
        /// In this case enum will be persisted as integer.
        /// Test result will be positive if the transformed type and value are correct.
        /// </summary>
        [Test]
        public void ToColumnType_ChangesTypeToFitDb_EnumChangedToInt()
        {
            var actualValue = table.TableCols[5].ToColumnType(table.TableCols[5].GetObjectValue(teacher));

            Assert.AreEqual(typeof(int), actualValue.GetType());
            Assert.AreEqual((int)teacher.Gender, actualValue);
        }

        /// <summary>
        /// Types will be adjusted to fit the database. 
        /// In this case a referenced object will be persisted by their primary key.
        /// Test result will be positive if the transformed type and value are correct.
        /// </summary>
        [Test]
        public void ToColumnType_ChangesTypeToFitDb_ReferenceChangedToPk()
        {
            var classTable = ORMapper.GetTable(c);
            var actualValue = classTable.TableCols[2].ToColumnType(c.kTeacher);

            Assert.AreEqual(typeof(string), actualValue.GetType());
            Assert.AreEqual(c.kTeacher.Id, actualValue);
        }

        /// <summary>
        /// Types will be adjusted to fit the code. 
        /// In this case a enum will be restored from integer.
        /// Test result will be positive if the transformed type and value are correct.
        /// </summary>
        [Test]
        public void ToCodeType_ChangesDbTypeToFitCode_IntChangedToEnum()
        {
            var actualValue = table.TableCols[5].ToCodeType(0, new Cache());

            Assert.IsTrue(actualValue.GetType().IsEnum);
            Assert.AreEqual(teacher.Gender, actualValue);
        }

        /// <summary>
        /// Sql string will be created to select foreign key from corresponding table
        /// Test result will be positive if the created sql is as expected.
        /// </summary>
        [Test]
        public void GetReferenceSql_CreatesSqlToSelectReferences_ReturnsCorrectSql()
        {
            var actualValue = table.ReferencedCols[0].GetReferenceSql(typeof(Teacher));

            Assert.AreEqual("SELECT * FROM classes WHERE kteacher = :fk", actualValue);
        }
    }
}