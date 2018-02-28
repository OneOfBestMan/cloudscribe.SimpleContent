﻿// Copyright (c) Source Tree Solutions, LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Author:					Joe Audette
// Created:					2016-09-02
// Last Modified:			2018-02-28
// 

using cloudscribe.SimpleContent.Models;
using cloudscribe.SimpleContent.Storage.EFCore.Common;
using cloudscribe.SimpleContent.Storage.EFCore.MSSQL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SimpleContentEFMSSQLServiceCollectionExtensions
    {

        public static IServiceCollection AddCloudscribeSimpleContentEFStorageMSSQL(
            this IServiceCollection services,
            string connectionString,
            int maxConnectionRetryCount = 0,
            int maxConnectionRetryDelaySeconds = 30,
            ICollection<int> transientSqlErrorNumbersToAdd = null,
            bool useSql2008Compatibility = false
            )
        {
            //services.AddEntityFrameworkSqlServer()
            //    .AddDbContext<SimpleContentDbContext>(optionsBuilder =>
            //        optionsBuilder.UseSqlServer(connectionString)
            //       );

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<SimpleContentDbContext>(optionsBuilder =>
                    optionsBuilder.UseSqlServer(connectionString,
                   sqlServerOptionsAction: sqlOptions =>
                   {
                       if (maxConnectionRetryCount > 0)
                       {
                           //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                           sqlOptions.EnableRetryOnFailure(
                               maxRetryCount: maxConnectionRetryCount,
                               maxRetryDelay: TimeSpan.FromSeconds(maxConnectionRetryDelaySeconds),
                               errorNumbersToAdd: transientSqlErrorNumbersToAdd);
                       }
                       if (useSql2008Compatibility)
                       {
                           sqlOptions.UseRowNumberForPaging();
                       }


                   }));

            services.AddScoped<ISimpleContentDbContext, SimpleContentDbContext>();

            services.AddCloudscribeSimpleContentEFStorageCommon();
            services.AddScoped<IStorageInfo, StorageInfo>();

            return services;
        }

    }
}
