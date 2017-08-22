﻿using System;
using Autofac;
using Autofac.Builder;
using Isidore.MagicMirror.DAL.MongoDB.Configuration;
using Isidore.MagicMirror.ImageProcessing.FaceRecognition.Services;
using Isidore.MagicMirror.Infrastructure.Exceptions;
using Isidore.MagicMirror.Infrastructure.Extensions;
using Isidore.MagicMirror.Infrastructure.Validation;
using Isidore.MagicMirror.Users.API.Configuration;
using Isidore.MagicMirror.Users.API.Validators;
using Isidore.MagicMirror.Users.Models;
using Isidore.MagicMirror.Users.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ProjectOxford.Face;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Isidore.MagicMirror.Users.API.Modules
{
    public class UsersModule : Module
    {
        private readonly IConfiguration _appConfig;
        private readonly ILoggerFactory _loggerFactory;

        public UsersModule(IConfiguration appConfig, ILoggerFactory loggerFactory)
        {
            _appConfig = appConfig;
            _loggerFactory = loggerFactory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var faceServiceConfig = _appConfig.GetSettings<FaceServiceConfig>();
            var faceRecognitionService =
                new FaceServiceClient(faceServiceConfig.AccessKey, faceServiceConfig.ServiceUrl);
            builder.RegisterInstance(faceRecognitionService).As<IFaceServiceClient>();
            builder.RegisterInstance(_loggerFactory).As<ILoggerFactory>();
            builder.RegisterType<AzureFaceRecognitionService>()
                .AsImplementedInterfaces();
            builder.RegisterInstance(SetUpMongoDb());
            builder.RegisterType<AzureUserService>().AsImplementedInterfaces();
            builder.RegisterInstance(this._appConfig.Get<FaceServiceConfig>());
            builder.RegisterInstance(GetValidatorFactory()).SingleInstance();
            builder.RegisterType<AzureUserGroupService>().AsImplementedInterfaces();
        }

        private ValidatorsFactory GetValidatorFactory()
        {
            var factory = new ValidatorsFactory();
            factory.RegisterValidator<User>(new UserFirstNameValidator());
            factory.RegisterValidator<User>(new LastNameValidator());

            return factory;
        }

        private IMongoDatabase SetUpMongoDb()
        {
            IMongoDatabase mongoDb;
            var config = _appConfig.GetSettings<MongoDbConfig>();
            var credential = MongoCredential.CreateCredential(config.DbName, config.Username, config.Password);
            try
            {
                mongoDb = new MongoClient(new MongoClientSettings
                {
                    Servers = new[] { new MongoServerAddress(config.ServerUrl, config.Port ?? 27017) },
                    ConnectTimeout = TimeSpan.FromSeconds(5),
                    Credentials = new[] { credential },
                    UseSsl = config.UseSsl
                }).GetDatabase(config.DbName);
                mongoDb.RunCommandAsync((Command<BsonDocument>)"{ping:1}")
                    .Wait();
            }
            catch (Exception e)
            {
                throw new DependentComponentException(ComponentType.MongoDb, e, $"Database name: {config.DbName}");
            }

            return mongoDb;
        }
    }
}