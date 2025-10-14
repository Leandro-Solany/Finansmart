using Finansmart.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Finansmart.Tests.IntegrationTests
{
    public abstract class BaseIntegrationTest : IDisposable
    {
        protected readonly DatabaseContext Context;

        protected BaseIntegrationTest()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            Context = new DatabaseContext(options);
            SeedDatabase();
        }

        protected virtual void SeedDatabase()
        {
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
