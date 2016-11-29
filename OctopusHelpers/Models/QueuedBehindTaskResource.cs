using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers.Models
{
    /// <summary>
    /// The objects pulled from the QueuedBehind link dont really seem to be tasks so this was created
    /// </summary>
    public class QueuedBehindTaskResource
    {
        /// <summary>
        /// ID of the task ahead of it
        /// </summary>
        [JsonProperty(Order = 1)]
        public string Id { get; set; }
        /// <summary>
        /// Name of the Task
        /// </summary>
        [JsonProperty(Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Arguments passeds?
        /// </summary>
        [JsonProperty(Order = 3)]
        public Dictionary<string, object> Arguments { get; set; }

        /// <summary>
        /// Description of the Object
        /// </summary>
        [JsonProperty(Order = 4)]
        public string Description { get; set; }
        /// <summary>
        /// Time in the Queue
        /// </summary>
        [JsonProperty(Order = 5)]
        public DateTimeOffset? QueueTime { get; set; }
        /// <summary>
        /// How long until the task expires
        /// </summary>
        [JsonProperty(Order = 6)]
        public string QueueTimeExpiry { get; set; }

        /// <summary>
        /// Start time of the task
        /// </summary>
        [JsonProperty(Order = 7)]
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// Time the task was last updated
        /// </summary>
        [JsonProperty(Order = 8)]
        public DateTimeOffset? LastUpdatedTime { get; set; }

        /// <summary>
        /// Time it was completed (This is really a moot point)
        /// </summary>
        [JsonProperty(Order = 9)]
        public DateTimeOffset? CompletedTime { get; set; }

        /// <summary>
        /// Any Error Messages Associated with it
        /// </summary>
        [JsonProperty(Order = 10)]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Project and Environment as projects-[#]/environments-[#]
        /// </summary>
        [JsonProperty(Order = 11)]
        public string ConcurrencyTag { get; set; }

        /// <summary>
        /// Current State
        /// </summary>
        [JsonConverter(typeof (StringEnumConverter))]
        [JsonProperty(Order = 12)]
        public TaskState State { get; set; }

        /// <summary>
        /// The Id of the actor currently orchastrating the task
        /// </summary>
        [JsonProperty(Order = 13)]
        public string ActorId { get; set; }

        /// <summary>
        /// If the task has pending interruptions
        /// </summary>
        [JsonProperty(Order = 14)]
        public bool HasPendingInterruptions { get; set; }

        /// <summary>
        /// The Id of the actor currently orchastrating the task
        /// </summary>
        [JsonProperty(Order = 14)]
        public bool HasWarningsOrErrors { get; set; }

        /// <summary>
        /// ProjectId of the task
        /// </summary>
        [JsonProperty(Order = 15)]
        public string ProjectId { get; set; }

        /// <summary>
        /// EnvironmentId of the task
        /// </summary>
        [JsonProperty(Order = 16)]
        public string EnvironmentId { get; set; }

        /// <summary>
        /// I dont know what this does
        /// </summary>
        [JsonProperty(Order = 17)]
        public List<object> ExtendedViewLogPermissions { get; set; }

        /// <summary>
        /// I dont know what this does
        /// </summary>
        [JsonProperty(Order = 18)]
        public List<string> ExtendedCreatePermissions { get; set; }

        /// <summary>
        /// A unique description of the task
        /// </summary>
        [JsonProperty(Order = 19)]
        public string DescriptionUnique { get; set; }

        /// <summary>
        /// If the task can be re-run
        /// </summary>
        [JsonProperty(Order = 20)]
        public bool CanRerun { get; set; }

        /// <summary>
        /// Returns the DeploymentId ofthe Task if any exists
        /// </summary>
        public string DeploymentId
        {
            get
            {
                if(Arguments.ContainsKey(ResourceStrings.DeploymentIdKey))
                {
                    return Arguments[ResourceStrings.DeploymentIdKey].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
