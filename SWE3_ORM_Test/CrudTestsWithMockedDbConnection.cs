using NUnit.Framework;
using Moq;
using System.Data;
using SWE3_ORM_Framework;
using SWE3_ORM_App.ModelClasses;
using System;

namespace SWE3_ORM_Test
{
    /// <summary>
    /// Tests the general CRUD functionality of the ORMapper with a Mock Setup that simulates as if the database works as intended.
    /// </summary>
    public class CrudTestsWithMockedDbConnection
    {
        Mock<IDbConnection> connMock;
        Mock<IDbCommand> cmdMock;
        Mock<MockDataReader> rdMock;
        Mock<IDbDataParameter> paramMock;

        Teacher testObject = new Teacher();

        /// <summary>
        /// Creates Mock Setup and test object for the correspong tests.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            testObject.Id = "t.0";
            testObject.Name = "Fritz";
            testObject.FirstName = "Ferdinand";
            testObject.BDate = new DateTime(1990, 8, 26);
            testObject.HDate = new DateTime(2020, 5, 4);
            testObject.Salary = 50000;
            testObject.Gender = Gender.Male;
            testObject.Classes = new System.Collections.Generic.List<Class>();


            rdMock = new Mock<MockDataReader>();
            rdMock.Object.Fill();

            paramMock = new Mock<IDbDataParameter>();

            cmdMock = new Mock<IDbCommand>();
            cmdMock.Setup(c => c.ExecuteReader()).Returns(rdMock.Object);
            cmdMock.Setup(c => c.ExecuteNonQuery());
            cmdMock.Setup(c => c.CreateParameter()).Returns(paramMock.Object);
            cmdMock.Setup(c => c.Parameters.Add(cmdMock.Object.CreateParameter()));

            connMock = new Mock<IDbConnection>();
            connMock.Setup(c => c.Open());
            connMock.Setup(c => c.CreateCommand()).Returns(cmdMock.Object);

            ORMapper.StartConnection(connMock.Object);
        }

        /// <summary>
        /// Selects entry from MockDatabase by its primary key and handles the data correspondingly.
        /// searchId has to be defined to replace the SQL that would define the primary key.
        /// Test result will be positive if the Get method does return the correct object.
        /// </summary>
        [Test]
        public void Get_SelectsEntryFromDatabaseByPK_ReturnsEntry()
        {
            var expectedId = "t.0";
            rdMock.Object.searchId = expectedId;
            var rObj = (Teacher)ORMapper.GetByPK(expectedId, typeof(Teacher));

            Assert.IsNotNull(rObj);
            Assert.AreEqual(expectedId, rObj.Id);
        }

        /// <summary>
        /// Cannot select entry from MockDatabase because primary key does not exist.
        /// searchId has to be defined to replace the SQL that would define the primary key.
        /// Test result will be positive if the Get method does return null.
        /// </summary>
        [Test]
        public void Get_SelectsEntryFromDatabaseByNonExistingPK_ReturnsNull()
        {
            var expectedId = "t.2";
            rdMock.Object.searchId = expectedId;
            var rObj = (Teacher)ORMapper.GetByPK(expectedId, typeof(Teacher));

            Assert.IsNull(rObj);
        }

        /// <summary>
        /// Transforms DataReader Values To Dictionary by using the keys and values of the reader.
        /// Test result will be positive if the Dictionary data represents the data of the reader.
        /// </summary>
        [Test]
        public void TransformReader()
        {
            var expectedId = "t.0";
            rdMock.Object.searchId = expectedId;
            rdMock.Object.Read();
            var actualValue = ORMapper.TransformReader(rdMock.Object);

            Assert.AreEqual(expectedId, actualValue["id"]);
        }

        /// <summary>
        /// Inserts object into the database. Database operation ExecuteNonQuery is mocked so it will get skipped during execution.
        /// Test if everything else is handled correspondingly and does not lead to any errors.
        /// Test result will be positive if the Create method does run without exception.
        /// </summary>
        [Test]
        public void Create_InsertsObjectIntoDb_RunsSuccessfully()
        {
            ORMapper.Create(testObject);

            Assert.IsTrue(true);
        }

        /// <summary>
        /// Updates object in the database. Database operation ExecuteNonQuery is mocked so it will get skipped during execution.
        /// Test if everything else is handled correspondingly and does not lead to any errors.
        /// Test result will be positive if the Update method does run without exception.
        /// </summary>
        [Test]
        public void Update_ChangesObjectInDb_RunsSuccessfully()
        {
            ORMapper.Update(testObject);

            Assert.IsTrue(true);
        }

        /// <summary>
        /// Removes object from the database. Database operation ExecuteNonQuery is mocked so it will get skipped during execution.
        /// Test if everything else is handled correspondingly and does not lead to any errors.
        /// Test result will be positive if the Remove method does run without exception.
        /// </summary>
        [Test]
        public void Remove_RemovesObjectFromDb_RunsSuccessfully()
        {
            ORMapper.Update(testObject);

            Assert.IsTrue(true);
        }
    }
}