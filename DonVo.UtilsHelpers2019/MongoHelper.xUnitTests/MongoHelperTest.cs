﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MongoHelper.xUnitTests
{
    public class MongoHelperTest : IClassFixture<MongoFixture>
    {
        private readonly MongoHelper _mongo;
        private readonly string _collection;
        private readonly ITestOutputHelper _testOutputHelper;

        public MongoHelperTest(MongoFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _mongo = fixture.Mongo;
            _collection = fixture.Collection;
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task QueryCountTestAsync()
        {
            var total = await _mongo.QueryCountAsync<Person>(_collection);
            Assert.True(total > 0);

            var adult = await _mongo.QueryCountAsync<Person>(_collection, p => p.Age >= 18);
            Assert.True(adult <= total);
        }

        [Fact]
        public async Task InsertTypeTestAsync()
        {
            var inserting = await _mongo.QueryCountAsync<Person>(_collection);
            await _mongo.InsertAsync<Person>(_collection, new Person("Colin", 8), new Person("Robin", 9));
            var inserted = await _mongo.QueryCountAsync<Person>(_collection);

            Assert.Equal(inserted, inserting + 2);
        }

        [Fact]
        public async Task DeleteTestAsync()
        {
            var deleting = await _mongo.QueryCountAsync<Person>(_collection);
            await _mongo.DeleteAsync<Person>(_collection, p => p.Name == "Colin");
            var deleted = await _mongo.QueryCountAsync<Person>(_collection);
            Assert.True(deleted < deleting);
        }

        [Fact]
        public async Task UpdateTestAsync()
        {
            var updates = new List<UpdateCondition<Person, object>>
            {
                new UpdateCondition<Person, object>(p => p.Name, "Tomas"),
                new UpdateCondition<Person, object>(p => p.Age, 40)
            };
            await _mongo.UpdateAsync(_collection, updates, p => p.Name == "Tom");


            var tom = await _mongo.QueryCountAsync<Person>(_collection, p => p.Name == "Tom");
            Assert.True(tom <= 0);
        }

        [Fact]
        public async Task QueryTestAsync()
        {
            var persons = await _mongo.QueryAsync<Person>(_collection, p => p.Age > 18);
            Assert.DoesNotContain(persons, p => p.Age <= 18);
        }

        [Fact]
        public async Task QueryPagesTestAsync()
        {
            var persons = await _mongo.QueryAsync<Person>(_collection, p => p.Age > 18, 2, 3);

            Assert.DoesNotContain(persons, p => p.Age <= 18);
            Assert.True(persons.Count() <= 3);
        }

        [Fact]
        public async Task QuerySortTestAsync()
        {
            var scs = new List<SortCondition<Person>>
            {
                new SortCondition<Person>(p => p.Age),
                new SortCondition<Person>(p => p.Name, SortDirection.Descending)
            };
            var persons = await _mongo.QueryAsync<Person>("persons", sortConditions: scs);

            if (persons.Count() > 2)
                Assert.True(persons.First().Age <= persons.ElementAt(1).Age);

            foreach (var person in persons)
                _testOutputHelper.WriteLine(person.ToString());
        }

        [Fact]
        public async Task QueryBigDataTestAsync()
        {
            using (var cursor = await _mongo.QueryBigDataAsync<Person>(_collection))
                while (cursor.MoveNext())
                    foreach (var person in cursor.Current)
                        _testOutputHelper.WriteLine(person.ToString());
        }

        [Fact]
        public async Task CreateIndexTest()
        {
            await _mongo.CreateOneIndexAsync("persons", new Dictionary<string, SortDirection>
            {
                ["ClassId"] = SortDirection.Ascending,
                ["CameraId"] = SortDirection.Ascending
            });

            await _mongo.CreateManyIndexAsync(_collection, new Dictionary<string, SortDirection>
            {
                ["FaceId"] = SortDirection.Ascending,
                ["UserName"] = SortDirection.Ascending
            });
        }
    }
}
