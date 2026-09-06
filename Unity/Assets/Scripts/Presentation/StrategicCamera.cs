using UnityEngine;

namespace PowerAboveAll
{
    /// <summary>Kalıcı atlas bakışı; taktik kamera aynı Camera bileşenini geçici kullanabilir.</summary>
    public sealed class StrategicCamera : MonoBehaviour
    {
        public enum ScaleLevel { Region, France, Europe, World }
        public Vector3 FocusPoint { get; private set; }
        public float Distance { get; private set; } = 150;
        public float Yaw { get; private set; }
        public float Pitch { get; private set; } = 65;
        public ScaleLevel ZoomLevel => Distance > 900 ? ScaleLevel.World : Distance > 280 ? ScaleLevel.Europe : Distance > 75 ? ScaleLevel.France : ScaleLevel.Region;
        private Camera view;
        private Vector3 target, velocity, panVelocity;
        private float targetDistance = 150, targetYaw, targetPitch = 65;
        private float zoomVelocity, yawVelocity, pitchVelocity;
        private bool suspended, dragging;
        private Vector3 dragPoint;
        public const float MinimumDistance = .018f, MaximumDistance = 3400;
        public void HandleMapEvent(Event current, Vector3 screenPoint)
        {
            if (!view || suspended || current == null) return;
            if(current.type == EventType.KeyDown && current.keyCode == KeyCode.Home)
            {
                SetView(new Vector3(16,0,466),150,0,65);current.Use();
            }
            else if(current.type == EventType.ScrollWheel)
            {
                // IMGUI üç satır/detent taşır; legacy mouseScrollDelta toplu girdiyi tek adıma indirgeyebilir.
                Zoom(-current.delta.y / 3f, screenPoint);current.Use();
            }
        }
        public void Zoom(float steps, Vector3 screenPoint)
        {
            if (!view || suspended || float.IsNaN(steps) || float.IsInfinity(steps)) return;
            float next = Mathf.Clamp(targetDistance * Mathf.Exp(-steps * .14f), MinimumDistance, MaximumDistance);
            if(Ground(screenPoint,out Vector3 underCursor))
                target += (underCursor-FocusPoint)*(1-next/targetDistance)*.65f;
            targetDistance = next;target=ClampFocus(target,targetDistance);
        }

        public void Initialize(Camera camera)
        {
            view = camera;
            SetView(new Vector3(16, 0, 466), 150, 0, 65, true);
        }

        public void SetView(Vector3 point, float distance, float yaw, float pitch, bool immediate = false)
        {
            targetDistance = Mathf.Clamp(distance, MinimumDistance, MaximumDistance);
            target = ClampFocus(point,targetDistance);
            targetYaw = Mathf.Repeat(yaw, 360);
            targetPitch = Mathf.Clamp(pitch, 35, 85);
            panVelocity = Vector3.zero; dragging = false;
            if (!immediate) return;
            FocusPoint = target; Distance = targetDistance; Yaw = targetYaw; Pitch = targetPitch;
            velocity = Vector3.zero; zoomVelocity = yawVelocity = pitchVelocity = 0;
            Apply();
        }

        public void Focus(Vector3 point, float distance = -1)
        {
            SetView(point, distance < 0 ? Mathf.Min(Distance, 60) : distance, targetYaw, targetPitch);
        }

        public void Suspend() { suspended = true; dragging = false; panVelocity = Vector3.zero; }
        public void Resume() { suspended = false; Apply(); }

        public static Vector3 ClampFocus(Vector3 point,float distance=0)
        {
            float far = Mathf.SmoothStep(0,1,Mathf.InverseLerp(900,MaximumDistance,distance));
            float x=Mathf.Lerp(1220,120,far),z=Mathf.Lerp(870,100,far);
            return new Vector3(Mathf.Clamp(point.x,-x,x),Mathf.Clamp(point.y,0,WorldMapEntities.Ground),Mathf.Clamp(point.z,-z,z));
        }

        public void Tick(bool allowInput)
        {
            if (!view || suspended) return;
            float dt = Mathf.Min(Time.unscaledDeltaTime, .05f);
            Vector3 desiredPan = Vector3.zero;
            if (allowInput)
            {
                float x = (Held(KeyCode.D, KeyCode.RightArrow) ? 1 : 0) - (Held(KeyCode.A, KeyCode.LeftArrow) ? 1 : 0);
                float z = (Held(KeyCode.W, KeyCode.UpArrow) ? 1 : 0) - (Held(KeyCode.S, KeyCode.DownArrow) ? 1 : 0);
                Vector3 direction = Quaternion.Euler(0, Yaw, 0) * Vector3.ClampMagnitude(new Vector3(x, 0, z), 1);
                desiredPan = direction * Mathf.Max(.008f, Distance * .65f) * (Input.GetKey(KeyCode.LeftShift) ? 1.8f : 1);
                float rotation = (Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0);
                targetYaw += rotation * 65 * dt;
                if (Input.GetMouseButton(1))
                {
                    targetYaw += Input.GetAxisRaw("Mouse X") * 3.1f;
                    targetPitch = Mathf.Clamp(targetPitch - Input.GetAxisRaw("Mouse Y") * 2.4f, 35, 85);
                }
                if (Input.GetMouseButtonDown(2)) dragging = Ground(Input.mousePosition, out dragPoint);
                if (dragging && Input.GetMouseButton(2) && Ground(Input.mousePosition, out Vector3 current))
                {
                    Vector3 delta = dragPoint - current;
                    target = ClampFocus(target + delta,targetDistance); FocusPoint = ClampFocus(FocusPoint + delta,targetDistance);
                    velocity = Vector3.zero; desiredPan = Vector3.zero; Apply();
                }
                if (!Input.GetMouseButton(2)) dragging = false;
            }
            else { dragging = false; panVelocity = Vector3.zero; }
            panVelocity = Vector3.Lerp(panVelocity, desiredPan, 1 - Mathf.Exp(-dt / (desiredPan.sqrMagnitude > 0 ? .16f : .10f)));
            target = ClampFocus(target + panVelocity * dt,targetDistance);
            FocusPoint = Vector3.SmoothDamp(FocusPoint, target, ref velocity, .13f, Mathf.Infinity, dt);
            Distance = Mathf.SmoothDamp(Distance, targetDistance, ref zoomVelocity, .20f, Mathf.Infinity, dt);
            Yaw = Mathf.SmoothDampAngle(Yaw, targetYaw, ref yawVelocity, .14f, Mathf.Infinity, dt);
            Pitch = Mathf.SmoothDamp(Pitch, targetPitch, ref pitchVelocity, .14f, Mathf.Infinity, dt);
            Apply();
        }

        private static bool Held(KeyCode a, KeyCode b) => Input.GetKey(a) || Input.GetKey(b);
        private bool Ground(Vector3 screen, out Vector3 point)
        {
            Ray ray = view.ScreenPointToRay(screen);
            if (new Plane(Vector3.up, new Vector3(0,Distance<2?WorldMapEntities.Ground:0,0)).Raycast(ray, out float d) && d < 15000)
            { point = ray.GetPoint(d); return true; }
            point = Vector3.zero; return false;
        }
        private void Apply()
        {
            if (!view) return;
            view.orthographic = false; view.fieldOfView = 45; view.nearClipPlane = Mathf.Clamp(Distance*.015f,.000002f,.2f); view.farClipPlane = Distance<2?20:10000;
            if(Distance<2){FocusPoint=new Vector3(FocusPoint.x,WorldMapEntities.Ground,FocusPoint.z);target=new Vector3(target.x,WorldMapEntities.Ground,target.z);}
            Quaternion rotation = Quaternion.Euler(Pitch, Yaw, 0);
            view.transform.SetPositionAndRotation(FocusPoint - rotation * Vector3.forward * Distance, rotation);
        }
    }
}
