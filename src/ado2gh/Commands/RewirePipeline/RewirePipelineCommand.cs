﻿using System;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using OctoshiftCLI.AdoToGithub.Factories;
using OctoshiftCLI.Commands;
using OctoshiftCLI.Services;

namespace OctoshiftCLI.AdoToGithub.Commands.RewirePipeline
{
    public class RewirePipelineCommand : CommandBase<RewirePipelineCommandArgs, RewirePipelineCommandHandler>
    {
        public RewirePipelineCommand() : base(
            name: "rewire-pipeline",
            description: "Updates an Azure Pipeline to point to a GitHub repo instead of an Azure Repo." +
                         Environment.NewLine +
                         "Note: Expects ADO_PAT env variable or --ado-pat option to be set.")
        {
            AddOption(AdoOrg);
            AddOption(AdoTeamProject);
            AddOption(AdoPipeline);
            AddOption(GithubOrg);
            AddOption(GithubRepo);
            AddOption(ServiceConnectionId);
            AddOption(AdoPat);
            AddOption(Verbose);
            AddOption(TargetApiUrl);
        }

        public Option<string> AdoOrg { get; } = new("--ado-org")
        {
            IsRequired = true
        };
        public Option<string> AdoTeamProject { get; } = new("--ado-team-project")
        {
            IsRequired = true
        };
        public Option<string> AdoPipeline { get; } = new("--ado-pipeline")
        {
            IsRequired = true,
            Description = "The path and/or name of your pipeline. If the pipeline is in the root pipeline folder this can be just the name. Otherwise you need to specify the full pipeline path (E.g. \\Services\\Finance\\CI-Pipeline)"
        };
        public Option<string> GithubOrg { get; } = new("--github-org")
        {
            IsRequired = true
        };
        public Option<string> GithubRepo { get; } = new("--github-repo")
        {
            IsRequired = true
        };
        public Option<string> ServiceConnectionId { get; } = new("--service-connection-id")
        {
            IsRequired = true
        };
        public Option<string> AdoPat { get; } = new("--ado-pat");
        public Option<bool> Verbose { get; } = new("--verbose");
        public Option<string> TargetApiUrl { get; } = new("--target-api-url")
        {
            Description = "The URL of the target API, if not migrating to github.com. Defaults to https://api.github.com"
        };

        public override RewirePipelineCommandHandler BuildHandler(RewirePipelineCommandArgs args, IServiceProvider sp)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (sp is null)
            {
                throw new ArgumentNullException(nameof(sp));
            }

            var log = sp.GetRequiredService<OctoLogger>();
            var adoApiFactory = sp.GetRequiredService<AdoApiFactory>();
            var adoApi = adoApiFactory.Create(args.AdoPat);

            return new RewirePipelineCommandHandler(log, adoApi);
        }
    }
}
