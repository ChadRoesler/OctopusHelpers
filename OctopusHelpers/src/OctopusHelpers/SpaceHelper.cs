using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusHelpers.Constants;

namespace OctopusHelpers
{
    public static class SpaceHelper
    {
        public static SpaceResource GetSpaceByName(OctopusRepository octRepository, string spaceName)
        {
            return octRepository.Spaces.FindByName(spaceName);
        }

        public static IEnumerable<string> GetSpaceNames(OctopusRepository octRepository)
        {
            return octRepository.Spaces.FindAll().Select(x => x.Name);
        }

        public static SpaceResource GetSpaceById(OctopusRepository octRepository, string spaceId)
        {
            var numberOnly = new int();
            if (int.TryParse(spaceId, out numberOnly))
            {
                return octRepository.Spaces.Get(string.Format(ResourceStrings.SpaceIdFormat, spaceId));
            }
            else
            {
                return octRepository.Spaces.Get(spaceId);
            }
        }

        public static void MoveProjectToSpace(OctopusRepository octRepository, ProjectResource project, SpaceResource space)
        {
            project.SpaceId = space.Id;
            project.ProjectGroupId = null;
            octRepository.Projects.Modify(project);
        }

        public static void MoveProjectToSpace(OctopusRepository octRepository, ProjectResource project, SpaceResource space, ProjectGroupResource)
        {
            project.SpaceId = space.Id;
            project.ProjectGroupId = null;
            octRepository.Projects.Modify(project);
        }
    }
}
