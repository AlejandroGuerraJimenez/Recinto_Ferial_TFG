using System;
using UnityEngine;

namespace Fairground.View.Attractions
{
    /// <summary>
    /// Shared booth shell for modules: environment, placement, then play support.
    /// </summary>
    public static class BoothModuleShell
    {
        public static TBuilder Build<TBuilder>(AttractionBuildContext context, Func<GameObject, TBuilder> create)
            where TBuilder : BoothBuilder
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (create == null)
                throw new ArgumentNullException(nameof(create));

            TBuilder builder = create(context.XrOriginPrefab);
            builder.BuildShell(
                AttractionSceneSupport.ResolveBakedEnvironment(context.BakedEnvironment),
                context.BoothEnvironmentPrefab);
            AttractionPlacement.TryApply(context.Visit, builder.BoothSpaceRoots());
            return builder;
        }

        public static void AttachPlay(GameObject xrOrigin, AttractionInput input, DesktopPlayProfile profile)
        {
            AttractionSceneSupport.CreateDesktopPlay(xrOrigin, input, profile);
            AttractionSceneSupport.CreateVrCamera(xrOrigin);
        }
    }
}
