using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    public static class StepHelper
    {
        /// <summary>
        /// Gathers the deployment steps for the passed release and environment
        /// </summary>
        /// <param name="octRepository"></param>
        /// <param name="release"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static IEnumerable<DeploymentTemplateStep> GetProjectEnvironmentDeploymentSteps(OctopusRepository octRepository, ReleaseResource release, EnvironmentResource environment)
        {
            var releaseTemplate = octRepository.Releases.GetTemplate(release);
            var deploymentPromotionTarget = releaseTemplate.PromoteTo.SingleOrDefault(x => x.Id.Equals(environment.Id, StringComparison.OrdinalIgnoreCase));
            return octRepository.Releases.GetPreview(deploymentPromotionTarget).StepsToExecute;
        }

        /// <summary>
        /// Returns the steps that are to be skipped based on two projects step lists
        /// This is used when managing octopus from octopus.
        /// YES I KNOW
        /// </summary>
        /// <param name="projectSteps"></param>
        /// <param name="referenceProject"></param>
        /// <returns></returns>
        public static ReferenceCollection GetStepsToSkipFromProjects(List<DeploymentTemplateStep> projectSteps, List<DeploymentTemplateStep> referenceProject)
        {
            var skippedSteps = new ReferenceCollection();
            if (!referenceProject.Exists(i => i.ActionName.Equals(ResourceStrings.MetaStepName)))
            {
                var skippedStepList = projectSteps.Where(s => !referenceProject.Any(s2 => s2.ActionName.Equals(s.ActionName, StringComparison.OrdinalIgnoreCase)));
                foreach (var stepToSkip in skippedStepList)
                {
                    skippedSteps.Add(stepToSkip.ActionId);
                }
            }
            return skippedSteps;
        }
    }
}
