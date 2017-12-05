using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing Step Objects.
    /// </summary>
    public static class StepHelper
    {
        /// <summary>
        /// Gathers the deployment steps for the passed release and environment.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="release">Release to gather steps from.</param>
        /// <param name="environment">Environment to gather steps from.</param>
        /// <returns>Enumerable of DeploymentTemplateSteps</returns>
        public static IEnumerable<DeploymentTemplateStep> GetReleaseEnvironmentDeploymentSteps(OctopusRepository octRepository, ReleaseResource release, EnvironmentResource environment)
        {
            var releaseTemplate = octRepository.Releases.GetTemplate(release);
            var deploymentPromotionTarget = releaseTemplate.PromoteTo.SingleOrDefault(x => x.Id.Equals(environment.Id, StringComparison.OrdinalIgnoreCase));
            return octRepository.Releases.GetPreview(deploymentPromotionTarget).StepsToExecute;
        }

        /// <summary>
        /// Returns the steps that are to be skipped based on two projects step lists.
        /// This is used when managing octopus from octopus.
        /// YES I KNOW.
        /// </summary>
        /// <param name="projectSteps">All steps from the project.</param>
        /// <param name="referenceProject">All Steps to keep.</param>
        /// <returns>Skipped steps as a ReferenceCollection (ugh)</returns>
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
