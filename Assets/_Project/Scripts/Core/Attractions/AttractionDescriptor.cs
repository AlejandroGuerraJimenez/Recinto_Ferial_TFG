using System;

namespace Fairground.Core.Attractions
{
    /// <summary>
    /// One attraction: stable id, visible name, and scene name.
    /// </summary>
    public sealed class AttractionDescriptor
    {
        public AttractionDescriptor(AttractionId id, string displayName, string sceneName)
        {
            if (id == AttractionId.None)
                throw new ArgumentOutOfRangeException(nameof(id), id, "An attraction needs an id.");
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name is required.", nameof(displayName));
            if (string.IsNullOrWhiteSpace(sceneName))
                throw new ArgumentException("Scene name is required.", nameof(sceneName));

            Id = id;
            DisplayName = displayName;
            SceneName = sceneName;
        }

        public AttractionId Id { get; }
        public string DisplayName { get; }
        public string SceneName { get; }
    }
}
