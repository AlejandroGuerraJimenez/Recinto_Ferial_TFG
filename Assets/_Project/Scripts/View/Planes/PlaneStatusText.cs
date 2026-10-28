using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Fairground.View
{
    /// <summary>
    /// Status sentences for the plane menu. The same stamp keeps the previous sentence.
    /// </summary>
    public sealed class PlaneStatusText
    {
        public const string QuitLine = "\nY del mando izquierdo cierra la app.";

        const string DeniedText =
            "Sin permiso de escena.\nPulsa B para volver a pedir el aviso.\nSi no sale: Ajustes, Aplicaciones, permisos, datos espaciales." + QuitLine;
        const string WaitingText =
            "Permiso de escena.\nSi sale el aviso, acéptalo. La búsqueda empieza igual." + QuitLine;
        const string LockedText =
            "Plano fijado. El resto queda fuera.\nApunta una parcela y pulsa el gatillo.\nB suelta el plano." + QuitLine;
        const string ChoosingLine = " planos. El marcado es la propuesta.\nGatillo: el apuntado, o esa si no apuntas.";

        enum StatusKind
        {
            None,
            Denied,
            WaitingPermission,
            Scanning,
            Locked,
            Choosing,
        }

        StatusKind _kind;
        int _primary;
        int _secondary;
        int _tertiary;
        int _sample;
        string _text = string.Empty;

        public string Current(
            bool permissionDenied,
            bool permissionSettled,
            bool subsystemRunning,
            int candidateCount,
            bool locked,
            ARPlaneManager manager,
            Func<int, string> scanningReport)
        {
            if (permissionDenied)
                return Remember(StatusKind.Denied, 0, 0, 0, 0, DeniedText);

            if (!permissionSettled && !subsystemRunning)
                return Remember(StatusKind.WaitingPermission, 0, 0, 0, 0, WaitingText);

            if (candidateCount == 0)
            {
                var tracked = Count(manager);
                var session = (int)ARSession.state;
                var subsystem = SubsystemStamp(manager);
                var sample = FirstPlaneStamp(manager);
                if (Same(StatusKind.Scanning, tracked, session, subsystem, sample))
                    return _text;

                var report = scanningReport != null ? scanningReport(tracked) : string.Empty;
                return Store(StatusKind.Scanning, tracked, session, subsystem, sample, report + QuitLine);
            }

            if (locked)
                return Remember(StatusKind.Locked, 0, 0, 0, 0, LockedText);

            if (Same(StatusKind.Choosing, candidateCount, 0, 0, 0))
                return _text;

            return Store(StatusKind.Choosing, candidateCount, 0, 0, 0, candidateCount + ChoosingLine + QuitLine);
        }

        string Remember(StatusKind kind, int primary, int secondary, int tertiary, int sample, string text)
        {
            if (Same(kind, primary, secondary, tertiary, sample))
                return _text;

            return Store(kind, primary, secondary, tertiary, sample, text);
        }

        bool Same(StatusKind kind, int primary, int secondary, int tertiary, int sample)
        {
            return _kind == kind
                && _primary == primary
                && _secondary == secondary
                && _tertiary == tertiary
                && _sample == sample;
        }

        string Store(StatusKind kind, int primary, int secondary, int tertiary, int sample, string text)
        {
            _kind = kind;
            _primary = primary;
            _secondary = secondary;
            _tertiary = tertiary;
            _sample = sample;
            _text = text;
            return text;
        }

        static int SubsystemStamp(ARPlaneManager manager)
        {
            if (manager == null)
                return 0;

            var subsystem = manager.subsystem;
            if (subsystem == null)
                return 1;

            return subsystem.running ? 3 : 2;
        }

        static int FirstPlaneStamp(ARPlaneManager manager)
        {
            if (manager == null)
                return 0;

            foreach (var plane in manager.trackables)
            {
                if (plane == null)
                    continue;

                var boundary = plane.boundary;
                var points = boundary.IsCreated ? boundary.Length : 0;
                var absorbed = plane.subsumedBy != null ? 1 : 0;
                var id = plane.trackableId;
                unchecked
                {
                    var stamp = points;
                    stamp = stamp * 31 + absorbed;
                    stamp = stamp * 31 + (int)plane.alignment;
                    stamp = stamp * 31 + (int)plane.classifications;
                    stamp = stamp * 31 + id.GetHashCode();
                    return stamp;
                }
            }

            return 0;
        }

        static int Count(ARPlaneManager manager)
        {
            if (manager == null)
                return 0;

            var count = 0;
            foreach (var plane in manager.trackables)
            {
                if (plane != null)
                    count++;
            }

            return count;
        }
    }
}
