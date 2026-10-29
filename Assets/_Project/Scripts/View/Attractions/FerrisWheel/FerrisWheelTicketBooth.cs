using Fairground.View.Attractions.Rendering;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fairground.View.Attractions.FerrisWheel
{
    /// <summary>
    /// Ticket booth on the spawn side of the platform. Yes seats the player in the cabin that is at the bottom.
    /// </summary>
    public sealed class FerrisWheelTicketBooth : MonoBehaviour
    {
        public const string RiderAnchorName = "RiderAnchor";

        const float ReachMeters = FerrisWheelSiteLayout.TicketReachMeters;

        FerrisWheelRigView _rig;
        GameObject _xrOrigin;
        Transform _yes;
        Transform _no;
        TextMeshPro _question;
        bool _boarded;
        InputAction _select;

        public static void Place(Transform axis, FerrisWheelRigView rig, GameObject xrOrigin)
        {
            if (axis == null || rig == null)
                return;

            float floorY = -FerrisWheelSiteLayout.FootBelowHubMeters;
            var root = new GameObject("TicketBooth");
            root.transform.SetParent(axis, false);
            root.transform.localPosition = new Vector3(0f, floorY, FerrisWheelSiteLayout.PlayerLocalZ + 7f);
            root.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            BuildStall(root.transform);
            Transform yes = BuildButton(root.transform, "TicketYes", new Vector3(-0.55f, 1.2f, 0.72f), new Color(0.18f, 0.55f, 0.28f, 1f));
            Transform no = BuildButton(root.transform, "TicketNo", new Vector3(0.55f, 1.2f, 0.72f), new Color(0.62f, 0.18f, 0.18f, 1f));
            BuildLabel(root.transform, "YesLabel", new Vector3(-0.55f, 1.2f, 0.8f), new Vector2(0.7f, 0.32f), "Sí");
            BuildLabel(root.transform, "NoLabel", new Vector3(0.55f, 1.2f, 0.8f), new Vector2(0.7f, 0.32f), "No");
            TextMeshPro question = BuildLabel(root.transform, "Question", new Vector3(0f, 1.9f, 0.02f), new Vector2(2.3f, 0.7f), "¿Quieres subir\na la noria?");
            BuildLabel(root.transform, "Hint", new Vector3(0f, 0.72f, 0.8f), new Vector2(2.2f, 0.28f), "Gatillo, o Y / N");

            var booth = root.AddComponent<FerrisWheelTicketBooth>();
            booth.Configure(rig, xrOrigin, yes, no, question);
        }

        void Configure(FerrisWheelRigView rig, GameObject xrOrigin, Transform yes, Transform no, TextMeshPro question)
        {
            _rig = rig;
            _xrOrigin = xrOrigin;
            _yes = yes;
            _no = no;
            _question = question;
        }

        void OnEnable()
        {
            _select = Button("TicketSelect");
            _select.AddBinding("<XRController>{RightHand}/triggerPressed");
            _select.AddBinding("<XRController>{RightHand}/primaryButton");
            _select.Enable();
        }

        void OnDisable()
        {
            _select?.Disable();
        }

        void OnDestroy()
        {
            _select?.Dispose();
            _select = null;
        }

        void Update()
        {
            if (_boarded || _rig == null)
                return;

            Camera camera = HeadsetCamera();
            if (camera == null)
                return;

            if (Vector3.Distance(camera.transform.position, transform.position) > ReachMeters)
                return;

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.yKey.wasPressedThisFrame)
            {
                Board();
                return;
            }

            if (keyboard != null && keyboard.nKey.wasPressedThisFrame)
            {
                Decline();
                return;
            }

            if (_select == null || !_select.WasPressedThisFrame())
                return;

            if (LooksAt(camera, _yes))
                Board();
            else if (LooksAt(camera, _no))
                Decline();
        }

        static bool LooksAt(Camera camera, Transform button)
        {
            if (button == null)
                return false;

            Ray ray = new Ray(camera.transform.position, camera.transform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, ReachMeters))
                return false;

            return hit.transform == button || hit.transform.IsChildOf(button);
        }

        void Board()
        {
            Transform cabin = LowestCabinNow();
            if (cabin == null || _xrOrigin == null)
            {
                Debug.LogError("[FerrisWheel] No cabin is available to board.");
                return;
            }

            Transform body = cabin.Find("Body");
            float inverse = 1f;
            if (cabin.lossyScale.x > 0.001f)
                inverse = 1f / cabin.lossyScale.x;

            var anchor = new GameObject(RiderAnchorName);
            anchor.transform.SetParent(cabin, false);
            anchor.transform.localPosition = body != null ? body.localPosition : new Vector3(0f, -0.4f, 0f);
            anchor.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            anchor.transform.localScale = Vector3.one * inverse;

            Transform origin = _xrOrigin.transform;
            origin.SetParent(anchor.transform, false);
            origin.localPosition = Vector3.zero;
            origin.localRotation = Quaternion.identity;
            origin.localScale = Vector3.one;

            AttractionStickMove move = _xrOrigin.GetComponent<AttractionStickMove>();
            if (move != null)
                move.enabled = false;

            _boarded = true;
            if (_question != null)
                _question.text = "¡Buen viaje!";
            Hide(_yes);
            Hide(_no);
            Hide(transform.Find("YesLabel"));
            Hide(transform.Find("NoLabel"));
            Hide(transform.Find("Hint"));
        }

        void Decline()
        {
            if (_question != null)
                _question.text = "Cuando quieras.";
        }

        Transform LowestCabinNow()
        {
            Transform lowest = _rig.GetCabin(0);
            for (int i = 1; i < _rig.CabinCount; i++)
            {
                Transform cabin = _rig.GetCabin(i);
                if (cabin.position.y < lowest.position.y)
                    lowest = cabin;
            }

            return lowest;
        }

        Camera HeadsetCamera()
        {
            if (_xrOrigin != null)
            {
                Camera camera = _xrOrigin.GetComponentInChildren<Camera>();
                if (camera != null)
                    return camera;
            }

            return Camera.main;
        }

        static void Hide(Transform button)
        {
            if (button == null)
                return;

            button.gameObject.SetActive(false);
        }

        static Transform BuildButton(Transform parent, string name, Vector3 localPosition, Color color)
        {
            var button = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button.name = name;
            button.transform.SetParent(parent, false);
            button.transform.localPosition = localPosition;
            button.transform.localScale = new Vector3(0.8f, 0.42f, 0.08f);
            PrimitiveMaterialApplier.Apply(button, color);
            return button.transform;
        }

        static void BuildStall(Transform parent)
        {
            Block(parent, "Counter", new Vector3(0f, 0.55f, 0.3f), new Vector3(2.4f, 1.1f, 0.55f), new Color(0.45f, 0.28f, 0.16f, 1f));
            Block(parent, "Wall", new Vector3(0f, 1.45f, -0.15f), new Vector3(2.6f, 2.7f, 0.12f), new Color(0.12f, 0.22f, 0.48f, 1f));
            Block(parent, "Lintel", new Vector3(0f, 2.75f, 0.1f), new Vector3(2.8f, 0.12f, 1f), new Color(0.1f, 0.16f, 0.36f, 1f));
            BuildLabel(parent, "Title", new Vector3(0f, 2.4f, 0.02f), new Vector2(2.2f, 0.4f), "TAQUILLA");
        }

        static void Block(Transform parent, string name, Vector3 localPosition, Vector3 scale, Color color)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.SetParent(parent, false);
            block.transform.localPosition = localPosition;
            block.transform.localScale = scale;
            PrimitiveMaterialApplier.Apply(block, color);
        }

        static TextMeshPro BuildLabel(Transform parent, string name, Vector3 localPosition, Vector2 size, string value)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            var text = go.AddComponent<TextMeshPro>();
            text.font = TmpFontResolver.Font();
            text.text = value;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(1f, 0.93f, 0.55f, 1f);
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Overflow;
            text.rectTransform.sizeDelta = size;
            text.enableAutoSizing = true;
            text.fontSizeMin = 0.15f;
            text.fontSizeMax = 3f;
            return text;
        }

        static InputAction Button(string name) => new InputAction(name, InputActionType.Button);
    }
}
