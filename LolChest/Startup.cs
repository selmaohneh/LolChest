using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RiotSharp;
using RiotSharp.Caching;
using RiotSharp.Endpoints.Interfaces;
using RiotSharp.Interfaces;

[assembly: FunctionsStartup(typeof(LolChest.Startup))]

namespace LolChest
{
    public class Startup : FunctionsStartup
    {
        /// <summary>
        /// This is the root of the functions app.
        /// It is used to initialize all services that are
        /// used via dependency injections.
        /// </summary>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<CloudTables>();
            builder.Services.AddSingleton<LolChestConfig>();

            builder.Services.AddSingleton<IRiotApi>(provider => RiotApi.GetInstance(
                provider.GetService<LolChestConfig>().ApiKey, new Dictionary<TimeSpan, int>
                {
                    {TimeSpan.FromSeconds(1), 20},
                    {TimeSpan.FromMinutes(2), 100}
                }, new Cache()));
            builder.Services.AddSingleton<ISummonerEndpoint>(provider => provider.GetService<IRiotApi>().Summoner);
            builder.Services.AddSingleton<IMatchEndpoint>(provider => provider.GetService<IRiotApi>().Match);
        }
    }
}