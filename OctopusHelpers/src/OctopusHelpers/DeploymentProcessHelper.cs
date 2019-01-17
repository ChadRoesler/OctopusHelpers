using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    /// <summary>
    /// Helpers for managing DeploymentProcess Objects.
    /// </summary>
    public static class DeploymentProcessHelper
    {
        /// <summary>
        /// Gathers the DeploymentProcessResource from a Project.
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project"></param>
        /// <returns>DeploymentProcessResource</returns>
        public static DeploymentProcessResource GetDeploymentProcessFromProject(OctopusRepository octRepository, ProjectResource project)
        {
            var deploymentProcess = octRepository.DeploymentProcesses.Get(project.DeploymentProcessId);
            return deploymentProcess;
        }

        /// <summary>
        /// returns step names that gather packages.
        /// </summary>
        /// <param name="deploymentProcess"></param>
        /// <returns>Enumerable of Package Step Names</returns>
        public static IEnumerable<string> GetPackageSteps(DeploymentProcessResource deploymentProcess)
        {
            return deploymentProcess.Steps.SelectMany(x => x.Actions).Where(y => y.ActionType.Equals(ResourceStrings.PackageActionType, StringComparison.OrdinalIgnoreCase)).Select(x => x.Name);
        }

        /// <summary>
        /// Updates the new Deployment Process with the info from another. (Look you cant update the Steps property, its read only for some reason)
        /// This ha changed but this is something ill need to look into more
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="newDeploymentProcess"></param>
        /// <param name="oldDeploymentProcess"></param>
        public static void UpdateDeploymentProcessFromDeploymentProcess(OctopusRepository octRepository, DeploymentProcessResource newDeploymentProcess, DeploymentProcessResource oldDeploymentProcess)
        {
            newDeploymentProcess.Id = oldDeploymentProcess.Id;
            newDeploymentProcess.ProjectId = oldDeploymentProcess.ProjectId;
            newDeploymentProcess.Version = oldDeploymentProcess.Version;
            newDeploymentProcess.Links = oldDeploymentProcess.Links;
            octRepository.DeploymentProcesses.Modify(newDeploymentProcess);
        }

        /// <summary>
        /// Updates the passed Project's Deployment Process
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project"></param>
        /// <param name="deploymentProcess"></param>
        public static void UpdateProjectDeploymentProcess(OctopusRepository octRepository, ProjectResource project, DeploymentProcessResource deploymentProcess)
        {
            var oldDeploymentProcess = GetDeploymentProcessFromProject(octRepository, project);
            UpdateDeploymentProcessFromDeploymentProcess(octRepository, deploymentProcess, oldDeploymentProcess);
        }

        /// <summary>
        /// Abandon all hope ye who enter here.
        /// Updates the passed Project's Release's Deployment Process to be that of the Projects.
        /// When you need to have old releases have their projects process.
        /// USE WITH CAUTION!
        /// THIS IS A REALLY DIRTY WAY TO DO THIS, BUT SOMETIMES YOU GOTTA DO YER DIRT.
        /// https://i.imgur.com/gPBuAgl.gif
        /// </summary>
        /// <param name="octRepository">The repository to call against.</param>
        /// <param name="project"></param>
        /// <param name="octDatabaseConnection"></param>
        public static void UpdateProjectReleaseDeploymentProcess(OctopusRepository octRepository, ProjectResource project, SqlConnection octDatabaseConnection)
        {
            var projectIdParameter = new SqlParameter(ResourceStrings.ProjectIdParameter, project.Id);
            using (var cmd = new SqlCommand(ResourceStrings.ProjectProcessUpdate, octDatabaseConnection))
            {
                cmd.Parameters.Add(projectIdParameter);
                try
                {
                    octDatabaseConnection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    throw new Exception(string.Format(ErrorStrings.SqlError, ResourceStrings.ProjectProcessUpdate, string.Format(ResourceStrings.ParameterPairings,projectIdParameter.ParameterName, projectIdParameter.Value.ToString()), octDatabaseConnection.ConnectionString, ex.Message));
                }
                finally
                {
                    if (octDatabaseConnection.State == ConnectionState.Open)
                    {
                        octDatabaseConnection.Close();
                    }
                }
            }
        }
    }
}
