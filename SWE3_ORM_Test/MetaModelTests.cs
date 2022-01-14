using NUnit.Framework;
using Moq;
using System.Data;
using SWE3_ORM_Framework;
using SWE3_ORM_App.ModelClasses;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Creates a Column object that will be used in the tests.
        /// A person table will be used.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            table = new Table(typeof(Person));
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

        [Test]
        public void GetObjectValue_GetsValueOfTheSpecificColumnFromTable_ReturnsValue()
        {
            var columnValue = table.TableCols[0].GetObjectValue(teacher);

            Assert.AreEqual("t.0", columnValue);

            columnValue = table.TableCols[3].GetObjectValue(teacher);

            Assert.AreEqual(Gender.Male, columnValue);
        }

        [Test]
        public void SetObjectValue_SetsValueForTheSpecificColumnAtTable_ChangesValue()
        {
            string expectedValue = "t.1";
            table.TableCols[0].SetObjectValue(teacher, expectedValue);

            Assert.AreEqual(expectedValue, table.TableCols[0].GetObjectValue(teacher));
        }

        [Test]
        public void ToColumnType_ChangesTypeToFitDb_TypeChanged()
        {
            //WIP
            var type = table.TableCols[3].ToColumnType(true);
        }
    }
}