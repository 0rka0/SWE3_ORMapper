using NUnit.Framework;
using SWE3_ORM_App.ModelClasses;
using SWE3_ORM_Framework.Caching;
using System;

namespace SWE3_ORM_Test
{
    /// <summary>
    /// Tests the functionality of the cache.
    /// </summary>
    public class CacheTests
    {
        ICache cache;
        Teacher teacher;
        Teacher preT;

        /// <summary>
        /// Creates a cache and test object for the tests.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            cache = new Cache();
            cache.CacheObject(preT = new Teacher() { Id = "t.0" });
            cache.AddTmp(preT);
            teacher = new Teacher()
            {
                Id = "t.1",
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
        public void GetObject_ObjectDoesNotExistInCache_ReturnsNull()
        {
            var actualValue = cache.GetObject(teacher.Id);

            Assert.IsNull(actualValue);
        }

        /// <summary>
        /// Cached Object is selected from the cache by its primary key.
        /// Test result will be positive if the values of the selected object match the original object.
        /// </summary>
        [Test]
        public void GetObject_ObjectExistsInCache_ReturnsObject()
        {
            var actualValue = cache.GetObject(preT.Id);

            Assert.AreEqual(preT.Id, ((Teacher)actualValue).Id);
        }

        [Test]
        public void CacheObject_GetsObject_PutsObjectIntoCache()
        {
            var actualValue = cache.GetObject(teacher.Id);
            Assert.IsNull(actualValue);

            cache.CacheObject(teacher);
            actualValue = cache.GetObject(teacher.Id);

            Assert.AreEqual(teacher.Id, ((Teacher)actualValue).Id);
        }

        [Test]
        public void HasChanged_ChecksIfObjectHasChanged_ReturnsChangeStatus()
        {
            cache.CacheObject(teacher);
            Assert.IsFalse(cache.CacheChanged(teacher));

            teacher.Salary = 45000;
            Assert.IsTrue(cache.CacheChanged(teacher));

            cache.CacheObject(teacher);
            Assert.IsFalse(cache.CacheChanged(teacher));
        }

        [Test]
        public void RemoveObject_RemovesObjectFromCache_ObjectIsNoLongerInCache()
        {
            var actualValue = cache.GetObject(preT.Id);
            Assert.IsNotNull(actualValue);

            cache.RemoveObject(preT);

            actualValue = cache.GetObject(preT.Id);
            Assert.IsNull(actualValue);
        }

        [Test]
        public void ContainsKey_ChecksCacheForPrimaryKey_ReturnsIfPrimaryKeyIsInCache()
        {
            var actualValue = cache.ContainsKey(teacher.Id);
            Assert.IsFalse(actualValue);

            actualValue = cache.ContainsKey(preT.Id);
            Assert.IsTrue(actualValue);
        }

        [Test]
        public void SeachTmp_ObjectDoesExistInTmpCache_ReturnsObject()
        {
            var actualValue = cache.SearchTmp(preT.Id);
            Assert.AreEqual(preT.Id, ((Teacher)actualValue).Id);
        }

        [Test]
        public void SeachTmp_ObjectDoesNotExistInTmpCache_ReturnsNull()
        {
            var actualValue = cache.SearchTmp(teacher.Id);
            Assert.IsNull(actualValue);
        }

        [Test]
        public void AddTmp_AddsObjectToTemporaryCache_ObjectAddedToCache()
        {
            var actualValue = cache.SearchTmp(teacher.Id);
            Assert.IsNull(actualValue);

            cache.AddTmp(teacher);

            actualValue = cache.SearchTmp(teacher.Id);
            Assert.AreEqual(teacher.Id, ((Teacher)actualValue).Id);
        }

        [Test]
        public void ClearTmp_RemovesAllObjectsFromTemporaryCache_TmpCacheIsEmpty()
        {
            cache.AddTmp(teacher);

            cache.ClearTmp();

            var actualValue = cache.SearchTmp(preT.Id);
            Assert.IsNull(actualValue);
            actualValue = cache.SearchTmp(teacher.Id);
            Assert.IsNull(actualValue);
        }
    }
}