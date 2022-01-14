using NUnit.Framework;
using Moq;
using System.Data;
using SWE3_ORM_Framework;
using SWE3_ORM_App.ModelClasses;
using System;
using System.Collections.Generic;

namespace SWE3_ORM_Test
{
    /// <summary>
    /// Tests the help methods of the ORMapper.
    /// Does not test methods that exclusivly deal with database access, because there is no proper DB connection in these unit tests.
    /// </summary>
    public class ORMHelpMethodsTests
    {
        /// <summary>
        /// Creates a Mock Setup so that database methods can be skipped if necessary.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Mock<IDataReader> rdMock = new Mock<IDataReader>();
            Mock<IDbDataParameter> paramMock = new Mock<IDbDataParameter>();
            Mock<IDbCommand> cmdMock = new Mock<IDbCommand>();
            cmdMock.Setup(c => c.ExecuteReader()).Returns(rdMock.Object);
            cmdMock.Setup(c => c.CreateParameter()).Returns(paramMock.Object);
            cmdMock.Setup(c => c.Parameters.Add(cmdMock.Object.CreateParameter()));
            Mock<IDbConnection> connMock = new Mock<IDbConnection>();
            connMock.Setup(c => c.Open());
            connMock.Setup(c => c.CreateCommand()).Returns(cmdMock.Object);
            ORMapper.StartConnection(connMock.Object);
        }

        /// <summary>
        /// Method gets a Type an returns all of its subtypes.
        /// Test result will be positive if the returned array contains all subtypes of the parameter type.
        /// </summary>
        [Test]
        public void GetSubtypes_GetsType_ReturnsArrayOfAllSubtypes()
        {
            var subTypes = ORMapper.GetSubtypes(typeof(Person));

            Assert.AreEqual("Student", subTypes[0].Name);
            Assert.AreEqual("Teacher", subTypes[1].Name);
        }

        /// <summary>
        /// Method builds sql to select with all corresponding discriminators. 
        /// Test result will be positive if the returned sql does contain all subclasses as a discriminator.
        /// </summary>
        [Test]
        public void GetDiscriminatorSql_GetsType_ReturnsSqlWithDiscriminatorsOfAllSubclasses()
        {
            string expectedSql = "(Discriminator = 'Person' OR Discriminator = 'Student' OR Discriminator = 'Teacher') AND ";
            string actualSql = ORMapper.GetDiscriminatorSql(typeof(Person));

            Assert.AreEqual(expectedSql, actualSql);
        }

        /// <summary>
        /// Method creates object consisting of all keys and values in the database.
        /// Test result will be positive if the returned object has the same type as the original object and the correct values.
        /// </summary>
        [Test]
        public void CreateObject_GetsTypeAndDictionaryWithData_ReturnsObjectWithFullValues()
        {
            Teacher expectedTeacher = new Teacher()
            {
                Id = "t.0",
                Name = "Fritz",
                FirstName = "Ferdinand",
                Gender = Gender.Male,
                BDate = new DateTime(1990, 08, 26, 0, 0, 0),
                HDate = new DateTime(2020, 05, 04, 0, 0, 0),
                Salary = 50000
            };

            var actualTeacher = (Teacher)ORMapper.CreateObject(typeof(Teacher),
                new Dictionary<string, object>()
                {
                    { "id", expectedTeacher.Id },
                    { "discriminator", nameof(Teacher) },
                    { "name", expectedTeacher.Name },
                    { "firstname", expectedTeacher.FirstName },
                    { "gender", expectedTeacher.Gender },
                    { "bdate", expectedTeacher.BDate },
                    { "hdate", expectedTeacher.HDate },
                    { "salary", expectedTeacher.Salary },
                    { "kclass", null },
                    { "grade", null }
                });

            Assert.AreEqual(expectedTeacher.Id, actualTeacher.Id);
            Assert.AreEqual(expectedTeacher.Name, actualTeacher.Name);
            Assert.AreEqual(expectedTeacher.GetType(), actualTeacher.GetType());
            Assert.AreEqual(expectedTeacher.HDate, actualTeacher.HDate);
        }

        /// <summary>
        /// Method gets the correct Table for the class.
        /// Test result will be positive if the returned object has the expected values that fit the type.
        /// </summary>
        [Test]
        public void GetTable_CreatesTableObjectForTypeParameter_ReturnsTableObject()
        {
            var expectedName = "persons";
            var expectedDisc = "Teacher";
            var table = ORMapper.GetTable(typeof(Teacher));

            Assert.AreEqual(expectedName, table.Name);
            Assert.AreEqual(expectedDisc, table.Discriminator);
            Assert.AreEqual(typeof(Teacher), table.Member);
        }
    }
}