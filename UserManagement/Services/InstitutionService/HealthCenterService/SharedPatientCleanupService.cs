using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.Models;
using UserManagement.Services.FileServices;
using UserManagement.Services.UserServices;
using UserManagement.Utils;

namespace UserManagement.Services.InstitutionService.HealthCenterService
{
    public class SharedPatientCleanupService : BackgroundService
    {
        private readonly IMongoCollection<HealthCenter> _healthCenterCollection;
        public SharedPatientCleanupService(IOptions<MongoDBSettings> options)
        {
            _healthCenterCollection = new MongoClient(options.Value.ConnectionUrl).GetDatabase(options.Value.DBName)
                                                                                  .GetCollection<HealthCenter>("HealthCenters");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var filter = Builders<HealthCenter>.Filter.ElemMatch(hc => hc.SharedPatients, sp => sp.ExpirationTime <= DateTime.UtcNow);
                var update = Builders<HealthCenter>.Update.PullFilter(hc => hc.SharedPatients, sp => sp.ExpirationTime <= DateTime.UtcNow);
                _ = await _healthCenterCollection.UpdateManyAsync(filter, update, cancellationToken: stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);  // Run every minute
            }
        }
    }
}