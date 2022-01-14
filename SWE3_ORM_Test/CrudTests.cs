using NUnit.Framework;
using Moq;
using System.Data;
using SWE3_ORM_Framework;
using SWE3_ORM_App.ModelClasses;
using System;

namespace SWE3_ORM_Test
{
    public class Tests
    {
        Mock<IDbConnection> connMock;
        Mock<IDbCommand> cmdMock;
        Mock<MockDataReader> rdMock;
        Mock<IDbDataParameter> paramMock;

        Teacher testObject = new Teacher();

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

        [Test]
        public void Get_SelectsEntryFromDatabaseByPK_ReturnsEntry()
        {
            var expectedId = "t.0";
            rdMock.Object.searchId = expectedId;
            var rObj = (Teacher)ORMapper.Get(expectedId, typeof(Teacher));

            Assert.IsNotNull(rObj);
            Assert.AreEqual(expectedId, rObj.Id);
        }

        [Test]
        public void Get_SelectsEntryFromDatabaseByNonExistingPK_ReturnsNull()
        {
            var expectedId = "t.2";
            rdMock.Object.searchId = expectedId;
            var rObj = (Teacher)ORMapper.Get(expectedId, typeof(Teacher));

            Assert.IsNull(rObj);
        }

        [Test]
        public void Create_InsertsObjectIntoDb_RunsSuccessfully()
        {
            ORMapper.Create(testObject);

            Assert.IsTrue(true);
        }
    }
}